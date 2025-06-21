using media.models.V1.Db;

namespace media.repositories.V1.Contracts;

public interface IMediaRepository
{
    Task<Media?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Media>> GetByIdsAsync(IEnumerable<int> mediaIds, CancellationToken cancellationToken = default);
    Task AddAsync(Media media, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
