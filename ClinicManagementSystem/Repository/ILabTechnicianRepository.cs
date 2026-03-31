using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Repository
{
    public interface ILabTechnicianRepository
    {
        Task<IEnumerable<object>> GetPendingTestsRepository();
        Task<BillLab> GenerateLabBillRepository(int consultationId);
        Task<int> GetConsultationIdFromLabTestRepository(int id);
        Task<IEnumerable<object>> GetBillsRepository();
        Task<PatientLabTest> UpdateResultRepository(int id, string result, int userId);
        Task<decimal> GetBillAmountRepository(int consultationId);
        Task<bool> BillExistsRepository(int consultationId);
        
        // Lab Test Management
        Task<IEnumerable<LabTest>> GetLabTestsRepository();
        Task<LabTest> AddLabTestRepository(LabTest test);
        Task<LabTest?> UpdateLabPriceRepository(int id, decimal price);
    }
}
