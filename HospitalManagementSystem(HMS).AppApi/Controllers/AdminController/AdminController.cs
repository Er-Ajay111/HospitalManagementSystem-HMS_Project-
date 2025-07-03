using HospitalManagementSystem_HMS_.BL.AppRepo.Admin.IServices;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.AdminDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem_HMS_.AppApi.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService service)
        {
            _adminService = service;
        }
        [HttpGet]
        [Route("GetAllDoctorDetails")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllDoctorDetails()
        {
            var result = await _adminService.GetAllDoctorDetails();
            if (result == null || !result.Any())
            {
                return NotFound("No doctor details found.");
            }
            return Ok(result);
        }
        [HttpPost("AddNewLabTest")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddNewLabTest([FromBody] LabTestsDto labTestDto)
        {
            try
            {
                var result = await _adminService.AddNewLabTest(labTestDto);

                return Created(string.Empty, new
                {
                    message = result,
                    testName = labTestDto.TestName,
                    cost = labTestDto.Cost
                });
            }
            catch (InvalidOperationException ex)
            {
                // Handle duplicate or validation issue
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle server error
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error adding lab test: {ex.Message}" });
            }
        }
        [HttpGet("GetAllLabTests")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllLabTests()
        {
            try
            {
                var labTests = await _adminService.GetAllLabTests();
                if (labTests == null || !labTests.Any())
                {
                    return NotFound("No lab tests found.");
                }
                return Ok(labTests);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error retrieving lab tests: {ex.Message}" });
            }
        }
        [HttpGet("GetAllPatientDetails")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPatientDetails()
        {
            try
            {
                var result = await _adminService.GetAllPatientDetails();
                if (result == null || !result.Any())
                {
                    return NotFound("No patient details found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = $"Error retrieving patient details: {ex.Message}" });
            }
        }
        [HttpGet("GetAllPendingAppointments")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPendingAppointments()
        {
            try
            {
                var appointments = await _adminService.GetPendingAppointmentsAsync();
                if (appointments == null || !appointments.Any())
                {
                    return NotFound("No pending appointments found.");
                }
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = $"Error retrieving pending appointments: {ex.Message}" });
            }
        }
        [HttpPost("ConfirmAppointment/{appointmentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmAppointment(Guid appointmentId)
        {
            try
            {
                var result = await _adminService.ConfirmAppointmentByAdminAsync(appointmentId);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = $"Error confirming appointment: {ex.Message}" });
            }
        }
        [HttpPost("CancelAppointment/{appointmentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CancelAppointment(Guid appointmentId, [FromQuery] string? cancelReason = null)
        {
            try
            {
                var result = await _adminService.CancelAppointmentByAdminAsync(appointmentId, cancelReason);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = $"Error cancelling appointment: {ex.Message}" });
            }
        }
    }
}
