using doctor.models.V1.Db;
using Microsoft.EntityFrameworkCore;

namespace doctor.repositories.V1.Context;

public class DoctorDbContext(DbContextOptions<DoctorDbContext> _options) : DbContext(_options)
{
    public DbSet<Doctor> Doctors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("doctors", t => t.HasComment("Stores doctor information"));
            entity.HasKey(e => e.DoctorId);

            entity.Property(e => e.DoctorId)
                .HasColumnName("doctor_id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Specialization)
                .HasColumnName("specialization")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.LicenseNumber)
                .HasColumnName("license_number")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.ContactNumber)
                .HasColumnName("contact_number")
                .HasMaxLength(15);

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(100);

            entity.Property(e => e.HospitalAffiliation)
                .HasColumnName("hospital_affiliation")
                .HasMaxLength(100);

            entity.HasIndex(e => e.LicenseNumber)
                .IsUnique();
            entity.HasIndex(e => e.Email)
                .IsUnique();
        });
    }
}
