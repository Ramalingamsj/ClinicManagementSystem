using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.Models;

public partial class BillLab
{
    public int BillId { get; set; }

    public int ConsultationId { get; set; }

    public decimal? TotalAmount { get; set; }

    public int? StatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Consultation Consultation { get; set; } = null!;

    public virtual Status? Status { get; set; }
}
