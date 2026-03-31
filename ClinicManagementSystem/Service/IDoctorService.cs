using ClinicManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ClinicManagementSystem.Service
{
    public interface IDoctorService
    {
        public Task<IEnumerable<Appointment>> GetPendingPatientsService(int id);
        public Task<Consultation> AddConsultationService(Consultation consultation);
        public Task<Doctor> GetDoctorProfileService(int doctorId);
        public Task<IEnumerable<Consultation>> GetPatientHistoryService(int patientId, int doctorId);
        public Task<IEnumerable<dynamic>> GetDoctorHistoryService(int doctorId);

        public Task<Appointment> GetAppointmentByIdService(int appointmentId);
        public Task<PatientMedicine> AddPrescriptionService(int consultationId, PatientMedicine medicine);
        public Task<PatientLabTest> AddLabTestService(int consultationId, PatientLabTest labTest);
        
        public Task<Consultation> GetConsultationDetailsService(int consultationId);
        public Task<Consultation> GetConsultationByAppointmentService(int appointmentId);
        
        public Task<IEnumerable<Medicine>> GetAllMedicinesService();
        public Task<IEnumerable<LabTest>> GetAllLabTestsService();
        
        public Task<PatientMedicine> EditPrescriptionService(int patientMedicineId, PatientMedicine medicine);
        public Task<bool> DeletePrescriptionService(int patientMedicineId);
        public Task<PatientLabTest> EditLabTestService(int patientLabtestId, PatientLabTest labTest);
        public Task<bool> DeleteLabTestService(int patientLabtestId);
    }
}
