using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Repository
{
    public interface IDoctorRepository
    {
        public Task<IEnumerable<Appointment>> GetPendingPatientsRepository(int id);
        public Task<Consultation> AddConsultationRepository(Consultation consultation);
        public Task<Doctor> GetDoctorProfileRepository(int doctorId);
        public Task<IEnumerable<Consultation>> GetPatientHistoryRepository(int patientId, int doctorId);
        public Task<PatientMedicine> AddPrescriptionRepository(int consultationId, PatientMedicine medicine);
        public Task<PatientLabTest> AddLabTestRepository(int consultationId, PatientLabTest labTest);
        
        public Task<Consultation> GetConsultationDetailsRepository(int consultationId);
        public Task<PatientMedicine> EditPrescriptionRepository(int patientMedicineId, PatientMedicine medicine);
        public Task<bool> DeletePrescriptionRepository(int patientMedicineId);
        public Task<PatientLabTest> EditLabTestRepository(int patientLabtestId, PatientLabTest labTest);
        public Task<bool> DeleteLabTestRepository(int patientLabtestId);
    }
}
