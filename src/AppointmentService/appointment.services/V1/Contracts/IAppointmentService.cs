using appointment.models.V1.Dtos;
using shared.V1.Models;

namespace appointment.services.V1.Contracts;

public interface IAppointmentService
{
    Task<Response<AppointmentResponseDto>> CreateAppointmentAsync(CreateAppointmentRequestDto dto, int userId, CancellationToken cancellationToken = default);
    Task<Response<AppointmentResponseDto>> GetAppointmentAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<Response<AppointmentResponseDto>> GetDoctorAppointmentAsync(int doctorId, int userId, CancellationToken cancellationToken = default);
    Task<Response<IEnumerable<AppointmentResponseDto>>> GetDoctorAppointmentsByDateAsync(
        int doctorId,
        DateTime date,
        int userId,
        CancellationToken cancellationToken = default
    );
    Task<Response<AppointmentResponseDto>> UpdateAppointmentAsync(int id, int userId, UpdateAppointmentRequestDto dto, CancellationToken cancellationToken = default);
    Task<Response<AppointmentResponseDto>> CancelAppointmentAsync(int id, int userId, CancellationToken cancellationToken = default);

    Task<Response<bool>> DeleteAppointmentAsync(
        int id,
        int userId,
        CancellationToken cancellationToken = default);
}
