using authentication.models.V1.Db;

namespace authentication.repositories.V1.Contracts;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithRolesAsync(int userId, CancellationToken cancellationToken = default);
    Task<UserRole?> GetUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task AddUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default);
    Task<bool> HasPermissionAsync(int userId, string permissionName, CancellationToken cancellationToken = default);
}
