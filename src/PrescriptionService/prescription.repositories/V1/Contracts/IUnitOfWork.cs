namespace prescription.repositories.V1.Contracts;

public interface IUnitOfWork
{
    IPrescriptionRepository PrescriptionsRepository { get; }
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
