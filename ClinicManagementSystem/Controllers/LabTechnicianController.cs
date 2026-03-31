using ClinicManagementSystem.Models;
using ClinicManagementSystem.Service;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabTechnicianController : ControllerBase
    {
        private readonly ILabTechnicianService _service;

        public LabTechnicianController(ILabTechnicianService service)
        {
            _service = service;
        }

        [HttpGet("pendingTests")]
        public async Task<IActionResult> GetPendingTests()
        {
            var result = await _service.GetPendingTestsService();
            return Ok(result);
        }

        public class ResultDto
        {
            public string Result { get; set; } = string.Empty;
        }

        [HttpPut("updateResult/{id}/{userId}")]
        public async Task<IActionResult> UpdateResult(int id, int userId, [FromBody] ResultDto dto)
        {
            try
            {
                var result = await _service.UpdateResultService(id, dto.Result, userId);
                if (result == null) return NotFound("Lab Test not found");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("generateBill/{consultationId}")]
        public async Task<IActionResult> GenerateLabBill(int consultationId)
        {
            var result = await _service.GenerateLabBillService(consultationId);
            return Ok(result);
        }

        [HttpGet("consultation/{id}")]
        public async Task<IActionResult> GetConsultationIdFromLabTest(int id)
        {
            var result = await _service.GetConsultationIdFromLabTestService(id);
            return Ok(result);
        }

        [HttpGet("bills")]
        public async Task<IActionResult> GetBills()
        {
            var result = await _service.GetBillsService();
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

        public class SMSRequest
        {
            public int ConsultationId { get; set; }
            public string PhoneNumber { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
        }

        [HttpPost("sendSMS")]
        public async Task<IActionResult> SendSMS([FromBody] SMSRequest request)
        {
            Console.WriteLine($"[API INCOMING] SMS Request for Consultation {request.ConsultationId} to {request.PhoneNumber}");
            var result = await _service.SendSMSService(request.ConsultationId, request.PhoneNumber, request.Message);
            return Ok(new { success = result.Success, message = result.Message });
        }

        // Lab Test Management
        [HttpGet("labTests")]
        public async Task<IActionResult> GetLabTests()
        {
            var result = await _service.GetLabTestsService();
            return Ok(result);
        }

        [HttpPost("addLabTest")]
        public async Task<IActionResult> AddLabTest([FromBody] LabTest test)
        {
            try
            {
                var result = await _service.AddLabTestService(test);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message); // Duplicate entry
            }
        }

        [HttpPut("updatePrice/{id}/{price}")]
        public async Task<IActionResult> UpdateLabPrice(int id, decimal price)
        {
            var result = await _service.UpdateLabPriceService(id, price);
            if (result == null) return NotFound("Lab Test not found");
            return Ok(result);
        }
    }
}
