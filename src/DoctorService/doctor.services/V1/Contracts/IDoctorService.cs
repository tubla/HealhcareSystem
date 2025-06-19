using doctor.models.V1.Dto;
using shared.Models;

namespace doctor.services.V1.Contracts;

public interface IDoctorService
{
    Task<Response<DoctorDto>> CreateAsync(CreateDoctorDto dto, int userId, CancellationToken cancellationToken = default);
    Task<Response<DoctorDto>> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<Response<DoctorDto>> UpdateAsync(int id, int userId, UpdateDoctorDto dto, CancellationToken cancellationToken = default);
    Task<Response<bool>> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<bool> CheckDepartmentAsignedAsync(int deptId, int userId, CancellationToken cancellationToken = default);
    Task<Response<IEnumerable<AppointmentDto>>> GetAppointmentsAsync(int doctorId, int userId, CancellationToken cancellationToken = default);
}
