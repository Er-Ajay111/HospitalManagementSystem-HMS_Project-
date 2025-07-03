using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HospitalManagementSystem_HMS_.BL.AppRepo.Doctor.IServices;
using HospitalManagementSystem_HMS_.BL.AuthRepo.IServices;
using HospitalManagementSystem_HMS_.DB.Data;
using HospitalManagementSystem_HMS_.DB.Model.AppModel.AppointmentModel;
using HospitalManagementSystem_HMS_.DB.Model.AuthModel;
using HospitalManagementSystem_HMS_.Dtos.AppDtos;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.DoctorDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem_HMS_.BL.AppRepo.Doctor.Implementation
{
    public class DoctorService : IDoctorService
    {
        private readonly HMSDBContext _doctorDb;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public DoctorService(HMSDBContext doctorDb, IMapper mapper, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _doctorDb = doctorDb;
            _mapper = mapper;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<List<DoctorViewAppointmentDto>> GetDoctorAppointments(string doctorId)
        {
            var allAppointments = await (from a in _doctorDb.Appointment_tbl
                                      join p in _doctorDb.AppointmentPayment_tbl
                                          on a.AppointmentId equals p.AppointmentId
                                      where a.DoctorId == doctorId
                                            && a.Status == AppointmentStatus.Confirmed
                                            && a.IsPaid == true
                                            && p.PaymentStatus == PaymentStatus.Successful
                                      select new DoctorViewAppointmentDto
                                      {
                                          AppointmentId = a.AppointmentId,
                                          PatientName = a.Patient.Name,
                                          PatientGender= a.Patient.Gender,
                                          PatientAge = a.Patient.Age,
                                          AppointmentDate = a.AppointmentDate,
                                          TotalPaid = p.PaidAmount,
                                          Status = a.Status.ToString()
                                      }).ToListAsync();

            return allAppointments;
        }

        public async Task<List<LabTestDetailDto>> GetAvailableLabTestsAsync()
        {
            try
            {
                // Step 1: Fetch only active lab tests
                var labTests = await _doctorDb.LabTests_tbl
                    .Where(test => test.IsActive)
                    .ToListAsync();

                // Step 2: Handle case if no test found
                if (labTests == null || !labTests.Any())
                    return new List<LabTestDetailDto>();

                // Step 3: Map to DTO
                var testDtos = labTests.Select(test => new LabTestDetailDto
                {
                    LabTestId = test.LabTestId,
                    TestName = test.TestName,
                    SampleRequired = test.SampleRequired,
                    Preparation = test.Preparation,
                    Duration = test.Duration,
                    Cost = test.Cost
                }).ToList();

                return testDtos;
            }
            catch (Exception ex)
            {
                // Step 4: Handle/log exception (optional)
                throw new Exception("Error while fetching available lab tests.", ex);
            }
        }


        public async Task<PrescribeLabTestResponseDto> PrescribeLabTestsAsync(PrescribeLabTestDto prescribeLabTestDto, string doctorId)
        {
            try
            {
                if (prescribeLabTestDto == null)
                    throw new ArgumentException("Invalid lab test data.");

                if (!prescribeLabTestDto.LabTestIds.Any())
                    throw new ArgumentException("No lab tests provided.");


                // ✅ Validate confirmed appointment between doctor and patient
                var appointment = await _doctorDb.Appointment_tbl
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .FirstOrDefaultAsync(a => a.AppointmentId == prescribeLabTestDto.AppointmentId
                                              && a.Status == AppointmentStatus.Confirmed
                                              && a.IsPaid == true
                                              && a.DoctorId == doctorId);


                if (appointment == null)
                    throw new ArgumentException("No confirmed appointment found for this patient with the specified doctor.");

                if (appointment.DoctorId != doctorId)
                {
                    throw new UnauthorizedAccessException("You are not authorized to prescribe Lab tests for this patient.");
                }

                // ✅ Fetch valid LabTests
                var labTests = await _doctorDb.LabTests_tbl
                    .Where(t => prescribeLabTestDto.LabTestIds.Contains(t.LabTestId))
                    .ToListAsync();

                if (labTests.Count != prescribeLabTestDto.LabTestIds.Count)
                    throw new Exception("One or more lab tests are invalid.");

                // ✅ Create PatientLabTests
                var patientLabTests = labTests.Select(test => new PatientLabTests
                {
                    AppointmentId = appointment.AppointmentId,        // 🔴 Link to Appointment
                    PatientId = appointment.Patient.Id,         // 🔴 Still storing for easy filtering
                    DoctorId = doctorId,                               // 🔴 Still storing for audit/tracking
                    LabTestId = test.LabTestId,
                    TestDate = prescribeLabTestDto.TestDate,
                    Notes = prescribeLabTestDto.Notes,
                    Result = null,
                    ReportFilePath =null,
                    Priority = prescribeLabTestDto.Priority,
                    Status = LabTestStatus.Requested,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = null
                }).ToList();

                await _doctorDb.PatientLabTests_tbl.AddRangeAsync(patientLabTests);
                await _doctorDb.SaveChangesAsync();

                string subject = "Lab Test Prescription - Payment Required";
                string body = $@"
                       <p>Dear <b>{appointment.Patient.Name}</b>,</p>
                      
                       <p>We hope you're doing well.</p>
                      
                       <p>This is to inform you that during your recent consultation on <b>{appointment.AppointmentDate:dd MMM yyyy}</b>,your        doctor <   b>        {appointment.Doctor.Name}</b> has prescribed the following lab test(s):</p>
                      
                       <ul>
                           {string.Join("", labTests.Select(t => $"<li><b>{t.TestName}</b> - ₹{t.Cost}</li>"))}
                       </ul>
                      
                       <p><b>Total Cost:</b> ₹{labTests.Sum(t => t.Cost)}</p>
                      
                       <p><span style='color:red;'><b>Important:</b></span> These tests will only be scheduled once the full payment is made.</p>
                      
                       <p>Please log in to your hospital account dashboard and complete the payment at your earliest convenience to avoid any delays in       your        diagnosis     and    treatment.</p>
                      
                       <p>If you have any questions or need assistance, feel free to contact our support team.</p>
                      
                       <br/>
                       <p>Regards,</p>
                       <p><b>Hospital Management System</b><p>
                        ";

                await _emailService.SendEmailAsync(appointment.Patient.Email, subject, body);

                // ✅ Prepare response DTO
                var response = new PrescribeLabTestResponseDto
                {
                    PatientId = appointment.Patient.Id,
                    PatientName = appointment.Patient.Name,
                    OrderedById = doctorId,
                    DoctorName = appointment.Doctor.Name,
                    AppointmentId = appointment.AppointmentId,
                    TestDate = prescribeLabTestDto.TestDate,
                    LabTests = labTests.Select(test => new PrescribedLabTestItemDto
                    {
                        LabTestId = test.LabTestId,
                        TestName = test.TestName,
                        Cost = test.Cost 
                    }).ToList()
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while prescribing lab tests: " + ex.Message);
            }
        }
        public async Task<PrescriptionResponseDto> CreatePrescriptionWithDetailsAsync(CreatePrescriptionDto createPrescriptionDto,string doctorId)
        {
            using var transaction = await _doctorDb.Database.BeginTransactionAsync();
            try
            {
                if (createPrescriptionDto == null || createPrescriptionDto.Medicines == null)
                {
                    throw new ArgumentException("Invalid prescription data provided.");
                }
                //bool isAppointmentIdExists = await _doctorDb.Appointment_tbl
                //              .AnyAsync(a => a.AppointmentId == createPrescriptionDto.AppointmentId);
                //if (isAppointmentIdExists == false)
                //{
                //    throw new ArgumentException($"Appointment ID {createPrescriptionDto.AppointmentId} does not exist.");
                //}
                var appointmentDetails = await _doctorDb.Appointment_tbl
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .FirstOrDefaultAsync(a => a.AppointmentId == createPrescriptionDto.AppointmentId);

                if (!appointmentDetails.IsPaid)
                {
                    throw new InvalidOperationException("The appointment must be paid before creating a prescription.");
                }

                if (appointmentDetails == null)
                {
                    throw new ArgumentException($"Appointment ID {createPrescriptionDto.AppointmentId} does not exist.");
                }
                if (appointmentDetails.DoctorId != doctorId)
                {
                    throw new UnauthorizedAccessException("You are not authorized to create a prescription for this appointment.");
                }
                var prescription = new Prescription
                {
                    AppointmentId = createPrescriptionDto.AppointmentId,
                    Note = createPrescriptionDto.Note,
                    PrescribeDate = DateTime.Now
                };
                await _doctorDb.Prescription_tbl.AddAsync(prescription);
                await _doctorDb.SaveChangesAsync();

                appointmentDetails.Status = AppointmentStatus.Completed; // Assuming you want to mark it as completed after prescription
                appointmentDetails.UpdatedAt = DateTime.Now;
                await _doctorDb.SaveChangesAsync();

                var prescriptionDetails = createPrescriptionDto.Medicines.Select(m => new PrescriptionDetail
                {
                    PrescriptionId = prescription.PrescriptionId,
                    MedicineName = m.MedicineName,
                    Dosage = m.Dosage,
                    Frequency = m.Frequency,
                    Duration = m.Duration,
                    Instructions = m.Instructions
                }).ToList();
                await _doctorDb.PrescriptionDetails_tbl.AddRangeAsync(prescriptionDetails);
                await _doctorDb.SaveChangesAsync();
                await transaction.CommitAsync();
                var prescriptionResponse = _mapper.Map<PrescriptionResponseDto>(prescription);
                prescriptionResponse.DoctorName = appointmentDetails.Doctor.Name;
                prescriptionResponse.PatientName = appointmentDetails.Patient.Name;
                return prescriptionResponse;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
    }
}
