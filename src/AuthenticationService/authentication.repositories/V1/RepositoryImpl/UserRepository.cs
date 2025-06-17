using authentication.models.V1.Context;
using authentication.models.V1.Db;
using authentication.repositories.V1.Contracts;
using Microsoft.EntityFrameworkCore;

namespace authentication.repositories.V1.RepositoryImpl;

public class UserRepository(AuthDbContext _context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
    }

    public async Task<User?> GetByIdWithRolesAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
    }

    public async Task<UserRole?> GetUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task AddUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default)
    {
        if (userRole == null)
            throw new ArgumentNullException(nameof(userRole));

        await _context.UserRoles.AddAsync(userRole, cancellationToken);
    }

    public async Task<bool> HasPermissionAsync(int userId, string permissionName, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission)
            .AnyAsync(p => p.PermissionName == permissionName, cancellationToken);
    }
}
