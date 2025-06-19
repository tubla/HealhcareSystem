using department.models.V1.Db;
using Microsoft.EntityFrameworkCore;

namespace department.repositories.V1.Context;

public class DepartmentsDbContext(DbContextOptions<DepartmentsDbContext> options) : DbContext(options)
{
    public DbSet<Department> Departments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("department", "healthcare", t => t.HasComment("Stores department information"));
            entity.HasKey(e => e.DeptId);

            entity.Property(e => e.DeptId)
                .HasColumnName("dept_id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(e => e.Name)
                .IsUnique();
        });
    }
}
