using System;

namespace ClinicManagementSystem.ViewModels
{
    public class ConsultationBillViewModel
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public decimal ConsultationFee { get; set; }
        public int BillId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
