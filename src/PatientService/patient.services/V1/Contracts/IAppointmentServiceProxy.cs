using patient.models.V1.Dto;

namespace patient.services.V1.Contracts;

public interface IAppointmentServiceProxyInternal
{
    public Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsAsync(int patientId, CancellationToken cancellationToken = default);
}
