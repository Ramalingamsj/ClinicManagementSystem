using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClinicManagementSystem.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Contact { get; set; }

    public string? Email { get; set; }

    public DateOnly? Dob { get; set; }

    public string? Gender { get; set; }

    public int? Experience { get; set; }

    public string? Qualification { get; set; }

    public int RoleId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }
    [JsonIgnore]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    [JsonIgnore]

    public virtual Doctor? Doctor { get; set; }
    [JsonIgnore]
    public virtual ICollection<PatientLabTest> PatientLabTests { get; set; } = new List<PatientLabTest>();
    [JsonIgnore]

    public virtual ICollection<PatientMedicine> PatientMedicines { get; set; } = new List<PatientMedicine>();
    [JsonIgnore]
    public virtual Role Role { get; set; } = null!;
}
