using ClinicManagementSystem.Models;
using ClinicManagementSystem.Service;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacistController : ControllerBase
    {
        private readonly IPharmacistService _service;

        public PharmacistController(IPharmacistService service)
        {
            _service = service;
        }

        [HttpGet("pendingMedicines")]
        public async Task<IActionResult> GetPendingMedicines()
        {
            var result = await _service.GetPendingMedicinesService();
            return Ok(result);
        }

        [HttpGet("hasPending/{consultationId}")]
        public async Task<IActionResult> HasPendingMedicines(int consultationId)
        {
            var result = await _service.HasPendingMedicinesService(consultationId);
            return Ok(result);
        }

        [HttpPost("addMedicine")]
        public async Task<IActionResult> AddMedicine([FromBody] Medicine m)
        {
            if (m == null) return BadRequest(new { message = "Invalid medicine data." });
            try
            {
                var result = await _service.AddMedicineService(m);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An internal server error occurred." });
            }
        }

        [HttpPut("updateStock/{id}/{stock}")]
        public async Task<IActionResult> UpdateStock(int id, int stock)
        {
            var result = await _service.UpdateStockService(id, stock);
            if (result == null) return NotFound("Medicine not found");
            return Ok(result);
        }

        [HttpGet("medicines")]
        public async Task<IActionResult> GetMedicines()
        {
            var result = await _service.GetMedicinesService();
            return Ok(result);
        }

        [HttpPost("generateBill/{consultationId}")]
        public async Task<IActionResult> GenerateMedicineBill(int consultationId)
        {
            var result = await _service.GenerateMedicineBillService(consultationId);
            return Ok(result);
        }

        [HttpGet("bills")]
        public async Task<IActionResult> GetAllBills()
        {
            var result = await _service.GetAllBillsService();
            return Ok(result);
        }

        [HttpGet("issuedHistory")]
        public async Task<IActionResult> GetIssuedMedicinesHistory()
        {
            var result = await _service.GetIssuedMedicinesHistoryService();
            return Ok(result);
        }

        [HttpGet("billAmount/{consultationId}")]
        public async Task<IActionResult> GetBillAmount(int consultationId)
        {
            var result = await _service.GetBillAmountService(consultationId);
            return Ok(result);
        }

        [HttpGet("billExists/{consultationId}")]
        public async Task<IActionResult> BillExists(int consultationId)
        {
            var result = await _service.BillExistsService(consultationId);
            return Ok(result);
        }

        [HttpGet("patientName/{consultationId}")]
        public async Task<IActionResult> GetPatientName(int consultationId)
        {
            var result = await _service.GetPatientNameService(consultationId);
            return Ok(new { patientName = result });
        }

        [HttpGet("todaysConsultations")]
        public async Task<IActionResult> GetTodaysConsultations()
        {
            var result = await _service.GetTodaysConsultationsService();
            return Ok(result);
        }

        [HttpPut("issueMedicine/{patientMedicineId}/{pharmacyUserId}")]
        public async Task<IActionResult> IssueMedicine(int patientMedicineId, int pharmacyUserId)
        {
            try
            {
                var result = await _service.IssueMedicineService(patientMedicineId, pharmacyUserId);
                if (result == null) return NotFound("Prescription not found");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("sendBillSms/{consultationId}")]
        public async Task<IActionResult> SendBillSms(int consultationId)
        {
            var result = await _service.SendBillSmsService(consultationId);
            if (result.Success) return Ok(new { message = result.Message });
            return BadRequest(new { message = result.Message });
        }
    }
}
