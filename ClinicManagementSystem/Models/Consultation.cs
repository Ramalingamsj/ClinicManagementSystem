using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ClinicManagementSystem.Models;

public partial class Consultation
{
    public int ConsultationId { get; set; }

    public int AppointmentId { get; set; }

    public string? Symptoms { get; set; }

    public string? Diagnosis { get; set; }

    public string? DoctorNotes { get; set; }

    public DateTime? CreatedAt { get; set; }
    
    [JsonIgnore]
    [ValidateNever]
    public virtual Appointment? Appointment { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public virtual BillLab? BillLab { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public virtual BillMed? BillMed { get; set; }
    [ValidateNever]
    public virtual ICollection<PatientLabTest> PatientLabTests { get; set; } = new List<PatientLabTest>();
    [ValidateNever]
    public virtual ICollection<PatientMedicine> PatientMedicines { get; set; } = new List<PatientMedicine>();
}
