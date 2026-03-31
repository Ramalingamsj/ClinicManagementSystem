using ClinicManagementSystem.Models;
using ClinicManagementSystem.ViewModels;
using System.Collections.Generic;
using System;

namespace ClinicManagementSystem.Repository
{
    public interface IReceptionistRepository
    {
        // Patient CRUD
        IEnumerable<Patient> GetAllPatients();
        IEnumerable<Patient> SearchPatients(string searchValue);
        Patient? GetPatientById(int patientId);
        void InsertPatient(Patient patient);
        void UpdatePatient(Patient patient);

        // Appointment Booking
        IEnumerable<Doctor> GetAllDoctors();
        IEnumerable<SlotMaster> GetAvailableSlots(int doctorId, DateTime appointmentDate);
        (bool success, int appointmentId, string message) BookAppointment(AppointmentBookingViewModel model);

        // Appointment History
        IEnumerable<object> GetAppointmentsByReceptionist(int receptionistId);

        // Billing
        bool BillExists(int appointmentId);
        ConsultationBillViewModel GetBillDetails(int appointmentId);
        void InsertBill(int appointmentId, decimal totalAmount, int statusId);
    }
}
