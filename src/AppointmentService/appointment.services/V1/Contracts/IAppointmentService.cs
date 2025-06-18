using appointment.models.V1.Dtos;
using shared.Models;

namespace appointment.services.V1.Contracts;

public interface IAppointmentService
{
    Task<Response<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto dto, int userId, CancellationToken cancellationToken = default);
    Task<Response<AppointmentDto>> GetAppointmentAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<Response<IEnumerable<AppointmentDto>>> GetDoctorAppointmentsAsync(
        int doctorId,
        int userId,
        CancellationToken cancellationToken = default
    );
    Task<Response<AppointmentDto>> CancelAppointmentAsync(int id, int userId, CancellationToken cancellationToken = default);
}
