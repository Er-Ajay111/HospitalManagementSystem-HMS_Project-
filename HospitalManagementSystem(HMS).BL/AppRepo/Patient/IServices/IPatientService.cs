using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.Dtos.AppDtos;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.PatientDtos;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.SearchDto;

namespace HospitalManagementSystem_HMS_.BL.AppRepo.Patient.IServices
{
    public interface IPatientService
    {
        Task<List<DoctorDtoForPatientView>> GetAllDoctorsByNameOrSpecialities(string name= null, string speciality= null);
        Task<string> BookNewAppointment(BookAppointmentDto appointmentDto,string patientId);
        Task<string> MakeAppointmentPayment(string patientId, Guid appointmentId, decimal amount);
        Task<PendingLabTestListDto> GetPendingLabTestsForPaymentAsync(string patientId);
        Task<string> PayForLabTestsAsync(LabTestPaymentRequestDto dto, string patientId);
        Task<PatientLabTestResultsListDto> GetCompletedLabResultsAsync(string patientId);
        Task<FileDto> DownloadLabReportAsync(Guid labTestId, string userId, List<string> roles);
        Task<List<ShowAppointmentDto>> GetPatientAppointmentsAsync(string patientId);
        Task<PrescriptionDto> GetPrescriptionByIdAsync(string loggedInPatientId,Guid prescriptionId);
        Task<List<AppointmentHistoryDetailedDto>> GetAppointmentHistoryDetailedAsync(string patientId);
        Task<byte[]> DownloadPrescriptionPdfAsync(Guid prescriptionId, string patientId);
    }
}
