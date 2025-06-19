using doctor.models.V1.Dto;

namespace doctor.services.V1.Contracts;

public interface IDepartmentServiceProxy
{
    public Task<IEnumerable<DepartmentDto>> GetDepartmentAsync(int deptId, CancellationToken cancellationToken = default);
}
