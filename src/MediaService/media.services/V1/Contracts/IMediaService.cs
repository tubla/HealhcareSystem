using media.models.V1.Dto;
using shared.V1.Models;

namespace media.services.V1.Contracts;

public interface IMediaService
{
    Task<Response<MediaResponseDto>> UploadAsync(MultipartFormDataContent content, int userId, CancellationToken cancellationToken = default);
    Task<Response<IEnumerable<MediaResponseDto>>> GetByIdsAsync(IEnumerable<int> mediaIds, int userId, CancellationToken cancellationToken = default);
}
