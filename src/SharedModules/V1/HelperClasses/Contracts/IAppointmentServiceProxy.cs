namespace shared.V1.HelperClasses.Contracts;

public interface IAppointmentServiceProxy
{
    Task<bool> CheckAppointmentExistsAsync(int appointmentId, CancellationToken cancellationToken);
    Task<bool> CheckDoctorHasAppointmentsAsync(int doctorId, CancellationToken cancellationToken);
}
