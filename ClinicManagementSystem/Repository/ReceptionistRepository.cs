using ClinicManagementSystem.Models;
using ClinicManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ClinicManagementSystem.Repository
{
    public class ReceptionistRepository : IReceptionistRepository
    {
        private readonly HospitalManagementDbContext _context;

        public ReceptionistRepository(HospitalManagementDbContext context)
        {
            _context = context;
        }

        #region Patient CRUD
        public IEnumerable<Patient> GetAllPatients()
        {
            return _context.Patients.OrderByDescending(p => p.CreatedAt).ToList();
        }

        public IEnumerable<Patient> SearchPatients(string searchValue)
        {
            if (string.IsNullOrWhiteSpace(searchValue))
                return GetAllPatients();

            return _context.Patients
                .Where(p => p.PatientName.Contains(searchValue) || (p.Contact != null && p.Contact.Contains(searchValue)))
                .ToList();
        }

        public Patient? GetPatientById(int patientId)
        {
            return _context.Patients.Find(patientId);
        }

        public void InsertPatient(Patient patient)
        {
            _context.Patients.Add(patient);
            _context.SaveChanges();
        }

        public void UpdatePatient(Patient patient)
        {
            var existing = _context.Patients.Find(patient.PatientId);
            if (existing != null)
            {
                existing.PatientName = patient.PatientName;
                existing.Dob = patient.Dob;
                existing.Gender = patient.Gender;
                existing.Contact = patient.Contact;
                existing.Email = patient.Email;
                existing.Address = patient.Address;
                _context.SaveChanges();
            }
        }
        #endregion

        #region Appointment Booking
        public IEnumerable<Doctor> GetAllDoctors()
        {
            return _context.Doctors
                .Include(d => d.Specialization)
                .Include(d => d.User)
                .ToList();
        }

        public IEnumerable<SlotMaster> GetAvailableSlots(int doctorId, DateTime appointmentDate)
        {
            var dateOnly = DateOnly.FromDateTime(appointmentDate);
            
            // Get all slots
            var allSlots = _context.SlotMasters.ToList();

            // Get booked slots for this doctor and date
            var bookedSlotIds = _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate == dateOnly && a.StatusId != 5) // 5 = Cancelled
                .Select(a => a.SlotId)
                .ToList();

            return allSlots.Where(s => !bookedSlotIds.Contains(s.SlotId)).ToList();
        }

        public (bool success, int appointmentId, string message) BookAppointment(AppointmentBookingViewModel model)
        {
            try
            {
                // We utilize the stored procedure sp_BookAppointment for complex validation (double booking, etc.)
                // If the procedure is NOT available, we implement it here in EF Core.
                // Assuming the SP exists in the DB as per legacy code.

                var patientIdParam = new SqlParameter("@patient_id", model.PatientId);
                var doctorIdParam = new SqlParameter("@doctor_id", model.DoctorId);
                var slotIdParam = new SqlParameter("@slot_id", model.SlotId);
                var dateParam = new SqlParameter("@appointment_date", model.AppointmentDate);
                var createdByParam = new SqlParameter("@created_by", model.CreatedBy);

                // For simplicity and absolute reliability in this migration, let's implement the logic in C# 
                // in case SPs are missing, but following the user's legacy validation intent.
                
                var dateOnly = DateOnly.FromDateTime(model.AppointmentDate);

                // Double booking check
                bool alreadyBooked = _context.Appointments.Any(a => 
                    a.DoctorId == model.DoctorId && 
                    a.AppointmentDate == dateOnly && 
                    a.SlotId == model.SlotId && 
                    a.StatusId != 5);

                if (alreadyBooked)
                    return (false, 0, "This slot is already booked for the selected doctor.");

                var appointment = new Appointment
                {
                    PatientId = model.PatientId,
                    DoctorId = model.DoctorId,
                    SlotId = model.SlotId,
                    AppointmentDate = dateOnly,
                    CreatedBy = model.CreatedBy,
                    StatusId = 1, // Pending
                    CreatedAt = DateTime.Now
                };

                _context.Appointments.Add(appointment);
                _context.SaveChanges();

                return (true, appointment.AppointmentId, "Appointment booked successfully.");
            }
            catch (Exception ex)
            {
                return (false, 0, ex.Message);
            }
        }
        #endregion

        #region Appointment History
        public IEnumerable<object> GetAppointmentsByReceptionist(int receptionistId)
        {
            return (from a in _context.Appointments
                    join p in _context.Patients on a.PatientId equals p.PatientId
                    join d in _context.Doctors on a.DoctorId equals d.DoctorId
                    join u in _context.Users on d.UserId equals u.UserId
                    join s in _context.SlotMasters on a.SlotId equals s.SlotId
                    join st in _context.Statuses on a.StatusId equals st.StatusId
                    where a.CreatedBy == receptionistId
                    orderby a.AppointmentDate descending, s.SlotTime descending
                    select new
                    {
                        AppointmentId = a.AppointmentId,
                        PatientName = p.PatientName,
                        DoctorName = u.FullName,
                        AppointmentDate = a.AppointmentDate.ToDateTime(TimeOnly.MinValue),
                        SlotTime = s.SlotTime,
                        TokenNo = s.TokenNo,
                        ConsultationFee = d.ConsultationFee ?? 0,
                        StatusName = st.StatusName,
                        CreatedAt = a.CreatedAt
                    }).ToList();
        }
        #endregion

        #region Billing
        public bool BillExists(int appointmentId)
        {
            return _context.BillConsultations.Any(b => b.AppointmentId == appointmentId);
        }

        public ConsultationBillViewModel GetBillDetails(int appointmentId)
        {
            var details = (from a in _context.Appointments
                          join p in _context.Patients on a.PatientId equals p.PatientId
                          join d in _context.Doctors on a.DoctorId equals d.DoctorId
                          join u in _context.Users on d.UserId equals u.UserId
                          join s in _context.Specializations on d.SpecializationId equals s.SpecializationId
                          where a.AppointmentId == appointmentId
                          select new ConsultationBillViewModel
                          {
                              AppointmentId = a.AppointmentId,
                              AppointmentDate = a.AppointmentDate.ToDateTime(TimeOnly.MinValue),
                              PatientName = p.PatientName,
                              Contact = p.Contact ?? "",
                              DoctorName = u.FullName,
                              Specialization = s.SpecializationName,
                              ConsultationFee = d.ConsultationFee ?? 0
                          }).FirstOrDefault();

            return details ?? new ConsultationBillViewModel();
        }

        public void InsertBill(int appointmentId, decimal totalAmount, int statusId)
        {
            var bill = new BillConsultation
            {
                AppointmentId = appointmentId,
                TotalAmount = totalAmount,
                StatusId = statusId,
                CreatedAt = DateTime.Now
            };

            _context.BillConsultations.Add(bill);

            // Update appointment status to 'Billed' (e.g. 6 or similar based on Status metadata)
            var appointment = _context.Appointments.Find(appointmentId);
            if (appointment != null)
            {
                appointment.StatusId = 6; // Billed
            }

            _context.SaveChanges();
        }
        #endregion
    }
}
