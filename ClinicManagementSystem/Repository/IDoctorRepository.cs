using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ClinicManagementSystem.Repository
{
    public interface IDoctorRepository
    {
        public Task<IEnumerable<Appointment>> GetPendingPatientsRepository(int id);
        public Task<Consultation> AddConsultationRepository(Consultation consultation);
        public Task<Doctor> GetDoctorProfileRepository(int doctorId);
        public Task<IEnumerable<Consultation>> GetPatientHistoryRepository(int patientId, int doctorId);
        public Task<IEnumerable<dynamic>> GetDoctorHistoryRepository(int doctorId);

        Task<Appointment> GetAppointmentByIdRepository(int appointmentId);
        public Task<PatientMedicine> AddPrescriptionRepository(int consultationId, PatientMedicine medicine);
        public Task<PatientLabTest> AddLabTestRepository(int consultationId, PatientLabTest labTest);
        
        public Task<Consultation> GetConsultationDetailsRepository(int consultationId);
        public Task<Consultation> GetConsultationByAppointmentRepository(int appointmentId);
        
        public Task<IEnumerable<Medicine>> GetAllMedicinesRepository();
        public Task<IEnumerable<LabTest>> GetAllLabTestsRepository();
        
        public Task<PatientMedicine> EditPrescriptionRepository(int patientMedicineId, PatientMedicine medicine);
        public Task<bool> DeletePrescriptionRepository(int patientMedicineId);
        public Task<PatientLabTest> EditLabTestRepository(int patientLabtestId, PatientLabTest labTest);
        public Task<bool> DeleteLabTestRepository(int patientLabtestId);
    }
}
