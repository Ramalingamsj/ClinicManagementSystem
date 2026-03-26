using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClinicManagementSystem.Models;

public partial class Status
{
    public int StatusId { get; set; }

    public string? StatusName { get; set; }
    [JsonIgnore]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    [JsonIgnore]

    public virtual ICollection<BillConsultation> BillConsultations { get; set; } = new List<BillConsultation>();
    [JsonIgnore]

    public virtual ICollection<BillLab> BillLabs { get; set; } = new List<BillLab>();
    [JsonIgnore]

    public virtual ICollection<BillMed> BillMeds { get; set; } = new List<BillMed>();
    [JsonIgnore]

    public virtual ICollection<PatientLabTest> PatientLabTests { get; set; } = new List<PatientLabTest>();
    [JsonIgnore]

    public virtual ICollection<PatientMedicine> PatientMedicines { get; set; } = new List<PatientMedicine>();

}
