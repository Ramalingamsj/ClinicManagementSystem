using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ClinicManagementSystem.Models;

public partial class PatientMedicine
{
    public int PatientMedicineId { get; set; }

    public int ConsultationId { get; set; }

    public int MedicineId { get; set; }

    public int? DurationDays { get; set; }

    public int? StatusId { get; set; }

    public int? IssuedBy { get; set; }

    public int? Frequency { get; set; }

    public int? Quantity { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public virtual Consultation? Consultation { get; set; }

    [ValidateNever]
    public virtual User? IssuedByNavigation { get; set; }

    [ValidateNever]
    public virtual Medicine? Medicine { get; set; }

    public virtual Status? Status { get; set; }
}
