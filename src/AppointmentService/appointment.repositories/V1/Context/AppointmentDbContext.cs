using appointment.models.V1.Db;
using Microsoft.EntityFrameworkCore;

namespace appointment.repositories.V1.Context;

public class AppointmentDbContext(DbContextOptions<AppointmentDbContext> options) : DbContext(options)
{
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<DoctorSlotAvailability> DoctorSlotAvailabilities { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("appointment", "healthcare", t => t.HasComment("Stores appointment information"));
            entity.HasKey(e => e.AppointmentId);

            entity.Property(e => e.AppointmentId)
                .HasColumnName("appointment_id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.PatientId)
                .HasColumnName("patient_id")
                .IsRequired();

            entity.Property(e => e.DoctorId)
                .HasColumnName("doctor_id")
                .IsRequired();

            entity.Property(e => e.AppointmentDateTime)
                .HasColumnName("appointment_datetime")
                .IsRequired();

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Notes)
                .HasColumnName("notes")
                .HasMaxLength(500);
        });

        modelBuilder.Entity<DoctorSlotAvailability>(entity =>
        {
            entity.ToTable("doctor_slot_availability", "healthcare", t => t.HasComment("Tracks doctor appointment slot availability"));
            entity.HasKey(e => e.SlotId);

            entity.Property(e => e.SlotId)
                .HasColumnName("slot_id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.DoctorId)
                .HasColumnName("doctor_id")
                .IsRequired();

            entity.Property(e => e.SlotDate)
                .HasColumnName("slot_date")
                .IsRequired();

            entity.Property(e => e.AppointmentCount)
                .HasColumnName("appointment_count")
                .IsRequired();

            entity.Property(e => e.SlotStatus)
                .HasColumnName("slot_status")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.RowVersion)
                .HasColumnName("row_version")
                .IsRowVersion();

            entity.HasIndex(e => new { e.DoctorId, e.SlotDate })
                .IsUnique();
        });
    }
}
