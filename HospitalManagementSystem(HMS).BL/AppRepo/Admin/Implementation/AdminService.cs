using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HospitalManagementSystem_HMS_.BL.AppRepo.Admin.IServices;
using HospitalManagementSystem_HMS_.BL.AuthRepo.IServices;
using HospitalManagementSystem_HMS_.DB.Data;
using HospitalManagementSystem_HMS_.DB.Model.AppModel.AppointmentModel;
using HospitalManagementSystem_HMS_.DB.Model.AuthModel;
using HospitalManagementSystem_HMS_.Dtos.AppDtos;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.AdminDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem_HMS_.BL.AppRepo.Admin.Implementation
{
    public class AdminService : IAdminService
    {
        private readonly HMSDBContext _adminDb;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        public AdminService(HMSDBContext doctorDb, IMapper mapper, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _adminDb = doctorDb;
            _mapper = mapper;
            _roleManager = roleManager;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<string> AddNewLabTest(LabTestsDto labTestDto)
        {
            try
            {
                if (labTestDto == null || string.IsNullOrWhiteSpace(labTestDto.TestName))
                {
                    throw new ArgumentNullException(nameof(labTestDto), "Lab test details cannot be null or empty.");
                }

                // Check for duplicate test name (case-insensitive)
                var isDuplicate = await _adminDb.LabTests_tbl
                    .AnyAsync(t => t.TestName.ToLower() == labTestDto.TestName.ToLower());

                if (isDuplicate)
                {
                    throw new InvalidOperationException("A lab test with the same name already exists.");
                }

                // Create entity and map
                var newTest = _mapper.Map<LabTests>(labTestDto);
                newTest.UpdatedAt = null;
                newTest.CreatedAt = DateTime.Now; // Set the creation date to now
                newTest.IsActive = true;

                await _adminDb.LabTests_tbl.AddAsync(newTest);
                await _adminDb.SaveChangesAsync();

                return $"Lab test added successfully.";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding lab test: {ex.Message}");
            }
        }


        public async Task<List<DoctorDetailsDto>> GetAllDoctorDetails()
        {
            try
            {
                //var allDoctorDetails = await _doctorDb.Doctor_tbl.ToListAsync();
                var allDoctorDetails = await _adminDb.Doctor_tbl.Include(x => x.AppUser).ToListAsync();
                //var doctorDtos = allDoctorDetails.Select(d => new DoctorDetailsDto
                //{
                //}).ToList();

                //List<DoctorDetailsDto> allDoctors = new List<DoctorDetailsDto>();
                //foreach (var doctor in allDoctorDetails)
                //{
                //    DoctorDetailsDto doctorDto = new DoctorDetailsDto
                //    {
                //        Name = doctor.AppUser.Name,
                //        AadharNumber = doctor.AppUser.AadharNumber,
                //        Age = doctor.AppUser.Age,
                //        AlternatePhoneNo = doctor.AlternatePhoneNo,
                //        City = doctor.AppUser.City,
                //        DateOfBirth = doctor.AppUser.DateOfBirth,
                //        LicenseNumber = doctor.LicenseNumber,
                //        LicenseExpiryDate = doctor.LicenseExpiryDate.Value,
                //        PostalCode = doctor.AppUser.PostalCode,
                //        Specialization = doctor.Specialization,
                //        State = doctor.AppUser.State,
                //        AvailableDays = doctor.AvailableDays.Split(',').ToList(),
                //        ExperienceYears = doctor.ExperienceYears,
                //        Qualifications = doctor.Qualifications.Split(',').ToList(),
                //        Email = doctor.AppUser.Email,
                //        EndTime = doctor.EndTime,
                //        StartTime = doctor.StartTime,
                //        Gender =doctor.AppUser.Gender,
                //        PhoneNumber = doctor.AppUser.PhoneNumber,
                //        RoomNumber = doctor.RoomNumber
                //    };
                //    allDoctors.Add(doctorDto);
                //}
                var allDoctors = _mapper.Map<List<DoctorDetailsDto>>(allDoctorDetails);
                return allDoctors;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<LabTestsDto>> GetAllLabTests()
        {
            try
            {
                var allLabTests = await _adminDb.LabTests_tbl.ToListAsync();
                if (allLabTests == null || !allLabTests.Any())
                {
                    throw new Exception("No lab tests found.");
                }
                var labTestDtos = _mapper.Map<List<LabTestsDto>>(allLabTests);
                return labTestDtos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //public async Task<List<PatientDetailsDto>> GetAllPatientDetails()
        //{
        //    try
        //    {
        //        var allPatientDetails = await _adminDb.Patient_tbl.Include(x => x.AppUser).ToListAsync();
        //        if (allPatientDetails == null || !allPatientDetails.Any())
        //        {
        //            throw new Exception("No patient details found.");
        //        }
        //        var patientDtos = _mapper.Map<List<PatientDetailsDto>>(allPatientDetails);
        //        return patientDtos;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        public async Task<List<PatientDetailsDto>> GetAllPatientDetails()
        {
            try
            {
                var users = await _userManager.Users.Include(x => x.PatientDetails).ToListAsync();

                // All users who are in "Patient" role
                var patientUsers = new List<ApplicationUser>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Patient"))
                    {
                        patientUsers.Add(user);
                    }
                }
                // Map to DTOs
                var allPatientDetailsDto = new List<PatientDetailsDto>();
                foreach (var patient in patientUsers)
                {
                    var dto = new PatientDetailsDto
                    {
                        Name = patient.Name,
                        AadharNumber = patient.AadharNumber,
                        Age = patient.Age,
                        City = patient.City,
                        DateOfBirth = patient.DateOfBirth,
                        Email = patient.Email,
                        Gender = patient.Gender,
                        State = patient.State,
                        PostalCode = patient.PostalCode,
                        PhoneNumber = patient.PhoneNumber,
                        BloodGroup = patient.PatientDetails?.BloodGroup ?? "Unknown",
                        MedicalHistory = patient.PatientDetails?.MedicalHistory ?? "N/A",
                        ChronicDiseases = patient.PatientDetails?.ChronicDiseases ?? "N/A",
                        RegistrationDate = patient.PatientDetails?.RegistrationDate ?? DateTime.MinValue
                    };
                    allPatientDetailsDto.Add(dto);
                }
                return allPatientDetailsDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching patients: " + ex.Message);
            }
        }
        public async Task<List<AppointmentResponseDto>> GetPendingAppointmentsAsync()
        {
            try
            {
                var pendingAppointments = await _adminDb.Appointment_tbl
                    .Where(a => a.Status == AppointmentStatus.Pending)
                    .Include(a => a.Doctor)
                    .Include(a => a.Patient)
                    .Select(a => new AppointmentResponseDto
                    {
                        AppointmentId = a.AppointmentId,
                        DoctorId = a.DoctorId,
                        DoctorName = a.Doctor.Name,
                        PatientId = a.PatientId,
                        PatientName = a.Patient.Name,
                        PatientGender = a.Patient.Gender,
                        PatientAge = a.Patient.Age,
                        AppointmentDate = a.AppointmentDate,
                        Remark = a.Remark,
                        Status = a.Status.ToString(),
                        BookingDate = a.BookingDate
                    })
                    .ToListAsync();

                return pendingAppointments;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching pending appointments: " + ex.Message);
            }
        }
        public async Task<string> ConfirmAppointmentByAdminAsync(Guid appointmentId)
        {
            using var transaction = await _adminDb.Database.BeginTransactionAsync();
            try
            {
                var appointment = await _adminDb.Appointment_tbl.Include(d=>d.Doctor)
                    .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

                if (appointment == null)
                    throw new Exception("Appointment not found.");

                if (appointment.Status != AppointmentStatus.Pending)
                    throw new Exception("Only pending appointments can be confirmed.");

                var consultantFee = await _adminDb.Doctor_tbl
                    .Where(d => d.UserId == appointment.DoctorId)
                    .Select(d => d.ConsultantFee)
                    .FirstOrDefaultAsync();

                appointment.Status = AppointmentStatus.Confirmed;
                appointment.UpdatedAt = DateTime.Now;

                _adminDb.Appointment_tbl.Update(appointment);
                await _adminDb.SaveChangesAsync();

                // Send email to patient
                var patient = await _userManager.FindByIdAsync(appointment.PatientId);
                if (patient == null)
                {
                    throw new Exception("Patient not found for the appointment.");
                }
                string body = $@"
                <p>Dear <strong>{patient.Name}</strong>,</p>

                <p>Your appointment request has been <strong style='color:green;'>Confirmed</strong> successfully.</p>

                <p><strong>Appointment Details:</strong></p>
                   <ul>
                       <li><strong>Doctor ID:</strong> {appointment.DoctorId}</li>
                       <li><strong>Doctor Name:</strong> {appointment.Doctor.Name}</li>
                       <li><strong>Consultant Fee<strong> {consultantFee}</li>
                   </ul>

                <p style='margin-top:10px;'>To proceed your appointment, please complete the payment as soon as possible.The consultation with your doctor will only be valid after the payment is successfully made.</p>

                <p><strong>Note:</strong> You will receive a confirmation email and access to your appointment details once your payment is verified/fully paid.</p>

                 <br />
                <p>Thank you for choosing <strong>Hospital Management System</strong>.</p>
                <p>We wish you good health!</p>
                ";

                await _emailService.SendEmailAsync(patient.Email, "Appointment Confirmed", body);

                await transaction.CommitAsync();

                return "Appointment confirmed successfully.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error while confirming appointment: {ex.Message}");
            }
        }
        public async Task<string> CancelAppointmentByAdminAsync(Guid appointmentId, string? cancelReason = null)
        {
            using var transaction = await _adminDb.Database.BeginTransactionAsync();
            try
            {
                var appointment = await _adminDb.Appointment_tbl.FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

                if (appointment == null)
                    throw new Exception("Appointment not found.");

                if (appointment.Status != AppointmentStatus.Pending)
                    throw new Exception("Only pending appointments can be cancelled.");

                appointment.Status = AppointmentStatus.Cancelled;
                appointment.UpdatedAt = DateTime.Now;

                _adminDb.Appointment_tbl.Update(appointment);
                await _adminDb.SaveChangesAsync();

                // Send email to patient
                var patient = await _userManager.FindByIdAsync(appointment.PatientId);
                if (patient == null)
                {
                    throw new ArgumentNullException($"Patient with ID : {appointment.PatientId} not found for the appointment.");
                }
                string reasonText = !string.IsNullOrEmpty(cancelReason) ? $"<br>Reason : {cancelReason}" : "";
                string body = $"Dear {patient.Name},<br>Your appointment on {appointment.AppointmentDate} has been <strong>Cancelled<strong> {reasonText}";
                await _emailService.SendEmailAsync(patient.Email, "Appointment Cancelled", body);
                await transaction.CommitAsync();

                return "Appointment cancelled successfully.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error while cancelling appointment: {ex.Message}");
            }
        }
    }
}
