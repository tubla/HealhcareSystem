using authentication.models.V1.Db;

namespace authentication.repositories.V1.Contracts;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
    Task<bool> HasPermissionAsync(int userId, string permissionName);
}
