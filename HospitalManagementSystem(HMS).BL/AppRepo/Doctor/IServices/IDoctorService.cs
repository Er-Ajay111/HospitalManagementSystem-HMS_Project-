using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.Dtos.AppDtos;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.DoctorDtos;

namespace HospitalManagementSystem_HMS_.BL.AppRepo.Doctor.IServices
{
    public interface IDoctorService
    {
        Task<List<DoctorViewAppointmentDto>> GetDoctorAppointments(string doctorId);
        Task<List<LabTestDetailDto>> GetAvailableLabTestsAsync();
        Task<PrescribeLabTestResponseDto> PrescribeLabTestsAsync(PrescribeLabTestDto dto, string doctorId);
        Task<PrescriptionResponseDto> CreatePrescriptionWithDetailsAsync(CreatePrescriptionDto createPrescriptionDto,string doctorId);
    }
}
