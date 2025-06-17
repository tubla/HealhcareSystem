namespace authentication.repositories.V1.Contracts;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IPermissionRepository Permissions { get; }
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
