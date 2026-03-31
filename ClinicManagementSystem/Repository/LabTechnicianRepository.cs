using ClinicManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Repository
{
    public class LabTechnicianRepository : ILabTechnicianRepository
    {
        private readonly HospitalManagementDbContext _context;

        public LabTechnicianRepository(HospitalManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetPendingTestsRepository()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _context.Consultations
                .Include(c => c.Appointment!).ThenInclude(a => a.Patient)
                .Include(c => c.PatientLabTests).ThenInclude(plt => plt.Labtest)
                .Include(c => c.PatientLabTests).ThenInclude(plt => plt.Status)
                .Where(c => c.Appointment != null && c.Appointment.AppointmentDate == today)
                .Select(c => new
                {
                    ConsultationId = c.ConsultationId,
                    PatientId = c.Appointment != null && c.Appointment.Patient != null ? c.Appointment.Patient.PatientId : 0,
                    PatientName = c.Appointment != null && c.Appointment.Patient != null ? c.Appointment.Patient.PatientName : "Unknown",
                    Contact = c.Appointment != null && c.Appointment.Patient != null ? (c.Appointment.Patient.Contact ?? "Not Provided") : "Not Provided",
                    LabTests = c.PatientLabTests
                        .Select(plt => new
                        {
                            PatientLabtestId = plt.PatientLabtestId,
                            TestName = plt.Labtest != null ? plt.Labtest.TestName : "Unknown",
                            Price = plt.Labtest != null ? (plt.Labtest.Price ?? 0) : 0,
                            StatusId = plt.StatusId,
                            StatusName = plt.Status != null ? plt.Status.StatusName : "Pending"
                        }).ToList()
                })
                .ToListAsync();
        }

        public async Task<PatientLabTest> UpdateResultRepository(int id, string result, int userId)
        {
            var test = await _context.PatientLabTests.Include(t => t.Labtest).FirstOrDefaultAsync(t => t.PatientLabtestId == id);
            if (test != null)
            {
                test.Result = result;
                test.UpdatedBy = userId;
                test.StatusId = 2; // 'Completed' based on user schema
                await _context.SaveChangesAsync();
                
                // Force EF Core to load the newly assigned navigation properties so they show up in the API output
                await _context.Entry(test).Reference(t => t.Status).LoadAsync();
                await _context.Entry(test).Reference(t => t.UpdatedByNavigation).LoadAsync();
                
                // Auto-bill logic: Check if all lab tests for this consultation are completed (2)
                bool allCompleted = !await _context.PatientLabTests
                    .AnyAsync(t => t.ConsultationId == test.ConsultationId && t.StatusId != 2);
                
                if (allCompleted)
                {
                    await GenerateLabBillRepository(test.ConsultationId);
                }
            }
            return test!;
        }

        public async Task<BillLab> GenerateLabBillRepository(int consultationId)
        {
            var tests = await _context.PatientLabTests
                .Include(t => t.Labtest)
                .Where(t => t.ConsultationId == consultationId && t.StatusId == 2) // Completed
                .ToListAsync();

            decimal totalAmount = 0;
            foreach (var t in tests)
            {
                if (t.Labtest != null)
                {
                    totalAmount += (t.Labtest.Price ?? 0);
                }
            }

            var existingBill = await _context.BillLabs.FirstOrDefaultAsync(b => b.ConsultationId == consultationId);
            
            if (existingBill != null)
            {
                existingBill.TotalAmount = totalAmount;
                await _context.SaveChangesAsync();
                return existingBill;
            }

            var bill = new BillLab
            {
                ConsultationId = consultationId,
                TotalAmount = totalAmount,
                StatusId = 2, // Pending/Generated
                CreatedAt = DateTime.Now
            };

            await _context.BillLabs.AddAsync(bill);
            await _context.SaveChangesAsync();
            
            return bill;
        }

        public async Task<int> GetConsultationIdFromLabTestRepository(int id)
        {
            var test = await _context.PatientLabTests.FindAsync(id);
            return test?.ConsultationId ?? 0;
        }

        public async Task<IEnumerable<object>> GetBillsRepository()
        {
            return await _context.BillLabs
                .Include(b => b.Status)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new
                {
                    BillId = b.BillId,
                    ConsultationId = b.ConsultationId,
                    PatientName = _context.Consultations
                        .Where(c => c.ConsultationId == b.ConsultationId)
                        .Select(c => c.Appointment != null ? c.Appointment.Patient.PatientName : "Unknown")
                        .FirstOrDefault() ?? "Unknown",
                    TotalAmount = b.TotalAmount,
                    StatusId = b.StatusId,
                    StatusName = b.Status != null ? b.Status.StatusName : "Unknown",
                    CreatedAt = b.CreatedAt,
                    LabTests = _context.PatientLabTests
                        .Where(plt => plt.ConsultationId == b.ConsultationId)
                        .Select(plt => new
                        {
                            TestName = plt.Labtest != null ? plt.Labtest.TestName : "Unknown",
                            Description = plt.Labtest != null ? plt.Labtest.Description : null,
                            Price = plt.Labtest != null ? (plt.Labtest.Price ?? 0) : 0,
                            Result = plt.Result
                        }).ToList()
                })
                .ToListAsync();
        }

        public async Task<decimal> GetBillAmountRepository(int consultationId)
        {
            var bill = await _context.BillLabs.FirstOrDefaultAsync(b => b.ConsultationId == consultationId);
            return bill?.TotalAmount ?? 0;
        }

        public async Task<bool> BillExistsRepository(int consultationId)
        {
            return await _context.BillLabs.AnyAsync(b => b.ConsultationId == consultationId);
        }

        public async Task<IEnumerable<LabTest>> GetLabTestsRepository()
        {
            return await _context.LabTests
                .OrderBy(l => l.TestName)
                .ToListAsync();
        }

        public async Task<LabTest> AddLabTestRepository(LabTest test)
        {
            // Duplicate check (Case-insensitive)
            var exists = await _context.LabTests
                .AnyAsync(l => l.TestName.ToLower() == test.TestName.ToLower());
            
            if (exists)
                throw new Exception($"Duplicate entry: {test.TestName} already exists in the catalog.");

            _context.LabTests.Add(test);
            await _context.SaveChangesAsync();
            return test;
        }

        public async Task<LabTest?> UpdateLabPriceRepository(int id, decimal price)
        {
            var test = await _context.LabTests.FindAsync(id);
            if (test == null) return null;

            test.Price = price;
            await _context.SaveChangesAsync();
            return test;
        }
    }
}
