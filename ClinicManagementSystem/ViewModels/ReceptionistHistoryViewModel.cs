using System;

namespace ClinicManagementSystem.ViewModels
{
    public class ReceptionistHistoryViewModel
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string SlotTime { get; set; }
        public int TokenNo { get; set; }
        public decimal ConsultationFee { get; set; }
        public string StatusName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
