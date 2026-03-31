using ClinicManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Service {
    public interface ILabTechnicianService {
        Task<IEnumerable<object>> GetPendingTestsService(); 
        Task<BillLab> GenerateLabBillService(int consultationId);
        Task<int> GetConsultationIdFromLabTestService(int id);
        Task<IEnumerable<object>> GetBillsService();
        Task<PatientLabTest> UpdateResultService(int id, string result, int userId);

        Task<decimal> GetBillAmountService(int consultationId); 
        Task<bool> BillExistsService(int consultationId); 
        Task<(bool Success, string Message)> SendSMSService(int consultationId, string phoneNumber, string message);

        // Lab Test Management
        Task<IEnumerable<LabTest>> GetLabTestsService();
        Task<LabTest> AddLabTestService(LabTest test);
        Task<LabTest?> UpdateLabPriceService(int id, decimal price);
    }
}
