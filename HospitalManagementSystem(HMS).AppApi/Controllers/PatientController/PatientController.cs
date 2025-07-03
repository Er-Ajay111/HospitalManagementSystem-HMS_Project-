using System.Security.Claims;
using HospitalManagementSystem_HMS_.BL.AppRepo.Patient.Implementation;
using HospitalManagementSystem_HMS_.BL.AppRepo.Patient.IServices;
using HospitalManagementSystem_HMS_.Dtos.AppDtos.PatientDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem_HMS_.AppApi.Controllers.PatientController
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patient;

        public PatientController(IPatientService patient)
        {
            _patient = patient;
        }
        [Authorize(Roles = "Patient")]
        [HttpGet("GetAllDoctorsByNameOrSpecialities")]
        public async Task<IActionResult> GetAllDoctorsBy([FromQuery] string? name, [FromQuery] string? speciality)
        {
            try
            {
                var result = await _patient.GetAllDoctorsByNameOrSpecialities(name, speciality);
                if (result == null || !result.Any())
                {
                    return NotFound("No doctors found with the provided name or specialization.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while fetching doctor details: {ex.Message}");
            }
        }
        [Authorize(Roles = "Patient")]
        [HttpPost("BookNewAppointment")]
        public async Task<IActionResult> BookAppointment(BookAppointmentDto appointmentDto)
        {
            try
            {
                var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(patientId))
                {
                    return Unauthorized("Patient identity could not be verified.");
                }

                var result = await _patient.BookNewAppointment(appointmentDto, patientId);

                if (result == null)
                {
                    return BadRequest("Appointment could not be booked.");
                }

                return Ok(result); // You can return AppointmentResponseDto here
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while booking the appointment: {ex.Message}");
            }
        }

        [HttpPost("MakeAppointmentPayment")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> MakeAppointmentPayment([FromQuery] Guid appointmentId, [FromQuery] decimal amount)
        {
            try
            {
                var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(patientId))
                {
                    return Unauthorized("Patient identity could not be verified.");
                }
                var result = await _patient.MakeAppointmentPayment( patientId, appointmentId, amount);
                if (string.IsNullOrEmpty(result))
                {
                    return BadRequest("Payment could not be processed.");
                }
                return Ok(new { message = "Payment successful", transactionId = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing the payment: {ex.Message}");
            }
        }
        [HttpGet("GetPendingLabTestsForPayment")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetPendingLabTestsForPayment()
        {
            try
            {
                var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(patientId))
                {
                    return Unauthorized("Patient identity could not be verified.");
                }
                var pendingLabTests = await _patient.GetPendingLabTestsForPaymentAsync(patientId);
                if (pendingLabTests == null || !pendingLabTests.Tests.Any())
                {
                    return NotFound("No pending lab tests found for this patient.");
                }
                return Ok(pendingLabTests);
            }
            catch (Exception ex)
            {
                return StatusCode(400, $"An error occurred while fetching pending lab tests: {ex.Message}");
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpPost("patient/pay-lab-tests")]
        public async Task<IActionResult> PayLabTests([FromBody] LabTestPaymentRequestDto dto)
        {
            try
            {
                var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(patientId))
                    return Unauthorized("Invalid token.");

                var result = await _patient.PayForLabTestsAsync(dto, patientId);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("GetCompletedLabResults")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetCompletedLabResults()
        {
            try
            {
                var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(patientId))
                {
                    return Unauthorized("Patient identity could not be verified.");
                }
                var labResults = await _patient.GetCompletedLabResultsAsync(patientId);
                if (labResults == null)
                {
                    return NotFound("No completed lab results found for this patient.");
                }
                return Ok(labResults);
            }
            catch (Exception ex)
            {
                return StatusCode(400 , $"An error occurred while fetching lab results: {ex.Message}");
            }
        }

        [Authorize(Roles = "Doctor,Patient")]
        [HttpGet("download-lab-report/{labTestId}")]
        public async Task<IActionResult> DownloadLabReport(Guid labTestId)
        {
            try
            {
                // 🔐 Get user info from token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

                // 📥 Call service
                var fileDto = await _patient.DownloadLabReportAsync(labTestId, userId, roles);

                // 📄 Return file
                return File(fileDto.FileBytes, fileDto.ContentType, fileDto.FileName);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new { message = "Something went wrong: " + ex.Message });
            }
        }

        [HttpGet("GetPatientAppointments")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetPatientAppointments()
        {
            try
            {
                var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(patientId))
                {
                    return Unauthorized("Patient identity could not be verified.");
                }
                var appointments = await _patient.GetPatientAppointmentsAsync(patientId);
                if (appointments == null || !appointments.Any())
                {
                    return NotFound("No appointments found for this patient.");
                }
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(400, $"An error occurred while fetching appointments: {ex.Message}");
            }
        }

        [HttpGet("GetPrescriptionForPatientById")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetPrescriptionForPatientById(Guid prescriptionId)
        {
            try
            {
                var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(patientId))
                {
                    return Unauthorized("Patient identity could not be verified.");
                }
                var prescriptions = await _patient.GetPrescriptionByIdAsync(patientId,prescriptionId);
                if (prescriptions == null)
                {
                    return NotFound("No prescriptions found for this patient.");
                }
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(400, $"An error occurred while fetching prescriptions: {ex.Message}");
            }
        }

        [HttpGet("GetAppointmentHistoryDetailed")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetAppointmentHistoryDetailed()
        {
            try
            {
                var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(patientId))
                {
                    return Unauthorized("Patient identity could not be verified.");
                }
                var appointmentHistory = await _patient.GetAppointmentHistoryDetailedAsync(patientId);
                if (appointmentHistory == null || !appointmentHistory.Any())
                {
                    return NotFound("No appointment history found for this patient.");
                }
                return Ok(appointmentHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(400, $"An error occurred while fetching appointment history: {ex.Message}");
            }
        }
        [HttpGet("DownloadPrescriptionPdf")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> DownloadPrescriptionPdf(Guid prescriptionId)
        {
            try
            {
                var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(patientId))
                {
                    return Unauthorized("Patient identity could not be verified.");
                }
                var pdfBytes = await _patient.DownloadPrescriptionPdfAsync(prescriptionId, patientId);
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    return NotFound("No prescription found for this patient.");
                }
                return File(pdfBytes, "application/pdf", $"Prescription_{prescriptionId}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(400, $"An error occurred while downloading the prescription PDF: {ex.Message}");
            }
        }
    }
}
