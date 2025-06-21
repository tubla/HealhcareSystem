using doctor.models.V1.Dto;
using shared.V1.Models;

namespace doctor.services.V1.Contracts;

public interface IDoctorService
{
    Task<Response<DoctorResponseDto>> CreateAsync(CreateDoctorRequestDto dto, int userId, CancellationToken cancellationToken = default);
    Task<Response<DoctorResponseDto>> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<Response<DoctorResponseDto>> UpdateAsync(int id, int userId, UpdateDoctorRequestDto dto, CancellationToken cancellationToken = default);
    Task<Response<bool>> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<bool> CheckDepartmentAsignedAsync(int deptId, int userId, CancellationToken cancellationToken = default);
    Task<Response<IEnumerable<AppointmentResponseDto>>> GetAppointmentsAsync(int doctorId, DateTime date, int userId, CancellationToken cancellationToken = default);
}
