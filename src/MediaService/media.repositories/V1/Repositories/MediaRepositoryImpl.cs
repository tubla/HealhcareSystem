using media.models.V1.Db;
using media.repositories.V1.Context;
using media.repositories.V1.Contracts;
using Microsoft.EntityFrameworkCore;

namespace media.repositories.V1.Repositories;

public class MediaRepositoryImpl(MediaDbContext _context) : IMediaRepository
{

    public async Task<Media?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Media
            .FirstOrDefaultAsync(m => m.MediaId == id, cancellationToken);
    }

    public async Task<IEnumerable<Media>> GetByIdsAsync(IEnumerable<int> mediaIds, CancellationToken cancellationToken = default)
    {
        return await _context.Media
            .Where(m => mediaIds.Contains(m.MediaId))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Media media, CancellationToken cancellationToken = default)
    {
        await _context.Media.AddAsync(media, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var media = await GetByIdAsync(id, cancellationToken);
        if (media != null)
        {
            _context.Media.Remove(media);
        }
    }
}
