using patient.models.V1.Dto;

namespace patient.services.V1.Contracts;

public interface IAppointmentServiceProxy
{
    public Task<IEnumerable<AppointmentDto>> GetAppointmentsAsync(int patientId, CancellationToken cancellationToken = default);
}
