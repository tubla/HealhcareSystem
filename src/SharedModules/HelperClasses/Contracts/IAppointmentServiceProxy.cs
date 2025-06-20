namespace shared.HelperClasses.Contracts;

public interface IAppointmentServiceProxy
{
    Task<bool> CheckAppointmentExistsAsync(int appointmentId, CancellationToken cancellationToken);
}
