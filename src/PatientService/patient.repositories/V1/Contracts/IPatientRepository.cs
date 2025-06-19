using patient.models.V1.Db;

namespace patient.repositories.V1.Contracts;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Patient?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Patient?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    Task<Patient?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task AddAsync(Patient patient, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
