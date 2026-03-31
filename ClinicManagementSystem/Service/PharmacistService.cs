using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repository;

namespace ClinicManagementSystem.Service
{
    public class PharmacistService : IPharmacistService
    {
        private readonly IPharmacistRepository _repository;
        private readonly ISmsService _smsService;

        public PharmacistService(IPharmacistRepository repository, ISmsService smsService)
        {
            _repository = repository;
            _smsService = smsService;
        }

        public async Task<IEnumerable<object>> GetPendingMedicinesService()
        {
            return await _repository.GetPendingMedicinesRepository();
        }

        public async Task<bool> HasPendingMedicinesService(int consultationId)
        {
            return await _repository.HasPendingMedicinesRepository(consultationId);
        }

        public async Task<Medicine> AddMedicineService(Medicine m)
        {
            return await _repository.AddMedicineRepository(m);
        }

        public async Task<Medicine> UpdateStockService(int id, int stock)
        {
            return await _repository.UpdateStockRepository(id, stock);
        }

        public async Task<IEnumerable<Medicine>> GetMedicinesService()
        {
            return await _repository.GetMedicinesRepository();
        }

        public async Task<BillMed> GenerateMedicineBillService(int consultationId)
        {
            return await _repository.GenerateMedicineBillRepository(consultationId);
        }

        public async Task<IEnumerable<object>> GetAllBillsService()
        {
            return await _repository.GetAllBillsRepository();
        }

        public async Task<IEnumerable<object>> GetIssuedMedicinesHistoryService()
        {
            return await _repository.GetIssuedMedicinesHistoryRepository();
        }

        public async Task<decimal> GetBillAmountService(int consultationId)
        {
            return await _repository.GetBillAmountRepository(consultationId);
        }

        public async Task<bool> BillExistsService(int consultationId)
        {
            return await _repository.BillExistsRepository(consultationId);
        }

        public async Task<string> GetPatientNameService(int consultationId)
        {
            return await _repository.GetPatientNameRepository(consultationId);
        }

        public async Task<IEnumerable<object>> GetTodaysConsultationsService()
        {
            return await _repository.GetTodaysConsultationsRepository();
        }

        public async Task<PatientMedicine> IssueMedicineService(int patientMedicineId, int pharmacyUserId)
        {
            return await _repository.IssueMedicineRepository(patientMedicineId, pharmacyUserId);
        }

        public async Task<(bool Success, string Message)> SendBillSmsService(int consultationId)
        {
            var details = await _repository.GetConsultationDetailsRepository(consultationId);
            if (details == null) return (false, "Consultation record not found.");

            // Use reflection or dynamic to get data from anonymous object
            var patientName = details.GetType().GetProperty("PatientName")?.GetValue(details)?.ToString() ?? "Patient";
            var contact = details.GetType().GetProperty("PatientContact")?.GetValue(details)?.ToString() ?? 
                            details.GetType().GetProperty("Contact")?.GetValue(details)?.ToString(); // Fallback to property check
            var totalAmount = (decimal)(details.GetType().GetProperty("TotalAmount")?.GetValue(details) ?? 0m);
            var medicines = details.GetType().GetProperty("Medicines")?.GetValue(details) as List<string>;

            if (string.IsNullOrEmpty(contact))
            {
                return (false, "Patient has no contact number registered.");
            }

            string medList = medicines != null && medicines.Any() ? string.Join(", ", medicines) : "prescribed medications";
            string smsBody = $"Dear {patientName}, your medicine bill for Consult #{consultationId} is ₹{totalAmount:F2}. Meds: {medList}. Thank you, ClinicSys.";

            return await _smsService.SendSmsAsync(contact, smsBody);
        }
    }
}
