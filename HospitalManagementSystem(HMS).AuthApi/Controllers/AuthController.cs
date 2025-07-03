using HospitalManagementSystem_HMS_.BL.AuthRepo.IServices;
using HospitalManagementSystem_HMS_.Dtos.AuthDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem_HMS_.AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }
        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin(AdminRegistrationDto adminRegistrationDto)
        {
            try
            {
                var result = await _service.AdminRegistration(adminRegistrationDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("RegisterDoctor")]
        public async Task<IActionResult> RegisterDoctor(DoctorRegistrationDto doctorRegistrationDto)
        {
            try
            {
                var result = await _service.DoctorRegistration(doctorRegistrationDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("RegisterPatient")]
        public async Task<IActionResult> RegisterPatient(PatientRegistrationDto patientRegistrationDto)
        {
            try
            {
                var result = await _service.PatientRegistration(patientRegistrationDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RegisterNurse")]
        public async Task<IActionResult> RegisterNurse(NurseRegistrationDto nurseRegistrationDto)
        {
            try
            {
                var result = await _service.NurseRegistration(nurseRegistrationDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("RegisterChemist")]
        public async Task<IActionResult> RegisterChemist(ChemistRegistrationDto chemistRegistrationDto)
        {
            try
            {
                var result = await _service.ChemistRegistration(chemistRegistrationDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("RegisterLabTechnician")]
        public async Task<IActionResult> RegisterLabTechnician(LabTechnicianRegistrationDto labTechnicianRegistrationDto)
        {
            try
            {
                var result = await _service.LabTechnicianRegistration(labTechnicianRegistrationDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                var result = await _service.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var token = await _service.ForgotPasswordAsync(forgotPasswordDto);
                return Ok("Password reset link sent to your email.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var result = await _service.ResetPasswordAsync(resetPasswordDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
