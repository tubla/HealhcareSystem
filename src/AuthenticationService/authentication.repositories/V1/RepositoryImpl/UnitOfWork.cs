using authentication.models.V1.Context;
using authentication.repositories.V1.Contracts;
using Microsoft.EntityFrameworkCore;

namespace authentication.repositories.V1.RepositoryImpl;

public class UnitOfWork(AuthDbContext _context) : IUnitOfWork
{
    private IUserRepository? _userRepository;
    private IRoleRepository? _roleRepository;
    private IPermissionRepository? _permissionRepository;
    public IUserRepository UserRepository
    {
        get
        {
            _userRepository = _userRepository ?? new UserRepository(_context);
            return _userRepository;
        }
    }

    public IRoleRepository RoleRepository
    {
        get
        {
            _roleRepository = _roleRepository ?? new RoleRepository(_context);
            return _roleRepository;
        }
    }

    public IPermissionRepository PermissionRepository
    {
        get
        {
            _permissionRepository = _permissionRepository ?? new PermissionRepository(_context);
            return _permissionRepository;
        }
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await operation();
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
