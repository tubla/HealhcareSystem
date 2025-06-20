using Microsoft.AspNetCore.Http;
using prescription.models.V1.Dto;
using shared.Models;

namespace prescription.services.V1.Contracts;

public interface IPrescriptionService
{
    Task<Response<PrescriptionDto>> CreateAsync(CreatePrescriptionDto dto, int userId, CancellationToken cancellationToken = default);
    Task<Response<PrescriptionDto>> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<Response<PrescriptionDto>> UpdateAsync(int id, UpdatePrescriptionDto dto, int userId, CancellationToken cancellationToken = default);
    Task<Response<bool>> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<Response<MediaDto>> UploadMediaAsync(int prescriptionId, IFormFile file, int userId, CancellationToken cancellationToken = default);
    Task<Response<IEnumerable<MediaDto>>> GetMediaAsync(int prescriptionId, int userId, CancellationToken cancellationToken = default);
}
