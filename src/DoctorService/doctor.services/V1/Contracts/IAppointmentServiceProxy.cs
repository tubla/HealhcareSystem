using doctor.models.V1.Dto;

namespace doctor.services.V1.Contracts;

public interface IAppointmentServiceProxy
{
    public Task<IEnumerable<AppointmentDto>> GetAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default);
}
