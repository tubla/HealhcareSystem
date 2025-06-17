using authentication.models.V1.Context;
using authentication.models.V1.Db;
using authentication.repositories.V1.Contracts;
using Microsoft.EntityFrameworkCore;

namespace authentication.repositories.V1.RepositoryImpl;

public class PermissionRepository(AuthDbContext _context) : IPermissionRepository
{
    public async Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .Include(p => p.RolePermissions)
            .FirstOrDefaultAsync(p => p.PermissionId == id, cancellationToken);
    }

    public async Task<Permission?> GetByNameAsync(string permissionName, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .Include(p => p.RolePermissions)
            .FirstOrDefaultAsync(p => p.PermissionName == permissionName, cancellationToken);
    }

    public async Task AddAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        if (permission == null)
            throw new ArgumentNullException(nameof(permission));

        await _context.Permissions.AddAsync(permission, cancellationToken);
    }
}
