using doctor.models.V1.Db;

namespace doctor.repositories.V1.Contracts;

public interface IDoctorRepository
{
    Task<Doctor?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Doctor?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Doctor?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Doctor?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Doctor doctor, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
