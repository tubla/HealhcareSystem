namespace shared.V1.HelperClasses.Contracts;

public interface IMedicationServiceProxy
{
    Task<bool> CheckMedicationExistsAsync(int medicationId, CancellationToken cancellationToken);
}
