namespace shared.V1.HelperClasses.Contracts;

public interface IPatientServiceProxy
{
    Task<bool> CheckPatientExistsAsync(int patientId, CancellationToken cancellationToken);
}
