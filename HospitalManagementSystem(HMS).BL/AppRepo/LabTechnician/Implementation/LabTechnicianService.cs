using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HospitalManagementSystem_HMS_.BL.AppRepo.LabTechnician.IServices;
using HospitalManagementSystem_HMS_.BL.AuthRepo.IServices;
using HospitalManagementSystem_HMS_.DB.Data;
using HospitalManagementSystem_HMS_.DB.Model.AppModel.AppointmentModel;
using HospitalManagementSystem_HMS_.DB.Model.AuthModel;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.LabTechnicianDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UglyToad.PdfPig;

namespace HospitalManagementSystem_HMS_.BL.AppRepo.LabTechnician.Implementation
{
    public class LabTechnicianService : ILabTechnicianService
    {
        private readonly HMSDBContext _labTechnicianDb;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public LabTechnicianService(HMSDBContext labTechnicianDb, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _labTechnicianDb = labTechnicianDb;
            _userManager = userManager;
            _emailService = emailService;
        }
        public async Task<List<PatientLabTestGroupDto>> GetReadyLabTestsAsync()
        {
            try
            {
                // ✅ Fetch only Paid lab tests with required relations
                var readyTests = await _labTechnicianDb.PatientLabTests_tbl
                    .Include(x => x.Patient)
                    .Include(x => x.Doctor)
                    .Include(x => x.LabTest)
                    .Include(x => x.Appointment)
                    .Where(x => x.Status == LabTestStatus.Paid)
                    .OrderBy(x => x.Appointment.BookingDate)      // 1️⃣ First-come-first-served
                    .ThenBy(x => x.PatientId)                     // 2️⃣ Group by patient
                    .ThenByDescending(x => x.Priority)            // 3️⃣ Critical → Urgent
                    .ThenBy(x => x.CreatedAt)                     // 4️⃣ Oldest prescribed first
                    .ToListAsync();

                if (readyTests == null || !readyTests.Any())
                {
                    throw new Exception("No ready lab tests found.");
                }

                // ✅ Group by Patient
                var grouped = readyTests
                .GroupBy(x => new { x.PatientId, x.Patient.Name })
                .Select(group => new PatientLabTestGroupDto
                {
                    PatientId = group.Key.PatientId,
                    PatientName = group.Key.Name,
                    LabTests = group.Select(x => new LabTestItemDto
                    {
                        LabTestId = x.LabTest.LabTestId,
                        TestName = x.LabTest.TestName,
                        Status = x.Status.ToString(),
                        Priority = x.Priority.ToString(),
                        TestDate = x.TestDate,
                        Notes = x.Notes,
                        DoctorName = x.Doctor.Name,
                        AppointmentDate = x.Appointment.AppointmentDate
                    }).ToList()
                }).ToList();

                return grouped;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to fetch ready lab tests: " + ex.Message);
            }
        }
        public async Task<LabTestStartResponseDto> StartLabTestAsync(Guid labTestId)
        {
            try
            {
                var test = await _labTechnicianDb.PatientLabTests_tbl
                    .Include(x => x.Patient)
                    .Include(x => x.Doctor)
                    .Include(x => x.LabTest)
                    .FirstOrDefaultAsync(x => x.LabTestId == labTestId);

                if (test == null)
                    throw new Exception("Lab test not found.");
                if (test.Status == LabTestStatus.Completed)
                    throw new Exception("Lab test has already been completed.");
                if (test.Status == LabTestStatus.InProgress)
                {
                    throw new Exception("Lab test is already in progress.");
                }

                if (test.Status != LabTestStatus.Paid)
                    throw new Exception("Lab test is not ready to be started.");

                // ✅ Update status
                test.Status = LabTestStatus.InProgress;
                test.UpdatedAt = DateTime.UtcNow;

                await _labTechnicianDb.SaveChangesAsync();

                return new LabTestStartResponseDto
                {
                    LabTestId = test.LabTestId,
                    TestName = test.LabTest.TestName,
                    PatientName = test.Patient.Name,
                    DoctorName = test.Doctor.Name,
                    StartedAt = test.UpdatedAt.Value,
                    Status = test.Status.ToString()
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to start lab test: " + ex.Message);
            }
        }

        public async Task<LabTestResultResponseDto> SubmitLabTestResultAsync(Guid labTestId, LabTestResultUploadDto dto)
        {
            using var transaction = await _labTechnicianDb.Database.BeginTransactionAsync();
            try
            {
                var test = await _labTechnicianDb.PatientLabTests_tbl
                    .Include(x => x.Patient)
                    .Include(x => x.Doctor)
                    .Include(x => x.LabTest)
                    .FirstOrDefaultAsync(x => x.LabTest.LabTestId == labTestId);

                if (test == null)
                    throw new Exception("Lab test not found.");

                if (test.Status != LabTestStatus.InProgress)
                    throw new Exception("Only tests in progress can be completed.");

                // Step 1: Save PDF and extract result
                if (dto.ReportFile != null && dto.ReportFile.Length > 0)
                {
                    var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    Directory.CreateDirectory(wwwRootPath);

                    var reportFolder = Path.Combine(wwwRootPath, "LabReports");
                    Directory.CreateDirectory(reportFolder);

                    var fileName = $"LabReport_{labTestId}_{DateTime.UtcNow.Ticks}{Path.GetExtension(dto.ReportFile.FileName)}";
                    var fullPath = Path.Combine(reportFolder, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await dto.ReportFile.CopyToAsync(stream);
                    }

                    test.ReportFilePath = Path.Combine("LabReports", fileName).Replace("\\", "/");

                    // ✅ Extract text from PDF
                    string extractedText = "";
                    using (var pdf = UglyToad.PdfPig.PdfDocument.Open(fullPath))
                    {
                        foreach (var page in pdf.GetPages())
                        {
                            extractedText += page.Text;
                        }
                    }

                    var resultStart = extractedText.IndexOf("Result Summary:", StringComparison.OrdinalIgnoreCase);
                    if (resultStart >= 0)
                    {
                        var summaryText = extractedText.Substring(resultStart + "Result Summary:".Length).Trim();
                        dto.Result = summaryText.Length > 700 ? summaryText.Substring(0, 700) : summaryText;
                    }
                    else
                    {
                        throw new Exception("Could not find 'Result Summary' section in PDF.");
                    }
                }

                if (string.IsNullOrWhiteSpace(dto.Result))
                    throw new ArgumentException("Result content is missing and could not be extracted.");

                test.Result = dto.Result;
                test.Status = LabTestStatus.Completed;
                test.UpdatedAt = DateTime.UtcNow;

                await _labTechnicianDb.SaveChangesAsync();
                await transaction.CommitAsync();

                return new LabTestResultResponseDto
                {
                    LabTestId = test.LabTestId,
                    TestName = test.LabTest.TestName,
                    PatientName = test.Patient.Name,
                    DoctorName = test.Doctor.Name,
                    Status = test.Status.ToString(),
                    ReportFilePath = test.ReportFilePath,
                    UpdatedAt = test.UpdatedAt.Value
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to submit lab test result: " + ex.Message);
            }
        }

        //public async Task<LabTestResultResponseDto> SubmitLabTestResultAsync(Guid labTestId, LabTestResultUploadDto dto)
        //{
        //    using var transaction = await _labTechnicianDb.Database.BeginTransactionAsync();
        //    try
        //    {
        //        // 🔹 Step 1: LabTest record dhoondo
        //        var test = await _labTechnicianDb.PatientLabTests_tbl
        //            .Include(x => x.Patient)
        //            .Include(x => x.Doctor)
        //            .Include(x => x.LabTest)
        //            .FirstOrDefaultAsync(x => x.LabTest.LabTestId == labTestId);

        //        if (test == null)
        //            throw new Exception("Lab test not found.");

        //        // 🔹 Step 2: Check karo ki test InProgress hona chahiye
        //        if (test.Status != LabTestStatus.InProgress)
        //            throw new Exception("Only tests in progress can be completed.");

        //        // 🔹 Step 3: Result text empty nahi hona chahiye
        //        if (string.IsNullOrWhiteSpace(dto.Result))
        //            throw new ArgumentException("Result cannot be empty.");

        //        // ✅ Set result
        //        test.Result = dto.Result;

        //        // 🔹 Step 4: Agar file hai to save karo
        //        if (dto.ReportFile != null && dto.ReportFile.Length > 0)
        //        {
        //            // wwwroot folder create karo agar nahi hai to
        //            var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        //            Directory.CreateDirectory(wwwRootPath);

        //            // wwwroot/LabReports bhi create karo
        //            var reportFolder = Path.Combine(wwwRootPath, "LabReports");
        //            Directory.CreateDirectory(reportFolder);

        //            // FileName generate karo
        //            var fileName = $"LabReport_{labTestId}_{DateTime.UtcNow.Ticks}{Path.GetExtension(dto.ReportFile.FileName)}";
        //            var fullPath = Path.Combine(reportFolder, fileName);

        //            // File save karo
        //            using (var stream = new FileStream(fullPath, FileMode.Create))
        //            {
        //                await dto.ReportFile.CopyToAsync(stream);
        //            }

        //            // File path set karo DB ke liye
        //            test.ReportFilePath = Path.Combine("LabReports", fileName).Replace("\\", "/");
        //        }

        //        // 🔹 Step 5: Status change karo Completed par
        //        test.Status = LabTestStatus.Completed;
        //        test.UpdatedAt = DateTime.UtcNow;

        //        await _labTechnicianDb.SaveChangesAsync();
        //        await transaction.CommitAsync();

        //        // 🔹 Step 6: Prepare response
        //        return new LabTestResultResponseDto
        //        {
        //            LabTestId = test.LabTestId,
        //            TestName = test.LabTest.TestName,
        //            PatientName = test.Patient.Name,
        //            DoctorName = test.Doctor.Name,
        //            Status = test.Status.ToString(),
        //            ReportFilePath = test.ReportFilePath,
        //            UpdatedAt = test.UpdatedAt.Value
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        throw new Exception("Failed to submit lab test result: " + ex.Message);
        //    }
        //}
    }
}
