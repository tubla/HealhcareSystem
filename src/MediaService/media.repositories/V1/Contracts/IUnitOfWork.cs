namespace media.repositories.V1.Contracts;

public interface IUnitOfWork
{
    IMediaRepository MediaRepository { get; }
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
