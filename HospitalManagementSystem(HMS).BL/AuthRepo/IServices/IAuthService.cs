using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.Dtos.AuthDtos;

namespace HospitalManagementSystem_HMS_.BL.AuthRepo.IServices
{
    public interface IAuthService
    {
        Task<string> AdminRegistration(AdminRegistrationDto adminRegistrationDto);
        Task<string> DoctorRegistration(DoctorRegistrationDto doctorRegistrationDto);
        Task<string> PatientRegistration(PatientRegistrationDto patientRegistrationDto);
        Task<string> NurseRegistration(NurseRegistrationDto nurseRegistrationDto);
        Task<string> ChemistRegistration(ChemistRegistrationDto chemistRegistrationDto);
        Task<string> LabTechnicianRegistration(LabTechnicianRegistrationDto labTechnicianRegistrationDto);
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> AssignRolesToUser(string email, List<string> roleName);
        Task<string> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
