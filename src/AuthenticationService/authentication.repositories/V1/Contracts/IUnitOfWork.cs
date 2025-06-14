namespace authentication.repositories.V1.Contracts;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    Task<int> CompleteAsync();
}
