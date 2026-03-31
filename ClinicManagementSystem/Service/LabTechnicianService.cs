using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Service
{
    public class LabTechnicianService : ILabTechnicianService
    {
        private readonly ILabTechnicianRepository _repository;
        private readonly ISmsService _smsService;

        public LabTechnicianService(ILabTechnicianRepository repository, ISmsService smsService)
        {
            _repository = repository;
            _smsService = smsService;
        }

        public async Task<IEnumerable<object>> GetPendingTestsService()
        {
            return await _repository.GetPendingTestsRepository();
        }

        public async Task<PatientLabTest> UpdateResultService(int id, string result, int userId)
        {
            return await _repository.UpdateResultRepository(id, result, userId);
        }

        public async Task<BillLab> GenerateLabBillService(int consultationId)
        {
            return await _repository.GenerateLabBillRepository(consultationId);
        }

        public async Task<int> GetConsultationIdFromLabTestService(int id)
        {
            return await _repository.GetConsultationIdFromLabTestRepository(id);
        }

        public async Task<IEnumerable<object>> GetBillsService()
        {
            return await _repository.GetBillsRepository();
        }

        public async Task<decimal> GetBillAmountService(int consultationId)
        {
            return await _repository.GetBillAmountRepository(consultationId);
        }

        public async Task<bool> BillExistsService(int consultationId)
        {
            return await _repository.BillExistsRepository(consultationId);
        }

        public async Task<(bool Success, string Message)> SendSMSService(int consultationId, string phoneNumber, string message)
        {
            // Now using the dedicated ISmsService which supports both Real transmission and Simulation mode
            return await _smsService.SendSmsAsync(phoneNumber, message);
        }

        public async Task<IEnumerable<LabTest>> GetLabTestsService()
        {
            return await _repository.GetLabTestsRepository();
        }

        public async Task<LabTest> AddLabTestService(LabTest test)
        {
            return await _repository.AddLabTestRepository(test);
        }

        public async Task<LabTest?> UpdateLabPriceService(int id, decimal price)
        {
            return await _repository.UpdateLabPriceRepository(id, price);
        }
    }
}
