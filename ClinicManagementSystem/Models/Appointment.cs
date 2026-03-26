using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClinicManagementSystem.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int SlotId { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public int? StatusId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }
    [JsonIgnore]
    public virtual BillConsultation? BillConsultation { get; set; }
    [JsonIgnore]
    public virtual Consultation? Consultation { get; set; }
    [JsonIgnore]
    public virtual User? CreatedByNavigation { get; set; }
    
    public virtual Doctor Doctor { get; set; } = null!;
    
    public virtual Patient Patient { get; set; } = null!;
   
    public virtual SlotMaster Slot { get; set; } = null!;
    
    public virtual Status? Status { get; set; }
}
