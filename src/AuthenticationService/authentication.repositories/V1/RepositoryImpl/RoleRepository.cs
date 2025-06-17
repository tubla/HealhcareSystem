using authentication.models.V1.Context;
using authentication.models.V1.Db;
using authentication.repositories.V1.Contracts;
using Microsoft.EntityFrameworkCore;

namespace authentication.repositories.V1.RepositoryImpl;

public class RoleRepository(AuthDbContext _context) : IRoleRepository
{
    public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Include(r => r.UserRoles)
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.RoleId == id, cancellationToken);
    }

    public async Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Include(r => r.UserRoles)
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.RoleName == roleName, cancellationToken);
    }

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);

        await _context.Roles.AddAsync(role, cancellationToken);
    }
}
