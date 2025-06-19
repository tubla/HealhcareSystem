using Microsoft.EntityFrameworkCore;
using patient.models.V1.Db;

namespace patient.repositories.V1.Context;

public class PatientDbContext(DbContextOptions<PatientDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("patient", "healthcare", t => t.HasComment("Stores patient information"));
            entity.HasKey(e => e.PatientId);

            entity.Property(e => e.PatientId)
                .HasColumnName("patient_id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Dob)
                .HasColumnName("dob")
                .IsRequired();

            entity.Property(e => e.Gender)
                .HasColumnName("gender")
                .HasMaxLength(1)
                .IsRequired();

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Phone)
                .HasColumnName("phone")
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(e => e.Address)
                .HasColumnName("address")
                .HasMaxLength(300);

            entity.Property(e => e.InsuranceProviderId)
                .HasColumnName("insurance_provider_id");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");

            entity.HasIndex(e => e.Email)
                .IsUnique();
            entity.HasIndex(e => e.UserId)
                .IsUnique();
        });
    }
}
