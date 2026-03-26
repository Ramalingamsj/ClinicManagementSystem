using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Models;

public partial class HospitalManagementDbContext : DbContext
{
    public HospitalManagementDbContext()
    {
    }

    public HospitalManagementDbContext(DbContextOptions<HospitalManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<BillConsultation> BillConsultations { get; set; }

    public virtual DbSet<BillLab> BillLabs { get; set; }

    public virtual DbSet<BillMed> BillMeds { get; set; }

    public virtual DbSet<Consultation> Consultations { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<LabTest> LabTests { get; set; }

    public virtual DbSet<Medicine> Medicines { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<PatientLabTest> PatientLabTests { get; set; }

    public virtual DbSet<PatientMedicine> PatientMedicines { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SlotMaster> SlotMasters { get; set; }

    public virtual DbSet<Specialization> Specializations { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-PQT26L7\\SQLEXPRESS; Initial Catalog = HospitalManagementDB; Integrated Security = True; Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__A50828FC5298286F");

            entity.ToTable("Appointment");

            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.AppointmentDate).HasColumnName("appointment_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.SlotId).HasColumnName("slot_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Appointme__creat__403A8C7D");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__docto__3D5E1FD2");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__patie__3C69FB99");

            entity.HasOne(d => d.Slot).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.SlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Appointme__slot___3E52440B");

            entity.HasOne(d => d.Status).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Appointme__statu__3F466844");
        });

        modelBuilder.Entity<BillConsultation>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__BillCons__D706DDB3B1108D25");

            entity.ToTable("BillConsultation");

            entity.HasIndex(e => e.AppointmentId, "UQ__BillCons__A50828FD12F027AA").IsUnique();

            entity.Property(e => e.BillId).HasColumnName("bill_id");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.Appointment).WithOne(p => p.BillConsultation)
                .HasForeignKey<BillConsultation>(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BillConsu__appoi__59063A47");

            entity.HasOne(d => d.Status).WithMany(p => p.BillConsultations)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__BillConsu__statu__59FA5E80");
        });

        modelBuilder.Entity<BillLab>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__BillLab__D706DDB3D1D7BD77");

            entity.ToTable("BillLab");

            entity.HasIndex(e => e.ConsultationId, "UQ__BillLab__650FE0FADF2DA1DA").IsUnique();

            entity.Property(e => e.BillId).HasColumnName("bill_id");
            entity.Property(e => e.ConsultationId).HasColumnName("consultation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.Consultation).WithOne(p => p.BillLab)
                .HasForeignKey<BillLab>(d => d.ConsultationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BillLab__consult__6477ECF3");

            entity.HasOne(d => d.Status).WithMany(p => p.BillLabs)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__BillLab__status___656C112C");
        });

        modelBuilder.Entity<BillMed>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__BillMed__D706DDB325F768A1");

            entity.ToTable("BillMed");

            entity.HasIndex(e => e.ConsultationId, "UQ__BillMed__650FE0FA80C10461").IsUnique();

            entity.Property(e => e.BillId).HasColumnName("bill_id");
            entity.Property(e => e.ConsultationId).HasColumnName("consultation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.Consultation).WithOne(p => p.BillMed)
                .HasForeignKey<BillMed>(d => d.ConsultationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BillMed__consult__5EBF139D");

            entity.HasOne(d => d.Status).WithMany(p => p.BillMeds)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__BillMed__status___5FB337D6");
        });

        modelBuilder.Entity<Consultation>(entity =>
        {
            entity.HasKey(e => e.ConsultationId).HasName("PK__Consulta__650FE0FB620F5CFF");

            entity.ToTable("Consultation");

            entity.HasIndex(e => e.AppointmentId, "UQ__Consulta__A50828FDD8D53584").IsUnique();

            entity.Property(e => e.ConsultationId).HasColumnName("consultation_id");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Diagnosis)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("diagnosis");
            entity.Property(e => e.DoctorNotes)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("doctor_notes");
            entity.Property(e => e.Symptoms)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("symptoms");

            entity.HasOne(d => d.Appointment).WithOne(p => p.Consultation)
                .HasForeignKey<Consultation>(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Consultat__appoi__44FF419A");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__Doctor__F3993564B27C98E8");

            entity.ToTable("Doctor");

            entity.HasIndex(e => e.UserId, "UQ__Doctor__B9BE370E31FAB9E9").IsUnique();

            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.ConsultationFee)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("consultation_fee");
            entity.Property(e => e.SpecializationId).HasColumnName("specialization_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Specialization).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.SpecializationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Doctor__speciali__33D4B598");

            entity.HasOne(d => d.User).WithOne(p => p.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Doctor__user_id__32E0915F");
        });

        modelBuilder.Entity<LabTest>(entity =>
        {
            entity.HasKey(e => e.LabtestId).HasName("PK__LabTest__FD66A6F0218F02BD");

            entity.ToTable("LabTest");

            entity.Property(e => e.LabtestId).HasColumnName("labtest_id");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.TestName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("test_name");
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.HasKey(e => e.MedicineId).HasName("PK__Medicine__E7148EBBB2DB8183");

            entity.ToTable("Medicine");

            entity.Property(e => e.MedicineId).HasColumnName("medicine_id");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.MedicineName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("medicine_name");
            entity.Property(e => e.MedicineType)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("medicine_type");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patient__4D5CE476E38F9869");

            entity.ToTable("Patient");

            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Contact)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("contact");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("gender");
            entity.Property(e => e.PatientName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("patient_name");
        });

        modelBuilder.Entity<PatientLabTest>(entity =>
        {
            entity.HasKey(e => e.PatientLabtestId).HasName("PK__PatientL__3F103C1C19EA8355");

            entity.ToTable("PatientLabTest");

            entity.Property(e => e.PatientLabtestId).HasColumnName("patient_labtest_id");
            entity.Property(e => e.ConsultationId).HasColumnName("consultation_id");
            entity.Property(e => e.LabtestId).HasColumnName("labtest_id");
            entity.Property(e => e.Result)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("result");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TestDate)
                .HasColumnType("datetime")
                .HasColumnName("test_date");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.Consultation).WithMany(p => p.PatientLabTests)
                .HasForeignKey(d => d.ConsultationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PatientLa__consu__4BAC3F29");

            entity.HasOne(d => d.Labtest).WithMany(p => p.PatientLabTests)
                .HasForeignKey(d => d.LabtestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PatientLa__labte__4CA06362");

            entity.HasOne(d => d.Status).WithMany(p => p.PatientLabTests)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__PatientLa__statu__4D94879B");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.PatientLabTests)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__PatientLa__updat__4E88ABD4");
        });

        modelBuilder.Entity<PatientMedicine>(entity =>
        {
            entity.HasKey(e => e.PatientMedicineId).HasName("PK__PatientM__2214662C083D4840");

            entity.ToTable("PatientMedicine");

            entity.Property(e => e.PatientMedicineId).HasColumnName("patient_medicine_id");
            entity.Property(e => e.ConsultationId).HasColumnName("consultation_id");
            entity.Property(e => e.DurationDays).HasColumnName("duration_days");
            entity.Property(e => e.Frequency).HasColumnName("frequency");
            entity.Property(e => e.IssuedBy).HasColumnName("issued_by");
            entity.Property(e => e.MedicineId).HasColumnName("medicine_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Consultation).WithMany(p => p.PatientMedicines)
                .HasForeignKey(d => d.ConsultationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PatientMe__consu__5165187F");

            entity.HasOne(d => d.IssuedByNavigation).WithMany(p => p.PatientMedicines)
                .HasForeignKey(d => d.IssuedBy)
                .HasConstraintName("FK__PatientMe__issue__5441852A");

            entity.HasOne(d => d.Medicine).WithMany(p => p.PatientMedicines)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PatientMe__medic__52593CB8");

            entity.HasOne(d => d.Status).WithMany(p => p.PatientMedicines)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__PatientMe__statu__534D60F1");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__760965CC8279CEC9");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__783254B12DE55DEB").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<SlotMaster>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("PK__SlotMast__971A01BBDC0BEB97");

            entity.ToTable("SlotMaster");

            entity.Property(e => e.SlotId).HasColumnName("slot_id");
            entity.Property(e => e.SlotTime).HasColumnName("slot_time");
            entity.Property(e => e.TokenNo).HasColumnName("token_no");
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.HasKey(e => e.SpecializationId).HasName("PK__Speciali__0E5BB650D4716AB7");

            entity.ToTable("Specialization");

            entity.HasIndex(e => e.SpecializationName, "UQ__Speciali__A28CFD79352CBC40").IsUnique();

            entity.Property(e => e.SpecializationId).HasColumnName("specialization_id");
            entity.Property(e => e.SpecializationName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("specialization_name");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Status__3683B5314D14E647");

            entity.ToTable("Status");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370FD38E1272");

            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC57238B508AA").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Contact)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("contact");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Experience).HasColumnName("experience");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("gender");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Qualification)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("qualification");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__role_id__2F10007B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
