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
            entity.ToTable("patients", t => t.HasComment("Stores patient information"));
            entity.HasKey(e => e.PatientId);

            entity.Property(e => e.PatientId)
                .HasColumnName("patient_id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.DateOfBirth)
                .HasColumnName("date_of_birth")
                .IsRequired();

            entity.Property(e => e.Gender)
                .HasColumnName("gender")
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(e => e.ContactNumber)
                .HasColumnName("contact_number")
                .HasMaxLength(15);

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(100);

            entity.Property(e => e.Address)
                .HasColumnName("address")
                .HasMaxLength(200);

            entity.HasIndex(e => e.Email)
                .IsUnique();
        });
    }
}
