using authentication.models.V1.Db;

namespace authentication.repositories.V1.Contracts;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Permission?> GetByNameAsync(string permissionName, CancellationToken cancellationToken = default);
    Task AddAsync(Permission permission, CancellationToken cancellationToken = default);
}
