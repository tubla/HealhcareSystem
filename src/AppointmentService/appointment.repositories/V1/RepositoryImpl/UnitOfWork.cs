using appointment.models.V1.Context;
using appointment.repositories.V1.Contracts;

namespace appointment.repositories.V1.RepositoryImpl;

public class UnitOfWork(AppointmentDbContext _context) : IUnitOfWork
{
    private IAppointmentRepository? _appointmentRepository;

    public IAppointmentRepository AppointmentRepository
    {
        get
        {
            _appointmentRepository = _appointmentRepository ?? new AppointmentRepository(_context);
            return _appointmentRepository;
        }
    }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
