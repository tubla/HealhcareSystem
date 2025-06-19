using department.models.V1.Dto;
using shared.Models;

namespace department.services.V1.Contracts;

public interface IDepartmentsService
{
    Task<Response<DepartmentDto>> CreateAsync(CreateDepartmentDto dto, int userId, CancellationToken cancellationToken = default);
    Task<Response<DepartmentDto>> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<Response<DepartmentDto>> UpdateAsync(int id, UpdateDepartmentDto dto, int userId, CancellationToken cancellationToken = default);
    Task<Response<bool>> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);
}