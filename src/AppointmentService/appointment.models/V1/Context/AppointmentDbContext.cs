using appointment.models.V1.Db;
using Microsoft.EntityFrameworkCore;

namespace appointment.models.V1.Context;

public class AppointmentDbContext(DbContextOptions<AppointmentDbContext> options) : DbContext(options)
{
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Appointment entity
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId);
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.AppointmentDateTime).HasColumnName("appointment_date_time");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.HasIndex(e => new { e.DoctorId, e.AppointmentDateTime })
                  .IsUnique()
                  .HasDatabaseName("uk_appointment");
        });
    }
}
