using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FuzzySharp;
using HospitalManagementSystem_HMS_.BL.AppRepo.Patient.IServices;
using HospitalManagementSystem_HMS_.BL.AppRepo.Utility.IService;
using HospitalManagementSystem_HMS_.BL.AuthRepo.Implementation;
using HospitalManagementSystem_HMS_.BL.AuthRepo.IServices;
using HospitalManagementSystem_HMS_.DB.Data;
using HospitalManagementSystem_HMS_.DB.Model.AppModel.AppointmentModel;
using HospitalManagementSystem_HMS_.DB.Model.AuthModel;
using HospitalManagementSystem_HMS_.Dtos.AppDtos;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.DoctorDtos;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.PatientDtos;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.SearchDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem_HMS_.BL.AppRepo.Patient.Implementation
{
    public class PatientService : IPatientService
    {
        private readonly HMSDBContext _patientDb;
        private readonly IMapper _mapper;
        //private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IPdfService _pdfService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientService(HMSDBContext patientDb, IMapper mapper, IEmailService emailService, IPdfService pdfService, IHttpContextAccessor httpContextAccessor)
        {
            _patientDb = patientDb;
            _mapper = mapper;
            _emailService = emailService;
            _pdfService = pdfService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<DoctorDtoForPatientView>> GetAllDoctorsByNameOrSpecialities(string name = null, string speciality = null)
        {
            try
            {
                var doctors = await _patientDb.Doctor_tbl
                    .Include(d => d.AppUser)
                    .Where(d => d.IsActive)
                    .ToListAsync();

                // Fuzzy match logic
                if (!string.IsNullOrWhiteSpace(name))
                {
                    doctors = doctors
                        .Where(d => Fuzz.PartialRatio(d.AppUser.Name, name) > 70) // 0 to 100
                        .ToList();
                }

                if (!string.IsNullOrWhiteSpace(speciality))
                {
                    doctors = doctors
                        .Where(d => Fuzz.PartialRatio(d.Specialization, speciality) > 70)
                        .ToList();
                }

                var allDoctorDtos = _mapper.Map<List<DoctorDtoForPatientView>>(doctors);

                if (allDoctorDtos == null || allDoctorDtos.Count == 0)
                {
                    throw new Exception("No doctors found with the provided name or specialization.");
                }

                return allDoctorDtos;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred: " + ex.Message);
            }
        }
        public async Task<string> BookNewAppointment(BookAppointmentDto appointmentDto, string patientId)
        {
            using var transaction = await _patientDb.Database.BeginTransactionAsync();
            try
            {
                if (appointmentDto == null)
                {
                    throw new ArgumentNullException("Appointment details cannot be null.");
                }
                var doctor = await _patientDb.Doctor_tbl
                 .Include(d => d.AppUser)
                 .FirstOrDefaultAsync(d => d.UserId == appointmentDto.DoctorId);
                if (doctor == null)
                {
                    throw new Exception("Doctor with the provided ID does not exist.");
                }

                var patient = await _patientDb.Users
                    .FirstOrDefaultAsync(p => p.Id == patientId);
                if (patient == null)
                {
                    throw new Exception("Patient with the provided ID does not exist.");
                }
                var newAppointment = new Appointment
                {
                    DoctorId = appointmentDto.DoctorId,
                    PatientId = patientId,
                    AppointmentDate = appointmentDto.AppointmentDate,
                    Remark = appointmentDto.Remark,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = null,
                    BookingDate = DateTime.Now,
                    Status = AppointmentStatus.Pending // Default status when booking
                };
                await _patientDb.Appointment_tbl.AddAsync(newAppointment);
                await _patientDb.SaveChangesAsync();
                // Code email to Admin about the new appointment,use email service---------------
                var adminRole = await _patientDb.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");

                if (adminRole == null)
                    throw new Exception("Admin role not found.");

                var adminUserId = await _patientDb.UserRoles
                    .Where(ur => ur.RoleId == adminRole.Id)
                    .Select(ur => ur.UserId)
                    .FirstOrDefaultAsync();

                if (adminUserId == null)
                    throw new Exception("No user found with Admin role.");

                var adminUser = await _patientDb.Users
                    .Where(u => u.Id == adminUserId)
                    .FirstOrDefaultAsync();

                if (adminUser == null || string.IsNullOrEmpty(adminUser.Email))
                {
                    throw new Exception("Admin email not found.");
                }

                string adminEmail = adminUser.Email;
                string subject = "New Appointment Notification";
                string body = $"Dear {adminUser.Name},<br/><br/>" +
                              $"A new appointment has been booked.<br/>" +
                              $"<strong>Doctor ID:</strong> {appointmentDto.DoctorId} and Doctor Name :{doctor.AppUser.Name}<br/>" +
                              $"<strong>Patient ID:</strong> {patientId} and Patient Name :{patient.Name}<br/>" +
                              $"<strong>Appointment Date:</strong> {appointmentDto.AppointmentDate}<br/>" +
                              $"<strong>Remark:</strong> {appointmentDto.Remark}<br/><br/>" +
                              $"Regards,<br/>Hospital Management System";

                await _emailService.SendEmailAsync(adminEmail, subject, body);
                await transaction.CommitAsync();

                return "Appointment booked and admin notified successfully.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }


        public async Task<string> MakeAppointmentPayment(string patientId,Guid appointmentId, decimal amount)
        {
            using var transaction = await _patientDb.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(patientId))
                    throw new ArgumentException("Patient ID cannot be null or empty.");
                var appointment = await _patientDb.Appointment_tbl.Include(a => a.Doctor)
                                                                  .Include(a => a.Patient)
                                                                   .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId && a.Patient.Id == patientId);
                if (appointment == null)
                    throw new Exception("Appointment not found.");

                var doctorFee = await _patientDb.Doctor_tbl
                    .Where(d => d.UserId == appointment.DoctorId)
                    .Select(d => d.ConsultantFee)
                    .FirstOrDefaultAsync();

                var payment = await _patientDb.AppointmentPayment_tbl
                                              .FirstOrDefaultAsync(p => p.AppointmentId == appointmentId);

                if (payment == null)
                {
                    payment = new AppointmentPayment
                    {
                        AppointmentId = appointmentId,
                        TotalAmount = doctorFee,
                        PaidAmount = 0,
                        PaymentStatus = PaymentStatus.Pending,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = null
                    };
                    await _patientDb.AppointmentPayment_tbl.AddAsync(payment);
                }

                payment.PaidAmount += amount;

                if (payment.PaidAmount >= doctorFee)
                {
                    payment.PaymentStatus = PaymentStatus.Successful;
                    appointment.IsPaid = true;
                    appointment.PaymentDate = DateTime.Now;

                    // Send Email to Doctor and Patient here (use EmailService)
                    // Subject
                    string subject = "Appointment Payment Confirmation - HMS";

                    // Doctor Email Body
                    string doctorBody = $@"
                    Dear {appointment.Doctor.Name},<br><br>
                    It is inform to you that your patient <strong>{appointment.Patient.Name}</strong> 
                    has successfully completed the payment for the appointment.<br><br>
                    
                    <b>Appointment Details:</b><br>
                    Appointment ID: {appointment.AppointmentId}<br>
                    Date: {appointment.AppointmentDate:dddd, dd MMM yyyy}<br>
                    Patient ID: {appointment.Patient.Id}<br>
                    Patient Email: {appointment.Patient.Email}<br>
                    Patient Name: {appointment.Patient.Name}<br>
                    Total Paid: ₹{payment.PaidAmount}<br><br>
                    
                    Please be prepared for the consultation.<br><br>
                    Regards,<br>
                    Hospital Management System";

                    // Patient Email Body
                    string patientBody = $@"
                    Dear {appointment.Patient.Name},<br><br>
                    Your payment of ₹{payment.PaidAmount} of Consultation fee for the appointment with {appointment.Doctor.Name} 
                    has been successfully received.<br><br>
                    
                    <b>Appointment Details:</b><br>
                    Appointment ID: {appointment.AppointmentId}<br>
                    Appointment Date: {appointment.AppointmentDate:dddd, dd MMM yyyy}<br>
                    Doctor: {appointment.Doctor.Name}<br>
                    Total Paid: ₹{payment.PaidAmount}<br><br>
                    
                    Please be available at the scheduled time.<br><br>
                    Thank you,<br>
                    Hospital Management System";
                    await _emailService.SendEmailAsync(appointment.Doctor.Email, subject, doctorBody);
                    await _emailService.SendEmailAsync(appointment.Patient.Email, subject, patientBody);
                }
                else
                {
                    payment.PaymentStatus = PaymentStatus.Pending; // Payment is still incomplete
                    appointment.IsPaid = false;

                    // Subject
                    string subject = "Partial Payment Acknowledgment - HMS";

                    // Patient Email Body
                    string patientBody = $@"
                We have received a partial payment of ₹{amount} for your upcoming appointment with Dr. {appointment.Doctor.Name}.<br><br>
                Dear {appointment.Patient.Name},<br><br>
                
                <b>Appointment Details:</b><br>
                Doctor Name: {appointment.Doctor.Name}<br>
                Consultant Fee : {doctorFee}<br>
                Amount Paid: ₹{payment.PaidAmount}<br>
                Remaining Amount: ₹{payment.TotalAmount - payment.PaidAmount}<br><br>
                
                Please complete the remaining payment to confirm your appointment and after full payment you will get the AppointmentId<br><br>
                Thank you,<br>
                Hospital Management System";

                    await _emailService.SendEmailAsync(appointment.Patient.Email, subject, patientBody);
                }

                await _patientDb.SaveChangesAsync();
                await transaction.CommitAsync();
                return payment.PaymentStatus == PaymentStatus.Successful ? "Payment Completed." : "Partial Payment Done.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Payment processing failed: " + ex.Message);
            }
        }


        public async Task<PendingLabTestListDto> GetPendingLabTestsForPaymentAsync(string patientId)
        {
            try
            {
                // ✅ Get all lab tests with Requested status for patient
                var labTests = await _patientDb.PatientLabTests_tbl
                    .Include(x => x.LabTest)
                    .Include(x => x.Doctor)
                    .Include(x => x.Patient)
                    .Where(x => x.PatientId == patientId && x.Status == LabTestStatus.Requested)
                    .ToListAsync();

                if (labTests == null || !labTests.Any())
                    throw new Exception("No pending lab tests found for payment.");

                // ✅ Prepare DTO
                var dto = new PendingLabTestListDto
                {
                    PatientId = patientId,
                    PatientName = labTests.First().Patient.Name,
                    DoctorName = labTests.DefaultIfEmpty().FirstOrDefault()?.Doctor?.Name ?? "N/A",
                    TotalAmount = labTests.Sum(x => x.LabTest.Cost),
                    Tests = labTests.Select(x => new PendingLabTestItemDto
                    {
                        PatientLabTestId = x.Id,
                        LabTestId=x.LabTestId,
                        TestName = x.LabTest.TestName,
                        SampleRequired = x.LabTest.SampleRequired,
                        Preparation = x.LabTest.Preparation,
                        TestDate = x.TestDate,
                        Cost = x.LabTest.Cost,
                    }).ToList()
                };

                return dto;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving pending lab tests: " + ex.Message);
            }
        }

        public async Task<string> PayForLabTestsAsync(LabTestPaymentRequestDto dto, string patientId)
        {
            try
            {
                if (dto == null || !dto.PatientLabTestIds.Any())
                    throw new ArgumentException("No lab tests selected for payment.");

                // ✅ Get relevant lab tests
                var labTests = await _patientDb.PatientLabTests_tbl
                    .Include(x => x.LabTest)
                    .Include(x => x.Patient)
                    .Where(x => dto.PatientLabTestIds.Contains(x.Id) &&
                                x.PatientId == patientId &&
                                x.Status == LabTestStatus.Requested)
                    .ToListAsync();

                if (labTests.Count != dto.PatientLabTestIds.Count)
                    throw new Exception("Some lab tests are invalid or already completed.");

                var totalCost = labTests.Sum(x => x.LabTest.Cost);

                // ✅ Find previous payments
                var previousPaymentIds = await _patientDb.LabTestPaymentMapping_tbl
                    .Where(m => dto.PatientLabTestIds.Contains(m.PatientLabTestId))
                    .Select(m => m.PaymentId)
                    .Distinct()
                    .ToListAsync();

                var previousPaidAmount = await _patientDb.LabTestPayment_tbl
                    .Where(p => previousPaymentIds.Contains(p.PaymentId))
                    .SumAsync(p => p.PaidAmount);

                var cumulativePaid = previousPaidAmount + dto.PaidAmount;
                var remainingAmount = totalCost - cumulativePaid;

                // ✅ Create new payment
                var payment = new LabTestPayment
                {
                    TotalAmount = totalCost,
                    PaidAmount = dto.PaidAmount,
                    IsPaid = cumulativePaid >= totalCost,
                    PaymentStatus = cumulativePaid >= totalCost ? LabPaymentStatus.Successful : LabPaymentStatus.Pending,
                    PaymentDate = DateTime.UtcNow
                };

                await _patientDb.LabTestPayment_tbl.AddAsync(payment);
                await _patientDb.SaveChangesAsync();

                // ✅ Add new mappings
                var mappings = labTests.Select(t => new LabTestPaymentMapping
                {
                    PatientLabTestId = t.Id,
                    PaymentId = payment.PaymentId
                }).ToList();

                await _patientDb.LabTestPaymentMapping_tbl.AddRangeAsync(mappings);

                // ✅ Update lab test statuses
                foreach (var test in labTests)
                {
                    if (cumulativePaid >= totalCost)
                        test.Status = LabTestStatus.Paid;

                    test.UpdatedAt = DateTime.UtcNow;
                }

                await _patientDb.SaveChangesAsync();

                // ✅ Email logic
                var patient = labTests.First().Patient;
                string subject, body;

                if (cumulativePaid >= totalCost)
                {
                    subject = "Lab Test Payment Confirmation";
                    body = $@"
                    <p>Dear <b>{patient.Name}</b>,</p>
                    <p>We have successfully received your full payment of ₹{cumulativePaid} for the following lab test(s):</p>
                    <ul>{string.Join("", labTests.Select(x => $"<li>{x.LabTest.TestName} - ₹{x.LabTest.Cost}</li>"))}</ul>
                    <p>Your tests are now scheduled and will be processed soon.</p>
                    <p>Thank you for trusting our services.</p>
                    <br/>
                    <p>Regards,<br/>Hospital Management System</p>";
                }
                else
                {
                    subject = "Partial Lab Test Payment Received";
                    body = $@"
                    <p>Dear <b>{patient.Name}</b>,</p>
                    <p>We have received ₹{dto.PaidAmount} towards your lab test(s), out of a total ₹{totalCost}.</p>
                    <p><b>Total Paid So Far:</b> ₹{cumulativePaid}<br/><b>Remaining Balance:</b> ₹{remainingAmount}</p>
                    <ul>{string.Join("", labTests.Select(x => $"<li>{x.LabTest.TestName} - ₹{x.LabTest.Cost}</li>"))}</ul>
                    <p>Please complete the full payment to proceed with the tests.</p>
                    <p>Until then, your lab tests remain on hold.</p>
                    <br/>
                    <p>Regards,<br/>Hospital Management System</p>";
                }

                await _emailService.SendEmailAsync(patient.Email, subject, body);

                return cumulativePaid >= totalCost
                    ? "Full payment received. Lab tests scheduled."
                    : $"₹{dto.PaidAmount} received. ₹{remainingAmount} remaining.";
            }
            catch (Exception ex)
            {
                throw new Exception("Payment processing failed: " + ex.Message);
            }
        }


        //public async Task<string> PayForLabTestsAsync(LabTestPaymentRequestDto dto, string patientId)
        //{
        //    try
        //    {
        //        if (dto == null || !dto.PatientLabTestIds.Any())
        //            throw new ArgumentException("No lab tests selected for payment.");

        //        // ✅ Fetch relevant lab tests
        //        var labTests = await _patientDb.PatientLabTests_tbl
        //            .Include(x => x.LabTest)
        //            .Include(x => x.Patient)
        //            .Where(x => dto.PatientLabTestIds.Contains(x.Id) &&
        //                        x.PatientId == patientId &&
        //                        x.Status == LabTestStatus.Requested)
        //            .ToListAsync();

        //        if (labTests.Count != dto.PatientLabTestIds.Count)
        //            throw new Exception("Some lab tests are either invalid or already processed.");

        //        var totalExpected = labTests.Sum(x => x.LabTest.Cost);

        //        // ✅ Create payment
        //        var payment = new LabTestPayment
        //        {
        //            TotalAmount = totalExpected,
        //            PaidAmount = dto.PaidAmount,
        //            IsPaid = dto.PaidAmount >= totalExpected,
        //            PaymentStatus = dto.PaidAmount >= totalExpected ? LabPaymentStatus.Successful : LabPaymentStatus.Pending,
        //            PaymentDate = DateTime.UtcNow
        //        };
        //        await _patientDb.LabTestPayment_tbl.AddAsync(payment);
        //        await _patientDb.SaveChangesAsync(); // Save to generate PaymentId

        //        // ✅ Map each PatientLabTest to the Payment
        //        var mappings = labTests.Select(test => new LabTestPaymentMapping
        //        {
        //            PaymentId = payment.PaymentId,
        //            PatientLabTestId = test.Id
        //        }).ToList();

        //        await _patientDb.LabTestPaymentMapping_tbl.AddRangeAsync(mappings);

        //        // ✅ Update PatientLabTests status
        //        foreach (var test in labTests)
        //        {
        //            test.Status = dto.PaidAmount >= totalExpected ? LabTestStatus.Paid : LabTestStatus.Requested;
        //            test.UpdatedAt = DateTime.UtcNow;
        //        }

        //        await _patientDb.SaveChangesAsync();

        //        // ✅ Prepare Email
        //        var patientName = labTests.First().Patient.Name;
        //        var patientEmail = labTests.First().Patient.Email;
        //        string subject = "";
        //        string body = "";

        //        if (dto.PaidAmount >= totalExpected)
        //        {
        //            // ✅ Full payment - confirmation email
        //            subject = "Lab Test Payment Confirmation";
        //            body = $@"
        //        <p>Dear <b>{patientName}</b>,</p>
        //        <p>We have successfully received your full payment of ₹{dto.PaidAmount} for the following lab test(s):</p>
        //        <ul>{string.Join("", labTests.Select(x => $"<li>{x.LabTest.TestName} - ₹{x.LabTest.Cost}</li>"))}</ul>
        //        <p>Your tests are now scheduled and will be processed soon.</p>
        //        <p>Thank you for trusting our services.</p>
        //        <br/>
        //        <p>Regards,<br/>Hospital Management System</p>";
        //        }
        //        else
        //        {
        //            // ❗ Partial payment - reminder email
        //            decimal remaining = totalExpected - dto.PaidAmount;
        //            subject = "Incomplete Lab Test Payment";
        //            body = $@"
        //        <p>Dear <b>{patientName}</b>,</p>
        //        <p>We have received a partial payment of ₹{dto.PaidAmount}, but the total amount for your selected lab test(s) is ₹{totalExpected}.</p>
        //        <p><b>Remaining Balance:</b> ₹{remaining}</p>
        //        <ul>{string.Join("", labTests.Select(x => $"<li>{x.LabTest.TestName} - ₹{x.LabTest.Cost}</li>"))}</ul>
        //        <p>Please complete the full payment to proceed with the test(s).</p>
        //        <p>Until then, your lab tests remain on hold.</p>
        //        <br/>
        //        <p>Regards,<br/>Hospital Management System</p>";
        //        }

        //        // ✅ Send email
        //        await _emailService.SendEmailAsync(patientEmail, subject, body);

        //        return dto.PaidAmount >= totalExpected
        //            ? "Lab test payment successful. Tests scheduled."
        //            : $"Partial payment of ₹{dto.PaidAmount} received. ₹{totalExpected - dto.PaidAmount} remaining.";
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Payment processing failed: " + ex.Message);
        //    }
        //}

        public async Task<PatientLabTestResultsListDto> GetCompletedLabResultsAsync(string patientId)
        {
            try
            {
                if(string.IsNullOrEmpty(patientId))
                    throw new ArgumentException("Invalid Patient Id.");
                var baseUrl = _httpContextAccessor.HttpContext?.Request.Scheme + "://" + _httpContextAccessor.HttpContext?.Request.Host;
                // 🔍 Step 1: Fetch patient + completed lab tests
                var labTests = await _patientDb.PatientLabTests_tbl
                    .Include(x => x.LabTest)
                    .Include(x => x.Doctor)
                    .Include(x => x.Patient)
                    .Where(x => x.PatientId == patientId && x.Status == LabTestStatus.Completed)
                    .OrderByDescending(x => x.TestDate)
                    .ToListAsync();

                if (!labTests.Any())
                    throw new Exception("No completed lab tests found for this patient.");

                var patientName = labTests.First().Patient.Name;

                // 🔄 Step 2: Convert each to DTO
                var resultList = labTests.Select(x => new PatientLabTestResultDto
                {
                    LabTestId = x.LabTest.LabTestId,
                    TestName = x.LabTest.TestName,
                    TestDate = x.TestDate,
                    Result = x.Result ?? "No result found",
                    ReportFileUrl = string.IsNullOrEmpty(x.ReportFilePath) ? null : $"{baseUrl}/{x.ReportFilePath}",
                    DoctorName = x.Doctor?.Name ?? "Unknown",
                    Status = x.Status.ToString()
                }).ToList();

                // 🔚 Step 3: Return final list
                return new PatientLabTestResultsListDto
                {
                    PatientId = patientId,
                    PatientName = patientName,
                    Results = resultList
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching lab results: " + ex.Message);
            }
        }

        public async Task<FileDto> DownloadLabReportAsync(Guid labTestId, string userId, List<string> roles)
        {
            try
            {
                // 🔍 Fetch the lab test
                var labTest = await _patientDb.PatientLabTests_tbl
                    .Include(x => x.LabTest)
                    .FirstOrDefaultAsync(x => x.LabTest.LabTestId == labTestId);

                if (labTest == null)
                    throw new Exception("Lab test not found.");

                // 🔐 Ownership check (keep roles logic unchanged)
                if (roles.Contains("Patient") && labTest.PatientId != userId)
                    throw new UnauthorizedAccessException("This report does not belong to you.");

                if (roles.Contains("Doctor") && labTest.DoctorId != userId)
                    throw new UnauthorizedAccessException("You did not prescribe this test.");

                if (!roles.Contains("Patient") && !roles.Contains("Doctor"))
                    throw new UnauthorizedAccessException("Your role is not allowed.");

                // 📁 File path validation
                if (string.IsNullOrEmpty(labTest.ReportFilePath))
                    throw new Exception("No report file uploaded for this test.");

                // ✅ Correct file path (assuming ReportFilePath = "LabReports/xyz.pdf")
                var fullPath = Path.Combine("wwwroot", labTest.ReportFilePath);

                if (!System.IO.File.Exists(fullPath))
                {
                    Console.WriteLine($"[DEBUG] File not found at: {fullPath}"); // Optional debug log
                    throw new FileNotFoundException("Report file not found on server.");
                }

                // 📦 Prepare FileDto
                var fileBytes = await File.ReadAllBytesAsync(fullPath);
                var fileName = $"{labTest.LabTest.TestName}_Report.pdf";

                return new FileDto
                {
                    FileBytes = fileBytes,
                    FileName = fileName,
                    ContentType = "application/pdf"
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error downloading lab report: " + ex.Message);
            }
        }


        public async Task<List<ShowAppointmentDto>> GetPatientAppointmentsAsync(string patientId)
        {
            try
            {
                if (string.IsNullOrEmpty(patientId))
                    throw new ArgumentException("Invalid Patient Id.");

                var result = await (
                    from a in _patientDb.Appointment_tbl
                    join doc in _patientDb.Users on a.DoctorId equals doc.Id
                    join pay in _patientDb.AppointmentPayment_tbl on a.AppointmentId equals pay.AppointmentId into payJoin
                    from p in payJoin.DefaultIfEmpty()
                    where a.PatientId == patientId
                    orderby a.AppointmentDate descending
                    select new ShowAppointmentDto
                    {
                        DoctorName = doc.Name,
                        AppointmentDate = a.AppointmentDate,
                        BookingDate = a.BookingDate,
                        Status = a.Status.ToString(),
                        IsPaid = a.IsPaid,
                        TotalAmount = p != null ? p.TotalAmount : 0,
                        PaidAmount = p != null ? p.PaidAmount : 0
                    }).ToListAsync();

                if (result == null || !result.Any())
                    throw new Exception("No appointments found for this patient.");

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while retrieving appointments.", ex);
            }
        }
        public async Task<PrescriptionDto> GetPrescriptionByIdAsync(string loggedInPatientId,Guid prescriptionId)
        {
            try
            {
                if (prescriptionId == Guid.Empty)
                    throw new ArgumentException("Prescription ID is required.");

                // 🔍 Validate Logged-In Patient ID
                if (string.IsNullOrEmpty(loggedInPatientId))
                    throw new ArgumentException("Patient ID is missing or user not authenticated.");

                // Step 2: Get the prescription with its details
                var prescription = await _patientDb.Prescription_tbl
                     .Include(p => p.Appointment)
                         .ThenInclude(a => a.Doctor)
                     .Include(p => p.PrescriptionDetails)
                     .FirstOrDefaultAsync(p =>
                         p.PrescriptionId == prescriptionId &&
                         p.Appointment.PatientId == loggedInPatientId &&     // ✅ Ensure access control
                         p.Appointment.Status == AppointmentStatus.Completed
                     );
                // Step 3: Handle not found
                if (prescription == null)
                    throw new Exception("Prescription not found.");

                var prescriptionDto = new PrescriptionDto
                {
                    PrescriptionId = prescription.PrescriptionId,
                    PrescribeDate = prescription.PrescribeDate,
                    Note = prescription.Note ?? string.Empty,
                    DoctorName = prescription.Appointment.Doctor?.Name ?? "N/A",
                    AppointmentDate = prescription.Appointment.AppointmentDate,
                    PrescriptionDetails = prescription.PrescriptionDetails.Select(d => new PrescriptionDetailsDto
                    {
                        MedicineName = d.MedicineName,
                        Dosage = d.Dosage,
                        Frequency = d.Frequency,
                        Duration = d.Duration,
                        Instructions = d.Instructions
                    }).ToList()
                };

                return prescriptionDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while retrieving prescriptions.", ex);
            }
        }
        public async Task<List<AppointmentHistoryDetailedDto>> GetAppointmentHistoryDetailedAsync(string patientId)
        {
            try
            {
                if (string.IsNullOrEmpty(patientId))
                    throw new ArgumentException("Patient ID is required.");

                // 1. Get all appointments with necessary joins
                var result = await (
                    from a in _patientDb.Appointment_tbl
                    join d in _patientDb.Users on a.DoctorId equals d.Id
                    join pay in _patientDb.AppointmentPayment_tbl on a.AppointmentId equals pay.AppointmentId into paymentJoin
                    from payment in paymentJoin.DefaultIfEmpty()
                    join pr in _patientDb.Prescription_tbl on a.AppointmentId equals pr.AppointmentId into prescJoin
                    from presc in prescJoin.DefaultIfEmpty()
                    join pd in _patientDb.PrescriptionDetails_tbl on presc.PrescriptionId equals pd.PrescriptionId into detailJoin
                    from detail in detailJoin.DefaultIfEmpty()
                    where a.PatientId == patientId &&
                          (a.Status == AppointmentStatus.Completed)
                    orderby a.AppointmentDate descending
                    select new
                    {
                        Appointment = a,
                        Doctor = d,
                        Payment = payment,
                        Prescription = presc,
                        PrescriptionDetail = detail
                    }).ToListAsync();

                // 2. Get all doctor info in one go
                var doctorUserIds = result.Select(r => r.Doctor.Id).Distinct().ToList();
                var doctorInfos = await _patientDb.Doctor_tbl
                    .Where(d => doctorUserIds.Contains(d.UserId))
                    .ToDictionaryAsync(d => d.UserId, d => new { d.Specialization, d.ConsultantFee ,d.Qualifications});

                // 3. Get patient disease
                var disease = await _patientDb.Patient_tbl
                    .Where(p => p.UserId == patientId)
                    .Select(p => p.ChronicDiseases)
                    .FirstOrDefaultAsync();

                // 4. Group by Appointment ID
                var appointmentHistory = result
                    .GroupBy(x => x.Appointment.AppointmentId)
                    .Select(g =>
                    {
                        var first = g.First();
                        var docInfo = doctorInfos.ContainsKey(first.Doctor.Id)
                                        ? doctorInfos[first.Doctor.Id]
                                        : null;

                        return new AppointmentHistoryDetailedDto
                        {
                            AppointmentId = first.Appointment.AppointmentId,
                            AppointmentDate = first.Appointment.AppointmentDate,
                            DoctorName = first.Doctor.Name,
                            Specialization = docInfo?.Specialization,
                            Qualifications = docInfo?.Qualifications?.Split(',').ToList() ?? new List<string>(),
                            ConsultationFee = docInfo?.ConsultantFee ?? 0,
                            IsPaid = first.Appointment.IsPaid,
                            TotalAmount = first.Payment?.TotalAmount ?? 0,
                            PaidAmount = first.Payment?.PaidAmount ?? 0,
                            Diagnosis = disease,
                            Notes = first.Prescription?.Note,
                            PrescriptionDate = first.Prescription?.PrescribeDate,
                            Medicines = g
                                .Where(x => x.PrescriptionDetail != null)
                                .Select(x => new PrescriptionMedicineDto
                                {
                                    MedicineName = x.PrescriptionDetail.MedicineName,
                                    Dosage = x.PrescriptionDetail.Dosage,
                                    Frequency = x.PrescriptionDetail.Frequency,
                                    Duration = x.PrescriptionDetail.Duration,
                                    Instructions = x.PrescriptionDetail.Instructions
                                }).ToList()
                        };
                    })
                    .ToList();

                return appointmentHistory;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching appointment history with prescription details.", ex);
            }
        }
        public async Task<byte[]> DownloadPrescriptionPdfAsync(Guid prescriptionId, string patientId)
        {
            try
            {
                // Step 1: Validate inputs
                if (prescriptionId == Guid.Empty)
                    throw new ArgumentException("Invalid prescription ID.");

                if (string.IsNullOrEmpty(patientId))
                    throw new ArgumentException("Invalid patient ID.");

                // Step 2: Fetch prescription with required joins
                var prescription = await _patientDb.Prescription_tbl
                    .Include(p => p.PrescriptionDetails)
                    .Include(p => p.Appointment)
                        .ThenInclude(a => a.Patient)
                    .Include(p => p.Appointment)
                        .ThenInclude(a => a.Doctor)
                    .FirstOrDefaultAsync(p =>
                        p.PrescriptionId == prescriptionId &&
                        p.Appointment.PatientId == patientId);

                if (prescription == null)
                    throw new Exception("Prescription not found or access denied.");

                var diagnosis = await _patientDb.Patient_tbl
                    .Where(p => p.UserId == patientId)
                    .Select(p => p.ChronicDiseases)
                    .FirstOrDefaultAsync();

                // Step 3: Prepare DTO or data for PDF
                var dto = new PrescriptionPdfDto
                {
                    DoctorName= prescription.Appointment.Doctor?.Name ?? "N/A",
                    PatientName = prescription.Appointment.Patient?.Name ?? "N/A",
                    Diagnosis = diagnosis ?? "N/A",
                    Date = prescription.PrescribeDate,
                    Note = prescription.Note ?? "N/A",
                    Medicines = prescription.PrescriptionDetails.Select(d => new PrescriptionMedicineDto
                    {
                        MedicineName = d.MedicineName,
                        Dosage = d.Dosage,
                        Frequency = d.Frequency,
                        Duration = d.Duration,
                        Instructions = d.Instructions
                    }).ToList()
                };

                // Step 4: Generate PDF from DTO
                var pdfBytes = _pdfService.GeneratePrescriptionPdf(dto);

                return pdfBytes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
