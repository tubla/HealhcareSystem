using authentication.models.V1.Context;
using authentication.models.V1.Db;
using authentication.repositories.V1.Contracts;
using Microsoft.EntityFrameworkCore;

namespace authentication.repositories.V1.RepositoryImpl;

public class UserRepository(AuthDbContext _context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context
            .Users.Include(u => u.Role)
            .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(u => u.UserID == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context
            .Users.Include(u => u.Role)
            .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<bool> HasPermissionAsync(int userId, string permissionName)
    {
        return await _context
            .Users.Where(u => u.UserID == userId)
            .SelectMany(u => u.Role.Permissions)
            .AnyAsync(p => p.PermissionName == permissionName);
    }
}
