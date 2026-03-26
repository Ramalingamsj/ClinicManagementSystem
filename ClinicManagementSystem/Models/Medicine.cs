using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClinicManagementSystem.Models;

public partial class Medicine
{
    public int MedicineId { get; set; }

    public string MedicineName { get; set; } = null!;

    public string? MedicineType { get; set; }

    public string? Description { get; set; }

    public int? StockQuantity { get; set; }

    public decimal? Price { get; set; }
    [JsonIgnore]
    public virtual ICollection<PatientMedicine> PatientMedicines { get; set; } = new List<PatientMedicine>();
}
