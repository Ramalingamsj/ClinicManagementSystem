using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClinicManagementSystem.Models;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public int UserId { get; set; }

    public int SpecializationId { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public decimal? ConsultationFee { get; set; }
    [JsonIgnore]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Specialization Specialization { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
