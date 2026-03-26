using ClinicManagementSystem.Models;
using ClinicManagementSystem.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers
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

        [HttpPost("GetTodayPendingPatients/{id}")]
        public async Task<IActionResult> GetTodayPendingPatients(int id)
        {
           
            var result = await _doctorService.GetPendingPatientsService(id);

            return Ok(result);
        }

        [HttpPost("AddConsultation")]
        public  async Task<IActionResult> AddConsultation(Consultation consultation)
        {
            var result = await _doctorService.AddConsultationService(consultation);
            return Ok(result);
        }

        [HttpGet("profile/{doctorId}")]
        public async Task<IActionResult> GetDoctorProfile(int doctorId)
        {
            var result = await _doctorService.GetDoctorProfileService(doctorId);
            if (result == null) return NotFound("Doctor not found");
            return Ok(result);
        }

        [HttpGet("patientHistory/{patientId}/{doctorId}")]
        public async Task<IActionResult> GetPatientHistory(int patientId, int doctorId)
        {
            var result = await _doctorService.GetPatientHistoryService(patientId, doctorId);
            if (result == null) return NotFound("Patient history not found");
            return Ok(result);
        }

        [HttpPost("AddMedicine/{consultationId}")]
        public async Task<IActionResult> AddMedicine(int consultationId, [FromBody] PatientMedicine medicine)
        {
            if (medicine == null) return BadRequest("No medicine provided.");
            var result = await _doctorService.AddPrescriptionService(consultationId, medicine);
            return Ok(result);
        }

        [HttpPost("AddLabTest/{consultationId}")]
        public async Task<IActionResult> AddLabTest(int consultationId, [FromBody] PatientLabTest labTest)
        {
            if (labTest == null) return BadRequest("No lab test provided.");
            var result = await _doctorService.AddLabTestService(consultationId, labTest);
            return Ok(result);
        }

        [HttpGet("consultationDetails/{consultationId}")]
        public async Task<IActionResult> GetConsultationDetails(int consultationId)
        {
            var result = await _doctorService.GetConsultationDetailsService(consultationId);
            if (result == null) return NotFound("Consultation not found.");
            return Ok(result);
        }

        [HttpPut("editPrescription/{patientMedicineId}")]
        public async Task<IActionResult> EditPrescription(int patientMedicineId, [FromBody] PatientMedicine medicine)
        {
            if (medicine == null) return BadRequest();
            var result = await _doctorService.EditPrescriptionService(patientMedicineId, medicine);
            if (result == null) return NotFound("Prescription not found.");
            return Ok(result);
        }

        [HttpDelete("deletePrescription/{patientMedicineId}")]
        public async Task<IActionResult> DeletePrescription(int patientMedicineId)
        {
            var result = await _doctorService.DeletePrescriptionService(patientMedicineId);
            if (!result) return NotFound("Prescription not found.");
            return Ok("Prescription deleted successfully.");
        }

        [HttpPut("editLabTest/{patientLabtestId}")]
        public async Task<IActionResult> EditLabTest(int patientLabtestId, [FromBody] PatientLabTest labTest)
        {
            if (labTest == null) return BadRequest();
            var result = await _doctorService.EditLabTestService(patientLabtestId, labTest);
            if (result == null) return NotFound("Lab test not found.");
            return Ok(result);
        }

        [HttpDelete("deleteLabTest/{patientLabtestId}")]
        public async Task<IActionResult> DeleteLabTest(int patientLabtestId)
        {
            var result = await _doctorService.DeleteLabTestService(patientLabtestId);
            if (!result) return NotFound("Lab test not found.");
            return Ok("Lab test deleted successfully.");
        }
    }
}
