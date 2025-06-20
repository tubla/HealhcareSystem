namespace prescription.repositories.V1.Contracts;

public interface IUnitOfWork
{
    IPrescriptionRepository Prescriptions { get; }
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
