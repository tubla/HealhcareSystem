using authentication.models.V1.Db;
using Microsoft.EntityFrameworkCore;

namespace authentication.models.V1.Context;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.UserID);
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<User>().HasOne(u => u.Role).WithMany().HasForeignKey(u => u.RoleID);

        modelBuilder.Entity<Role>().HasKey(r => r.RoleID);
        modelBuilder
            .Entity<Role>()
            .HasMany(r => r.Permissions)
            .WithOne(p => p.Role)
            .HasForeignKey(p => p.RoleID);

        modelBuilder.Entity<Permission>().HasKey(p => p.PermissionID);
        modelBuilder
            .Entity<Permission>()
            .HasIndex(p => new { p.PermissionName, p.RoleID })
            .IsUnique();
    }
}
