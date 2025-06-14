using appointment.models.V1.Dtos;
using shared.Models;

namespace appointment.services.V1.Contracts;

public interface IAppointmentService
{
    Task<Response<AppointmentDto>> CreateAppointmentAsync(AppointmentDto dto, int userId);
    Task<Response<AppointmentDto>> GetAppointmentAsync(int id, int userId);
    Task<Response<IEnumerable<AppointmentDto>>> GetDoctorAppointmentsAsync(
        int doctorId,
        int userId
    );
    Task<Response<AppointmentDto>> CancelAppointmentAsync(int id, int userId);
}
