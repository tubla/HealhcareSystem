using appointment.models.V1.Db;
using Microsoft.EntityFrameworkCore;

namespace appointment.models.V1.Context
{
    public class AppointmentDbContext : DbContext
    {
        public DbSet<Appointment> Appointments { get; set; }

        public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(a => a.AppointmentID);
                entity.HasIndex(a => new { a.DoctorID, a.AppointmentDateTime }).IsUnique();
                entity.Property(a => a.Status).HasConversion<string>();
                entity.ToTable(
                    "CK_Appointment_Status",
                    "Status IN ('Scheduled', 'Completed', 'Cancelled')"
                );
            });
        }
    }
}
