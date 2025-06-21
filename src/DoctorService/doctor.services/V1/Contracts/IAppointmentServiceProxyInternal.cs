using doctor.models.V1.Dto;

namespace doctor.services.V1.Contracts;

public interface IAppointmentServiceProxyInternal
{
    public Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsAsync(int doctorId, DateTime date, CancellationToken cancellationToken = default);
}
