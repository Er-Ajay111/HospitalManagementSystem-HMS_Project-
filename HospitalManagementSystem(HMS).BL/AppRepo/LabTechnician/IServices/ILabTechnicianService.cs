using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.LabTechnicianDto;

namespace HospitalManagementSystem_HMS_.BL.AppRepo.LabTechnician.IServices
{
    public interface ILabTechnicianService
    {
        Task<List<PatientLabTestGroupDto>> GetReadyLabTestsAsync();
        Task<LabTestStartResponseDto> StartLabTestAsync(Guid labTestId);
        Task<LabTestResultResponseDto> SubmitLabTestResultAsync(Guid labTestId, LabTestResultUploadDto dto);
    }
}
