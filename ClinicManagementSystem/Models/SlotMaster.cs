using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClinicManagementSystem.Models;

public partial class SlotMaster
{
    public int SlotId { get; set; }

    public int TokenNo { get; set; }

    public TimeOnly SlotTime { get; set; }
    [JsonIgnore]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
