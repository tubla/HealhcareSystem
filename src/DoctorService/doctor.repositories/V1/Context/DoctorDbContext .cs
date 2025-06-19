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
            entity.ToTable("doctor", "healthcare", t => t.HasComment("Stores doctor information"));
            entity.HasKey(e => e.DoctorId);

            entity.Property(e => e.DoctorId)
                .HasColumnName("doctor_id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.LicenseNumber)
                .HasColumnName("license_number")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Specialization)
                .HasColumnName("specialization")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Phone)
                .HasColumnName("phone")
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(e => e.DeptId)
                .HasColumnName("dept_id")
                .IsRequired();

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");

            entity.HasIndex(e => e.LicenseNumber)
                .IsUnique();
            entity.HasIndex(e => e.Email)
                .IsUnique();
            entity.HasIndex(e => e.UserId)
                .IsUnique();
        });
    }
}
