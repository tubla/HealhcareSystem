namespace doctor.repositories.V1.Contracts;

public interface IUnitOfWork
{
    IDoctorRepository Doctors { get; }
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
