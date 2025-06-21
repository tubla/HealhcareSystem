using media.repositories.V1.Context;
using media.repositories.V1.Contracts;

namespace media.repositories.V1.Repositories;

public class UnitOfWork(MediaDbContext _context) : IUnitOfWork
{
    private IMediaRepository? _mediaRepository;
    public IMediaRepository MediaRepository => _mediaRepository ??= new MediaRepositoryImpl(_context);

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
