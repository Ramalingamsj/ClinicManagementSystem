using ClinicManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Repository
{
    public class PharmacistRepository : IPharmacistRepository
    {
        private readonly HospitalManagementDbContext _context;

        public PharmacistRepository(HospitalManagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetPendingMedicinesRepository()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _context.Consultations
                .Include(c => c.Appointment).ThenInclude(a => a.Patient)
                .Include(c => c.PatientMedicines).ThenInclude(pm => pm.Medicine)
                .Include(c => c.PatientMedicines).ThenInclude(pm => pm.Status)
                .Where(c => c.Appointment != null && c.Appointment.AppointmentDate == today)
                .Select(c => new
                {
                    ConsultationId = c.ConsultationId,
                    PatientId = c.Appointment != null ? c.Appointment.PatientId : 0,
                    PatientName = c.Appointment != null && c.Appointment.Patient != null ? c.Appointment.Patient.PatientName : "Unknown",
                    Medicines = c.PatientMedicines
                        .Select(pm => new
                        {
                            PatientMedicineId = pm.PatientMedicineId,
                            MedicineName = pm.Medicine != null ? pm.Medicine.MedicineName : "Unknown",
                            MedicineType = pm.Medicine != null ? pm.Medicine.MedicineType : "Unknown",
                            StatusId = pm.StatusId,
                            StatusName = pm.Status != null ? pm.Status.StatusName : "Pending",
                            Frequency = pm.Frequency ?? 0,
                            DurationDays = pm.DurationDays ?? 0,
                            Stock = pm.Medicine != null ? (pm.Medicine.StockQuantity ?? 0) : 0,
                            Quantity = pm.Quantity ?? 0,
                            Price = pm.Medicine != null ? (pm.Medicine.Price ?? 0) : 0
                        }).ToList()
                })
                .ToListAsync();
        }

        public async Task<bool> HasPendingMedicinesRepository(int consultationId)
        {
            return await _context.PatientMedicines.AnyAsync(pm => pm.ConsultationId == consultationId && (pm.StatusId == 6 || pm.StatusId == 4));
        }

        public async Task<Medicine> AddMedicineRepository(Medicine m)
        {
            if (m == null) throw new ArgumentNullException(nameof(m));
            if (string.IsNullOrWhiteSpace(m.MedicineName))
                throw new ArgumentException("Medicine name is required.");

            // Check for duplicates
            bool exists = await _context.Medicines.AnyAsync(x => x.MedicineName.ToLower() == m.MedicineName.ToLower());
            if (exists)
                throw new InvalidOperationException($"Medicine '{m.MedicineName}' already exists in the inventory.");

            // Special character check
            if (!System.Text.RegularExpressions.Regex.IsMatch(m.MedicineName, @"^[a-zA-Z0-9 ]*$"))
                throw new ArgumentException("Medicine name cannot contain special characters.");

            if ((m.Price ?? 0) <= 0)
                throw new ArgumentException("Unit Price must be greater than zero.");

            if ((m.StockQuantity ?? 0) < 0)
                throw new ArgumentException("Initial Stock cannot be negative.");

            if ((m.StockQuantity ?? 0) > 1000)
                throw new ArgumentException("Initial Stock cannot exceed 1000 units.");

            await _context.Medicines.AddAsync(m);
            await _context.SaveChangesAsync();
            return m;
        }

        public async Task<Medicine> UpdateStockRepository(int id, int stock)
        {
            var med = await _context.Medicines.FindAsync(id);
            if (med != null)
            {
                // Add the new stock to the existing stock instead of replacing it
                med.StockQuantity = (med.StockQuantity ?? 0) + stock;
                await _context.SaveChangesAsync();
            }
            return med;
        }

        public async Task<IEnumerable<Medicine>> GetMedicinesRepository()
        {
            return await _context.Medicines.ToListAsync();
        }

        public async Task<BillMed> GenerateMedicineBillRepository(int consultationId)
        {
            var medicines = await _context.PatientMedicines
                .Include(pm => pm.Medicine)
                .Where(pm => pm.ConsultationId == consultationId && pm.StatusId == 7) // Issued
                .ToListAsync();

            decimal totalAmount = 0;
            foreach (var m in medicines)
            {
                if (m.Medicine != null && m.Quantity.HasValue)
                {
                    totalAmount += (m.Medicine.Price ?? 0) * m.Quantity.Value;
                }
            }

            var existingBill = await _context.BillMeds.FirstOrDefaultAsync(b => b.ConsultationId == consultationId);
            
            if(existingBill != null) 
            {
                // Update the existing bill amount instead of ignoring recalculation
                existingBill.TotalAmount = totalAmount;
                await _context.SaveChangesAsync();
                return existingBill;
            }

            var bill = new BillMed
            {
                ConsultationId = consultationId,
                TotalAmount = totalAmount,
                StatusId = 4, // Pending/Generated
                CreatedAt = DateTime.Now
            };

            await _context.BillMeds.AddAsync(bill);
            await _context.SaveChangesAsync();
            
            return bill;
        }

        public async Task<IEnumerable<object>> GetAllBillsRepository()
        {
            return await _context.BillMeds
                .Include(b => b.Consultation)
                    .ThenInclude(c => c.Appointment)
                        .ThenInclude(a => a.Patient)
                .Include(b => b.Status)
                .Select(b => new
                {
                    BillId = b.BillId,
                    ConsultationId = b.ConsultationId,
                    PatientName = b.Consultation != null && b.Consultation.Appointment != null && b.Consultation.Appointment.Patient != null ? b.Consultation.Appointment.Patient.PatientName : "Unknown",
                    TotalAmount = b.TotalAmount,
                    Status = b.Status != null ? b.Status.StatusName : "Unknown",
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetIssuedMedicinesHistoryRepository()
        {
            return await _context.Consultations
                .Include(c => c.Appointment).ThenInclude(a => a.Patient)
                .Include(c => c.PatientMedicines).ThenInclude(pm => pm.Medicine)
                .Include(c => c.PatientMedicines).ThenInclude(pm => pm.Status)
                .Where(c => c.PatientMedicines.Any(pm => pm.StatusId == 7))
                .Select(c => new
                {
                    ConsultationId = c.ConsultationId,
                    PatientId = c.Appointment != null ? c.Appointment.PatientId : 0,
                    PatientName = c.Appointment != null && c.Appointment.Patient != null ? c.Appointment.Patient.PatientName : "Unknown",
                    PatientContact = c.Appointment != null && c.Appointment.Patient != null ? c.Appointment.Patient.Contact : "",
                    CreatedAt = c.CreatedAt,
                    AppointmentDate = c.Appointment != null ? c.Appointment.AppointmentDate : (DateOnly?)null,
                    TotalAmount = c.BillMed != null ? c.BillMed.TotalAmount : 0,
                    Medicines = c.PatientMedicines
                        .Where(pm => pm.StatusId == 7)
                        .Select(pm => new
                        {
                            PatientMedicineId = pm.PatientMedicineId,
                            MedicineName = pm.Medicine != null ? pm.Medicine.MedicineName : "Unknown",
                            MedicineType = pm.Medicine != null ? pm.Medicine.MedicineType : "Unknown",
                            StatusId = pm.StatusId,
                            StatusName = pm.Status != null ? pm.Status.StatusName : "Issued",
                            Frequency = pm.Frequency ?? 0,
                            DurationDays = pm.DurationDays ?? 0,
                            Quantity = pm.Quantity ?? 0,
                            IssuedBy = pm.IssuedBy
                        }).ToList()
                })
                .ToListAsync();
        }

        public async Task<decimal> GetBillAmountRepository(int consultationId)
        {
            var bill = await _context.BillMeds.FirstOrDefaultAsync(b => b.ConsultationId == consultationId);
            return bill?.TotalAmount ?? 0;
        }

        public async Task<bool> BillExistsRepository(int consultationId)
        {
            return await _context.BillMeds.AnyAsync(b => b.ConsultationId == consultationId);
        }

        public async Task<string> GetPatientNameRepository(int consultationId)
        {
            var consultation = await _context.Consultations
                .Include(c => c.Appointment)
                    .ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(c => c.ConsultationId == consultationId);

            return consultation?.Appointment?.Patient?.PatientName ?? "Unknown";
        }

        public async Task<IEnumerable<object>> GetTodaysConsultationsRepository()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _context.Consultations
                .Include(c => c.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(c => c.PatientMedicines)
                    .ThenInclude(pm => pm.Medicine)
                .Include(c => c.PatientMedicines)
                    .ThenInclude(pm => pm.Status)
                .Where(c => c.Appointment != null && c.Appointment.AppointmentDate == today)
                .Select(c => new
                {
                    ConsultationId = c.ConsultationId,
                    PatientName = c.Appointment!.Patient.PatientName,
                    Medicines = c.PatientMedicines.Select(pm => new
                    {
                        PatientMedicineId = pm.PatientMedicineId,
                        MedicineId = pm.MedicineId,
                        MedicineName = pm.Medicine != null ? pm.Medicine.MedicineName : "",
                        DurationDays = pm.DurationDays,
                        Frequency = pm.Frequency,
                        Quantity = pm.Quantity,
                        StatusId = pm.StatusId,
                        StatusName = pm.Status != null ? pm.Status.StatusName : "",
                        Price = pm.Medicine != null ? (pm.Medicine.Price ?? 0) : 0
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<PatientMedicine> IssueMedicineRepository(int patientMedicineId, int pharmacyUserId)
        {
            var pm = await _context.PatientMedicines.Include(p => p.Medicine).FirstOrDefaultAsync(p => p.PatientMedicineId == patientMedicineId);
            if (pm != null && pm.Medicine != null)
            {
                int calculatedQuantity = (pm.Frequency ?? 0) * (pm.DurationDays ?? 0);
                
                if (pm.Medicine.StockQuantity < calculatedQuantity)
                {
                    throw new Exception("Insufficient stock for medicine: " + pm.Medicine.MedicineName);
                }

                pm.Quantity = calculatedQuantity;
                pm.IssuedBy = pharmacyUserId;
                pm.StatusId = 7; // 'Issued'
                
                // Deduct stock
                pm.Medicine.StockQuantity -= calculatedQuantity;
                
                await _context.SaveChangesAsync();
                await _context.Entry(pm).Reference(p => p.Status).LoadAsync();

                // Check if all medicines for this consultation are issued
                bool allIssued = !await _context.PatientMedicines
                    .AnyAsync(m => m.ConsultationId == pm.ConsultationId && m.StatusId != 7);
                
                if (allIssued)
                {
                    await GenerateMedicineBillRepository(pm.ConsultationId);
                }
            }
            return pm;
        }

        public async Task<object?> GetConsultationDetailsRepository(int consultationId)
        {
            return await _context.Consultations
                .Include(c => c.Appointment).ThenInclude(a => a.Patient)
                .Include(c => c.PatientMedicines).ThenInclude(pm => pm.Medicine)
                .Include(c => c.BillMed)
                .Where(c => c.ConsultationId == consultationId)
                .Select(c => new
                {
                    PatientName = c.Appointment != null && c.Appointment.Patient != null ? c.Appointment.Patient.PatientName : "Patient",
                    Contact = c.Appointment != null && c.Appointment.Patient != null ? c.Appointment.Patient.Contact : "",
                    TotalAmount = c.BillMed != null ? c.BillMed.TotalAmount : 0,
                    Medicines = c.PatientMedicines
                        .Where(pm => pm.StatusId == 7)
                        .Select(pm => pm.Medicine != null ? pm.Medicine.MedicineName : "Med")
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }
    }
}
