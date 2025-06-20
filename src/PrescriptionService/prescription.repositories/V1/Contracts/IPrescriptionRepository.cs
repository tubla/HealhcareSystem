using prescription.models.V1.Db;

namespace prescription.repositories.V1.Contracts;

public interface IPrescriptionRepository
{
    Task<Prescription?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<int>> GetMediaIdsAsync(int prescriptionId, CancellationToken cancellationToken = default);
    Task AddAsync(Prescription prescription, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
