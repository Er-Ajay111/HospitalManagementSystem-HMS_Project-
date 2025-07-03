using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.Dtos.AppDtos;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.AdminDtos;

namespace HospitalManagementSystem_HMS_.BL.AppRepo.Admin.IServices
{
    public interface IAdminService
    {
        Task<List<DoctorDetailsDto>> GetAllDoctorDetails();
        Task<List<PatientDetailsDto>> GetAllPatientDetails();
        Task<List<LabTestsDto>> GetAllLabTests();
        Task<string> AddNewLabTest(LabTestsDto labTestDto);
        Task<List<AppointmentResponseDto>> GetPendingAppointmentsAsync();
        Task<string> ConfirmAppointmentByAdminAsync(Guid appointmentId);
        Task<string> CancelAppointmentByAdminAsync(Guid appointmentId, string? cancelReason = null);
    }
}
