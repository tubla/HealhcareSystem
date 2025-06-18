namespace authentication.repositories.V1.Contracts;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }
    IPermissionRepository PermissionRepository { get; }
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
