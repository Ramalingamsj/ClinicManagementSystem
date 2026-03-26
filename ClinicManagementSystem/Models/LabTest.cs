using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClinicManagementSystem.Models;

public partial class LabTest
{
    public int LabtestId { get; set; }

    public string TestName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? Price { get; set; }
    [JsonIgnore]
    public virtual ICollection<PatientLabTest> PatientLabTests { get; set; } = new List<PatientLabTest>();
}
