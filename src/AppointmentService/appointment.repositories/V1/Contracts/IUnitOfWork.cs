namespace appointment.repositories.V1.Contracts;

public interface IUnitOfWork : IDisposable
{
    IAppointmentRepository AppointmentRepository { get; }
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
