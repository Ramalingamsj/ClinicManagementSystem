using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClinicManagementSystem.Models;

public partial class PatientLabTest
{
    public int PatientLabtestId { get; set; }

    public int ConsultationId { get; set; }

    public int LabtestId { get; set; }

    public string? Result { get; set; }

    public DateTime? TestDate { get; set; }

    public int? StatusId { get; set; }

    public int? UpdatedBy { get; set; }

    [JsonIgnore]
    public virtual Consultation? Consultation { get; set; }

    [JsonIgnore]
    public virtual LabTest? Labtest { get; set; }

    public virtual Status? Status { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}
