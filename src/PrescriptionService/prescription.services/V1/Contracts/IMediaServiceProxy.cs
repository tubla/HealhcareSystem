using prescription.models.V1.Dto;
using shared.V1.Models;

namespace prescription.services.V1.Contracts;

public interface IMediaServiceProxy
{
    Task<Response<IEnumerable<MediaResponseDto>>> GetAllMediaAsync(IEnumerable<int> mediaIds, CancellationToken cancellationToken);
    Task<Response<MediaResponseDto>> UploadMediaAsync(MultipartFormDataContent content, CancellationToken cancellationToken);
}
