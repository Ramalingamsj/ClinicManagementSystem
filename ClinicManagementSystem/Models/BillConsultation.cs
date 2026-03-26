using System;
using System.Collections.Generic;

namespace ClinicManagementSystem.Models;

public partial class BillConsultation
{
    public int BillId { get; set; }

    public int AppointmentId { get; set; }

    public int? StatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public decimal TotalAmount { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Status? Status { get; set; }
}
