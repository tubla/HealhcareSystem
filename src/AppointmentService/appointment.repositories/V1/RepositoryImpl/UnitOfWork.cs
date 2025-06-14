using appointment.models.V1.Context;
using appointment.repositories.V1.Contracts;

namespace appointment.repositories.V1.RepositoryImpl;

public class UnitOfWork(AppointmentDbContext _context) : IUnitOfWork
{
    private IAppointmentRepository? _appointmentRepository;

    public IAppointmentRepository Appointments
    {
        get
        {
            _appointmentRepository = _appointmentRepository ?? new AppointmentRepository(_context);
            return _appointmentRepository;
        }
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
