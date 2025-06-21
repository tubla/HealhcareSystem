using Microsoft.AspNetCore.Http;
using prescription.models.V1.Dto;
using shared.V1.Models;

namespace prescription.services.V1.Contracts;

public interface IPrescriptionService
{
    Task<Response<PrescriptionResponseDto>> CreateAsync(CreatePrescriptionRequestDto dto, int userId, CancellationToken cancellationToken = default);
    Task<Response<PrescriptionResponseDto>> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<Response<PrescriptionResponseDto>> UpdateAsync(int id, UpdatePrescriptionRequestDto dto, int userId, CancellationToken cancellationToken = default);
    Task<Response<bool>> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<Response<MediaResponseDto>> UploadMediaAsync(int prescriptionId, IFormFile file, int userId, CancellationToken cancellationToken = default);
    Task<Response<IEnumerable<MediaResponseDto>>> GetMediaAsync(int prescriptionId, int userId, CancellationToken cancellationToken = default);
}
