using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Repository
{
    public interface IPharmacistRepository
    {
        Task<IEnumerable<object>> GetPendingMedicinesRepository();
        Task<bool> HasPendingMedicinesRepository(int consultationId);
        Task<Medicine> AddMedicineRepository(Medicine m);
        Task<Medicine> UpdateStockRepository(int id, int stock);
        Task<IEnumerable<Medicine>> GetMedicinesRepository();
        Task<BillMed> GenerateMedicineBillRepository(int consultationId);
        Task<IEnumerable<object>> GetAllBillsRepository();
        Task<IEnumerable<object>> GetIssuedMedicinesHistoryRepository();
        Task<decimal> GetBillAmountRepository(int consultationId);
        Task<bool> BillExistsRepository(int consultationId);
        Task<string> GetPatientNameRepository(int consultationId);
        Task<IEnumerable<object>> GetTodaysConsultationsRepository();
        Task<PatientMedicine> IssueMedicineRepository(int patientMedicineId, int pharmacyUserId);
        Task<object?> GetConsultationDetailsRepository(int consultationId);
    }
}
