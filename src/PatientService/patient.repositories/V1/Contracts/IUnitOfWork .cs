namespace patient.repositories.V1.Contracts;

public interface IUnitOfWork
{
    IPatientRepository PatientRepository { get; }
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
