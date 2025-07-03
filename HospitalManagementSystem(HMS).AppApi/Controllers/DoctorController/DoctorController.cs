using System.Security.Claims;
using HospitalManagementSystem_HMS_.BL.AppRepo.Doctor.IServices;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.DoctorDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem_HMS_.AppApi.Controllers.DoctorController
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet("GetAllConfirmedAndPaidAppointments")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetConfirmedAndPaidAppointments()
        {
            try
            {
                var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(doctorId))
                {
                    return Unauthorized("Doctor not identified.");
                }

                var appointments = await _doctorService.GetDoctorAppointments(doctorId);

                if (appointments == null || !appointments.Any())
                {
                    return NotFound("No confirmed and paid appointments found.");
                }

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles ="Doctor")]
        [HttpGet("GetAllLabTests")]
        public async Task<IActionResult> GetAllLabTests()
        {
            try
            {
                var labTests = await _doctorService.GetAvailableLabTestsAsync();
                if (labTests == null || !labTests.Any())
                {
                    return NotFound("No lab tests available.");
                }
                return Ok(labTests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost("PrescribeLabTestsForPatient")]
        public async Task<IActionResult> PrescribeLabTestsForPatient([FromBody] PrescribeLabTestDto prescribeLabTestDto)
        {
            if (prescribeLabTestDto == null)
            {
                return BadRequest("Lab test details cannot be null.");
            }
            try
            {
                var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _doctorService.PrescribeLabTestsAsync(prescribeLabTestDto, doctorId);
                return Created("", new
                {
                    message = "Lab tests prescribed successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while prescribing lab tests: {ex.Message}");
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost("WritePrescriptionForPatient")]
        public async Task<IActionResult> WritePrescriptionForPatient([FromBody] CreatePrescriptionDto createPrescriptionDto)
        {
            if (createPrescriptionDto == null)
            {
                return BadRequest("Prescription details cannot be null.");
            }

            try
            {
                var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _doctorService.CreatePrescriptionWithDetailsAsync(createPrescriptionDto,doctorId);

                return Created("", new
                {
                    message = "Prescription created successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while creating the prescription: {ex.Message}");
            }
        }
    }
}
