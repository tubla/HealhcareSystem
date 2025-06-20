using prescription.models.V1.Dto;
using shared.Models;

namespace prescription.services.V1.Contracts;

public interface IMediaServiceProxy
{
    Task<Response<IEnumerable<MediaDto>>> GetAllMediaAsync(IEnumerable<int> mediaIds, CancellationToken cancellationToken);
    Task<Response<MediaDto>> UploadMediaAsync(MultipartFormDataContent content, CancellationToken cancellationToken);
}
