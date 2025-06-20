using Microsoft.EntityFrameworkCore;
using prescription.models.V1.Db;

namespace prescription.repositories.V1.Context;

public class PrescriptionDbContext(DbContextOptions<PrescriptionDbContext> _options) : DbContext(_options)
{
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedia> PrescriptionMedia { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.ToTable("prescription", "healthcare", t => t.HasComment("Stores prescription information"));
            entity.HasKey(e => e.PrescriptionId);

            entity.Property(e => e.PrescriptionId)
                .HasColumnName("prescription_id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.AppointmentId)
                .HasColumnName("appointment_id")
                .IsRequired();

            entity.Property(e => e.MedicationId)
                .HasColumnName("medication_id")
                .IsRequired();

            entity.Property(e => e.Dosage)
                .HasColumnName("dosage")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Duration)
                .HasColumnName("duration")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.IssueDate)
                .HasColumnName("issue_date")
                .IsRequired();
        });

        modelBuilder.Entity<PrescriptionMedia>(entity =>
        {
            entity.ToTable("prescription_media", "healthcare");
            entity.HasKey(e => new { e.PrescriptionId, e.MediaId });

            entity.Property(e => e.PrescriptionId)
                .HasColumnName("prescription_id");

            entity.Property(e => e.MediaId)
                .HasColumnName("media_id");

            entity.HasOne(e => e.Prescription)
                .WithMany(p => p.PrescriptionMedia)
                .HasForeignKey(e => e.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
