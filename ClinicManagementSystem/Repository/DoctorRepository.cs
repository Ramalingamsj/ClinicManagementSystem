using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ClinicManagementSystem.Repository
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly HospitalManagementDbContext _context;
        public DoctorRepository(HospitalManagementDbContext context)
        {
            _context = context;
        }

        public async Task<Consultation> AddConsultationRepository(Consultation consultation)
        {
            if (_context == null) return null;

            // Check if consultation already exists for this appointment
            var existingConsultation = await _context.Consultations
                .FirstOrDefaultAsync(c => c.AppointmentId == consultation.AppointmentId);

            if (existingConsultation != null)
            {
                // Update existing record
                existingConsultation.Symptoms = consultation.Symptoms;
                existingConsultation.Diagnosis = consultation.Diagnosis;
                existingConsultation.DoctorNotes = consultation.DoctorNotes;
                // Keep the original CreatedAt or update it if preferred. We'll keep it.
                
                _context.Consultations.Update(existingConsultation);
                await _context.SaveChangesAsync();
                return existingConsultation;
            }
            else
            {
                // Add new record
                await _context.Consultations.AddAsync(consultation);

                // Auto-update the related Appointment Status to 2 ('Completed')
                var appointment = await _context.Appointments.FindAsync(consultation.AppointmentId);
                if (appointment != null)
                {
                    appointment.StatusId = 2;
                }

                await _context.SaveChangesAsync();
                return consultation;
            }
        }

        public async Task<IEnumerable<Appointment>> GetPendingPatientsRepository(int id)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return await _context.Appointments
                 .Include(a => a.Patient)
                 .Include(a => a.Status)
                 .Include(a => a.Slot)
                 .Include(a => a.Doctor)
                     .ThenInclude(d => d.User)              
                 .Include(a => a.Doctor)
                     .ThenInclude(d => d.Specialization)    
                 .Where(a => a.DoctorId == id &&
                             a.AppointmentDate == today)
                 .ToListAsync();
        }

        public async Task<Doctor> GetDoctorProfileRepository(int doctorId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(d => d.DoctorId == doctorId);
        }

        public async Task<IEnumerable<Consultation>> GetPatientHistoryRepository(int patientId, int doctorId)
        {
            return await _context.Consultations
                .Include(c => c.Appointment)
                    .ThenInclude(a => a.Doctor)
                        .ThenInclude(d => d.Specialization)
                .Include(c => c.PatientMedicines)
                    .ThenInclude(pm => pm.Medicine)
                .Include(c => c.PatientLabTests)
                    .ThenInclude(plt => plt.Labtest)
                .Where(c => c.Appointment.PatientId == patientId && c.Appointment.DoctorId == doctorId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Appointment> GetAppointmentByIdRepository(int appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }

        public async Task<PatientMedicine> AddPrescriptionRepository(int consultationId, PatientMedicine medicine)
        {
            if (medicine != null)
            {
                medicine.ConsultationId = consultationId;
                medicine.StatusId = 6; // 6 = 'Prescribed' according to the status table
                medicine.IssuedBy = null; // Filled by Pharmacy later
                medicine.Quantity = null; // Filled by Pharmacy
                await _context.PatientMedicines.AddAsync(medicine);
                await _context.SaveChangesAsync();

                // Explicitly load the Status navigation property so it appears in the JSON response
                await _context.Entry(medicine).Reference(m => m.Status).LoadAsync();
            }
            return medicine;
        }

        public async Task<PatientLabTest> AddLabTestRepository(int consultationId, PatientLabTest labTest)
        {
            if (labTest != null)
            {
                labTest.ConsultationId = consultationId;
                labTest.TestDate = DateTime.Now;
                labTest.Result = null; // Filled by lab tech later
                labTest.UpdatedBy = null; // Filled by lab tech later
                labTest.StatusId = 6; // 6 = 'Prescribed'

                await _context.PatientLabTests.AddAsync(labTest);
                await _context.SaveChangesAsync();

                // Explicitly load the Status navigation property so it appears in the JSON response
                await _context.Entry(labTest).Reference(lt => lt.Status).LoadAsync();
            }
            return labTest;
        }

        public async Task<Consultation> GetConsultationDetailsRepository(int consultationId)
        {
            return await _context.Consultations
                .Include(c => c.PatientMedicines)
                    .ThenInclude(pm => pm.Medicine)
                .Include(c => c.PatientMedicines)
                    .ThenInclude(pm => pm.Status)
                .Include(c => c.PatientLabTests)
                    .ThenInclude(plt => plt.Labtest)
                .Include(c => c.PatientLabTests)
                    .ThenInclude(plt => plt.Status)
                .FirstOrDefaultAsync(c => c.ConsultationId == consultationId);
        }

        public async Task<Consultation> GetConsultationByAppointmentRepository(int appointmentId)
        {
            return await _context.Consultations
                .Include(c => c.PatientMedicines)
                    .ThenInclude(pm => pm.Medicine)
                .Include(c => c.PatientMedicines)
                    .ThenInclude(pm => pm.Status)
                .Include(c => c.PatientLabTests)
                    .ThenInclude(plt => plt.Labtest)
                .Include(c => c.PatientLabTests)
                    .ThenInclude(plt => plt.Status)
                .FirstOrDefaultAsync(c => c.AppointmentId == appointmentId);
        }

        public async Task<IEnumerable<Medicine>> GetAllMedicinesRepository()
        {
            return await _context.Medicines.ToListAsync();
        }

        public async Task<IEnumerable<LabTest>> GetAllLabTestsRepository()
        {
            return await _context.LabTests.ToListAsync();
        }

        public async Task<PatientMedicine> EditPrescriptionRepository(int patientMedicineId, PatientMedicine medicine)
        {
            var existingMedicine = await _context.PatientMedicines.FindAsync(patientMedicineId);
            if (existingMedicine != null)
            {
                existingMedicine.MedicineId = medicine.MedicineId;
                existingMedicine.DurationDays = medicine.DurationDays;
                existingMedicine.Frequency = medicine.Frequency;
                existingMedicine.StatusId = 6; // Force back to Prescribed on edit
                
                await _context.SaveChangesAsync();
                await _context.Entry(existingMedicine).Reference(m => m.Status).LoadAsync();
            }
            return existingMedicine;
        }

        public async Task<bool> DeletePrescriptionRepository(int patientMedicineId)
        {
            var medicine = await _context.PatientMedicines.FindAsync(patientMedicineId);
            if (medicine != null)
            {
                _context.PatientMedicines.Remove(medicine);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<PatientLabTest> EditLabTestRepository(int patientLabtestId, PatientLabTest labTest)
        {
            var existingLabTest = await _context.PatientLabTests.FindAsync(patientLabtestId);
            if (existingLabTest != null)
            {
                existingLabTest.LabtestId = labTest.LabtestId;
                existingLabTest.StatusId = 6; // Force back to Prescribed on edit
                
                await _context.SaveChangesAsync();
                await _context.Entry(existingLabTest).Reference(lt => lt.Status).LoadAsync();
            }
            return existingLabTest;
        }

        public async Task<bool> DeleteLabTestRepository(int patientLabtestId)
        {
            var labTest = await _context.PatientLabTests.FindAsync(patientLabtestId);
            if (labTest != null)
            {
                _context.PatientLabTests.Remove(labTest);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<dynamic>> GetDoctorHistoryRepository(int doctorId)
        {
            return await _context.Consultations
                .Include(c => c.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(c => c.PatientMedicines)
                .Include(c => c.PatientLabTests)
                .Where(c => c.Appointment.DoctorId == doctorId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new
                {
                    ConsultationId = c.ConsultationId,
                    AppointmentId = c.AppointmentId,
                    PatientName = c.Appointment.Patient.PatientName,
                    Diagnosis = c.Diagnosis,
                    ConsultationDate = c.CreatedAt,
                    PatientMedicines = c.PatientMedicines.Select(pm => new { pm.PatientMedicineId }), // Minimal data for counts
                    PatientLabTests = c.PatientLabTests.Select(plt => new { plt.PatientLabtestId })    // Minimal data for counts
                })
                .ToListAsync<dynamic>();
        }
    }
}

