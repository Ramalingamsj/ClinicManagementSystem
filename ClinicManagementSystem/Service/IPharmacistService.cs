using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Service
{
    public interface IPharmacistService
    {
        Task<IEnumerable<object>> GetPendingMedicinesService();
        Task<bool> HasPendingMedicinesService(int consultationId);
        Task<Medicine> AddMedicineService(Medicine m);
        Task<Medicine> UpdateStockService(int id, int stock);
        Task<IEnumerable<Medicine>> GetMedicinesService();
        Task<BillMed> GenerateMedicineBillService(int consultationId);
        Task<IEnumerable<object>> GetAllBillsService();
        Task<IEnumerable<object>> GetIssuedMedicinesHistoryService();
        Task<decimal> GetBillAmountService(int consultationId);
        Task<bool> BillExistsService(int consultationId);
        Task<string> GetPatientNameService(int consultationId);
        Task<IEnumerable<object>> GetTodaysConsultationsService();
        Task<PatientMedicine> IssueMedicineService(int patientMedicineId, int pharmacyUserId);
        Task<(bool Success, string Message)> SendBillSmsService(int consultationId);
    }
}
