using System;

namespace ClinicManagementSystem.ViewModels
{
    public class AppointmentBookingViewModel
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int SlotId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
