using ClinicManagementSystem.Models;
using ClinicManagementSystem.ViewModels;
using ClinicManagementSystem.Repository;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ClinicManagementSystem.Service
{
    public class ReceptionistService : IReceptionistService
    {
        private readonly IReceptionistRepository _repository;

        public ReceptionistService(IReceptionistRepository repository)
        {
            _repository = repository;
        }

        #region Patient CRUD
        public IEnumerable<Patient> GetAllPatients() => _repository.GetAllPatients();

        public IEnumerable<Patient> SearchPatients(string searchValue) => _repository.SearchPatients(searchValue);

        public Patient? GetPatientById(int patientId) => _repository.GetPatientById(patientId);

        public void InsertPatient(Patient patient) => _repository.InsertPatient(patient);

        public void UpdatePatient(Patient patient) => _repository.UpdatePatient(patient);
        #endregion

        #region Appointment Booking
        public IEnumerable<object> GetAllDoctors()
        {
            return _repository.GetAllDoctors().Select(d => new 
            {
                DoctorId = d.DoctorId,
                FullName = d.User?.FullName ?? "Unknown",
                Specialization = d.Specialization?.SpecializationName ?? "General"
            });
        }

        public IEnumerable<SlotMaster> GetAvailableSlots(int doctorId, DateTime appointmentDate) 
            => _repository.GetAvailableSlots(doctorId, appointmentDate);

        public (bool success, int appointmentId, string message) BookAppointment(AppointmentBookingViewModel model) 
            => _repository.BookAppointment(model);

        public IEnumerable<object> GetAppointmentsByReceptionist(int receptionistId)
            => _repository.GetAppointmentsByReceptionist(receptionistId);
        #endregion

        #region Billing
        public bool BillExists(int appointmentId) => _repository.BillExists(appointmentId);

        public ConsultationBillViewModel GetBillDetails(int appointmentId) => _repository.GetBillDetails(appointmentId);

        public void InsertBill(int appointmentId, decimal totalAmount, int statusId) 
            => _repository.InsertBill(appointmentId, totalAmount, statusId);
        #endregion
    }
}
