using appointment.models.V1.Db;
using appointment.repositories.V1.Context;
using appointment.repositories.V1.Contracts;
using Microsoft.EntityFrameworkCore;

namespace appointment.repositories.V1.RepositoryImpl;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppointmentDbContext _context;

    public AppointmentRepository(AppointmentDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .FirstOrDefaultAsync(a => a.AppointmentId == id, cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAndDateAsync(int doctorId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .Where(a => a.DoctorId == doctorId && a.AppointmentDateTime == date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .Where(a => a.DoctorId == doctorId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .Where(a => a.PatientId == patientId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        if (appointment == null)
            throw new ArgumentNullException(nameof(appointment));

        await _context.Appointments.AddAsync(appointment, cancellationToken);
    }

    public async Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime appointmentDateTime, int? excludeAppointmentId = null, CancellationToken cancellationToken = default)
    {
        var slotDuration = TimeSpan.FromMinutes(30);
        var startTime = appointmentDateTime;
        var endTime = appointmentDateTime.Add(slotDuration);

        return !await _context.Appointments
            .Where(a => a.DoctorId == doctorId
                && a.Status != "Cancelled"
                && a.AppointmentDateTime < endTime
                && a.AppointmentDateTime.Add(slotDuration) > startTime
                && (!excludeAppointmentId.HasValue || a.AppointmentId != excludeAppointmentId.Value))
            .AnyAsync(cancellationToken);
    }

    public void Remove(Appointment appointment)
    {
        if (appointment == null)
            throw new ArgumentNullException(nameof(appointment));

        _context.Appointments.Remove(appointment);
    }
}
