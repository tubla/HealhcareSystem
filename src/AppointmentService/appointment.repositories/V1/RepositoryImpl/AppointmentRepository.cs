using appointment.models.V1.Db;
using appointment.repositories.V1.Context;
using appointment.repositories.V1.Contracts;
using Microsoft.EntityFrameworkCore;

namespace appointment.repositories.V1.RepositoryImpl;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppointmentDbContext _context;
    private readonly int _appointmentSlotDurationMinutes = 60;
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
        var slotDuration = TimeSpan.FromMinutes(_appointmentSlotDurationMinutes);
        var startTime = appointmentDateTime;
        var endTime = appointmentDateTime.Add(slotDuration);

        var slot = await GetDoctorSlotAvailabilityAsync(doctorId, appointmentDateTime.Date, cancellationToken);
        if (slot != null && slot.SlotStatus == "Full")
            return false;

        return !await _context.Appointments
            .Where(a => a.DoctorId == doctorId
                && a.Status != "Cancelled"
                && a.AppointmentDateTime < endTime
                && a.AppointmentDateTime.Add(slotDuration) > startTime
                && (!excludeAppointmentId.HasValue || a.AppointmentId != excludeAppointmentId.Value))
            .AnyAsync(cancellationToken);
    }


    public async Task<DoctorSlotAvailability?> GetDoctorSlotAvailabilityAsync(int doctorId, DateTime slotDate, CancellationToken cancellationToken = default)
    {
        return await _context.DoctorSlotAvailabilities
            .FirstOrDefaultAsync(s => s.DoctorId == doctorId && s.SlotDate == slotDate.Date, cancellationToken);
    }

    public async Task AddOrUpdateDoctorSlotAvailabilityAsync(DoctorSlotAvailability slot, CancellationToken cancellationToken = default)
    {
        var existingSlot = await _context.DoctorSlotAvailabilities
            .FirstOrDefaultAsync(s => s.DoctorId == slot.DoctorId && s.SlotDate == slot.SlotDate.Date, cancellationToken);

        if (existingSlot == null)
        {
            _context.DoctorSlotAvailabilities.Add(slot);
        }
        else
        {
            existingSlot.AppointmentCount = slot.AppointmentCount;
            existingSlot.SlotStatus = slot.SlotStatus;
            _context.Entry(existingSlot).Property(e => e.RowVersion).OriginalValue = slot.RowVersion;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Remove(Appointment appointment)
    {
        if (appointment == null)
            throw new ArgumentNullException(nameof(appointment));

        _context.Appointments.Remove(appointment);
    }
}
