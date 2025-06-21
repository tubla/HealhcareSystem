using department.models.V1.Dto;
using shared.V1.Models;

namespace department.services.V1.Contracts;

public interface IDepartmentsService
{
    Task<Response<DepartmentResponseDto>> CreateAsync(CreateDepartmentRequestDto dto, int userId, CancellationToken cancellationToken = default);
    Task<Response<DepartmentResponseDto>> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<Response<DepartmentResponseDto>> UpdateAsync(int id, UpdateDepartmentRequestDto dto, int userId, CancellationToken cancellationToken = default);
    Task<Response<bool>> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);
}