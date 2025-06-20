namespace shared.HelperClasses.Contracts;

public interface IMedicationServiceProxy
{
    Task<bool> CheckMedicationExistsAsync(int medicationId, CancellationToken cancellationToken);
}
