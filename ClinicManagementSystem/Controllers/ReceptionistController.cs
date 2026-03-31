using ClinicManagementSystem.Models;
using ClinicManagementSystem.ViewModels;
using ClinicManagementSystem.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceptionistController : ControllerBase
    {
        private readonly IReceptionistService _service;

        public ReceptionistController(IReceptionistService service)
        {
            _service = service;
        }

        #region Patient Management
        [HttpGet("patients")]
        public IActionResult GetAllPatients()
        {
            var patients = _service.GetAllPatients();
            return Ok(patients);
        }

        [HttpGet("patients/search")]
        public IActionResult SearchPatients([FromQuery] string searchValue)
        {
            var patients = _service.SearchPatients(searchValue);
            return Ok(patients);
        }

        [HttpGet("patients/{id}")]
        public IActionResult GetPatientById(int id)
        {
            var patient = _service.GetPatientById(id);
            if (patient == null) return NotFound("Patient not found");
            return Ok(patient);
        }

        [HttpPost("patients")]
        public IActionResult AddPatient([FromBody] Patient patient)
        {
            if (patient == null) return BadRequest("Invalid patient data");
            _service.InsertPatient(patient);
            return Ok(new { success = true, message = "Patient added successfully" });
        }

        [HttpPut("patients/{id}")]
        public IActionResult UpdatePatient(int id, [FromBody] Patient patient)
        {
            if (patient == null || id != patient.PatientId) return BadRequest("Mismatched patient ID");
            _service.UpdatePatient(patient);
            return Ok(new { success = true, message = "Patient updated successfully" });
        }
        #endregion

        #region Appointment Booking
        [HttpGet("doctors")]
        public IActionResult GetAllDoctors()
        {
            var doctors = _service.GetAllDoctors();
            return Ok(doctors);
        }

        [HttpGet("slots")]
        public IActionResult GetAvailableSlots([FromQuery] int doctorId, [FromQuery] DateTime date)
        {
            var slots = _service.GetAvailableSlots(doctorId, date);
            return Ok(slots);
        }

        [HttpPost("book")]
        public IActionResult BookAppointment([FromBody] AppointmentBookingViewModel model)
        {
            if (model == null) return BadRequest("Invalid booking data");
            var result = _service.BookAppointment(model);
            if (result.success)
                return Ok(new { success = true, appointmentId = result.appointmentId, message = result.message });
            
            return BadRequest(new { success = false, message = result.message });
        }
        #endregion

        #region Appointment History
        [HttpGet("history/{receptionistId}")]
        public IActionResult GetAppointmentsHistory(int receptionistId)
        {
            var history = _service.GetAppointmentsByReceptionist(receptionistId);
            return Ok(history);
        }
        #endregion

        #region Billing
        [HttpGet("bill/exists/{appointmentId}")]
        public IActionResult CheckBillExists(int appointmentId)
        {
            bool exists = _service.BillExists(appointmentId);
            return Ok(new { exists });
        }

        [HttpGet("bill/details/{appointmentId}")]
        public IActionResult GetBillDetails(int appointmentId)
        {
            var details = _service.GetBillDetails(appointmentId);
            return Ok(details);
        }

        [HttpPost("bill")]
        public IActionResult GenerateBill([FromBody] dynamic billData)
        {
            try
            {
                int appointmentId = (int)billData.appointmentId;
                decimal totalAmount = (decimal)billData.totalAmount;
                int statusId = (int)billData.statusId;

                _service.InsertBill(appointmentId, totalAmount, statusId);
                return Ok(new { success = true, message = "Consultation bill generated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion
    }
}
