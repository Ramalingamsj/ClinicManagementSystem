using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClinicManagementSystem.Models;

public partial class Specialization
{
    public int SpecializationId { get; set; }

    public string SpecializationName { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
