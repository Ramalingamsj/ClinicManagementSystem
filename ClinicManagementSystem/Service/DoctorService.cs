using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ClinicManagementSystem.Service
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repository;
        public DoctorService(IDoctorRepository repository)
        {
            _repository = repository;
        }

        public async Task<Consultation> AddConsultationService(Consultation consultation)
        {
            return await _repository.AddConsultationRepository(consultation);
        }

        public async Task<IEnumerable<Appointment>> GetPendingPatientsService(int id)
        {
            return await _repository.GetPendingPatientsRepository(id);
        }

        public async Task<Doctor> GetDoctorProfileService(int doctorId)
        {
            return await _repository.GetDoctorProfileRepository(doctorId);
        }

        public async Task<IEnumerable<Consultation>> GetPatientHistoryService(int patientId, int doctorId)
        {
            return await _repository.GetPatientHistoryRepository(patientId, doctorId);
        }

        public async Task<Appointment> GetAppointmentByIdService(int appointmentId)
        {
            return await _repository.GetAppointmentByIdRepository(appointmentId);
        }

        public async Task<PatientMedicine> AddPrescriptionService(int consultationId, PatientMedicine medicine)
        {
            return await _repository.AddPrescriptionRepository(consultationId, medicine);
        }

        public async Task<PatientLabTest> AddLabTestService(int consultationId, PatientLabTest labTest)
        {
            return await _repository.AddLabTestRepository(consultationId, labTest);
        }

        public async Task<Consultation> GetConsultationDetailsService(int consultationId)
        {
            return await _repository.GetConsultationDetailsRepository(consultationId);
        }

        public async Task<Consultation> GetConsultationByAppointmentService(int appointmentId)
        {
            return await _repository.GetConsultationByAppointmentRepository(appointmentId);
        }

        public async Task<IEnumerable<Medicine>> GetAllMedicinesService()
        {
            return await _repository.GetAllMedicinesRepository();
        }

        public async Task<IEnumerable<LabTest>> GetAllLabTestsService()
        {
            return await _repository.GetAllLabTestsRepository();
        }

        public async Task<PatientMedicine> EditPrescriptionService(int patientMedicineId, PatientMedicine medicine)
        {
            return await _repository.EditPrescriptionRepository(patientMedicineId, medicine);
        }

        public async Task<bool> DeletePrescriptionService(int patientMedicineId)
        {
            return await _repository.DeletePrescriptionRepository(patientMedicineId);
        }

        public async Task<PatientLabTest> EditLabTestService(int patientLabtestId, PatientLabTest labTest)
        {
            return await _repository.EditLabTestRepository(patientLabtestId, labTest);
        }

        public async Task<bool> DeleteLabTestService(int patientLabtestId)
        {
            return await _repository.DeleteLabTestRepository(patientLabtestId);
        }

        public async Task<IEnumerable<dynamic>> GetDoctorHistoryService(int doctorId)
        {
            return await _repository.GetDoctorHistoryRepository(doctorId);
        }
    }
}

