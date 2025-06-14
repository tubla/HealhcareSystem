using appointment.models.V1.Context;
using appointment.models.V1.Db;
using appointment.repositories.V1.Contracts;
using Microsoft.EntityFrameworkCore;

namespace appointment.repositories.V1.RepositoryImpl;

public class AppointmentRepository(AppointmentDbContext _context) : IAppointmentRepository
{
    public async Task<Appointment?> GetByIdAsync(int id)
    {
        return await _context.Appointments.FindAsync(id);
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
    {
        return await _context.Appointments.Where(a => a.DoctorID == doctorId).ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
    {
        return await _context.Appointments.Where(a => a.PatientID == patientId).ToListAsync();
    }

    public async Task AddAsync(Appointment appointment)
    {
        await _context.Appointments.AddAsync(appointment);
    }

    public async Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime dateTime)
    {
        return !await _context.Appointments.AnyAsync(a =>
            a.DoctorID == doctorId && a.AppointmentDateTime == dateTime && a.Status != "Cancelled"
        );
    }
}
