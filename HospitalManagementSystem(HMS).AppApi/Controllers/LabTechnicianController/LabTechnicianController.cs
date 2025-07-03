using HospitalManagementSystem_HMS_.BL.AppRepo.LabTechnician.IServices;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.LabTechnicianDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem_HMS_.AppApi.Controllers.LabTechnicianController
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabTechnicianController : ControllerBase
    {
        private readonly ILabTechnicianService _labTechnicianService;
        public LabTechnicianController(ILabTechnicianService labTechnicianService)
        {
            _labTechnicianService = labTechnicianService;
        }
        [HttpGet("GetAllReadyLabTests")]
        [Authorize(Roles = "Lab Technician")]
        public async Task<IActionResult> GetAllReadyLabTests()
        {
            try
            {
                var readyLabTests = await _labTechnicianService.GetReadyLabTestsAsync();
                return Ok(readyLabTests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("StartLabTest")]
        [Authorize(Roles = "Lab Technician")]
        public async Task<IActionResult> StartLabTest(Guid labTestId)
        {
            try
            {
                var result = await _labTechnicianService.StartLabTestAsync(labTestId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("SubmitLabTestResult/{labTestId}")]
        [Authorize(Roles = "Lab Technician")]
        public async Task<IActionResult> SubmitLabTestResult([FromRoute] Guid labTestId, [FromForm] LabTestResultUploadDto dto)
        {
            try
            {
                var result = await _labTechnicianService.SubmitLabTestResultAsync(labTestId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
