using appointment.models.V1.Db;

namespace appointment.repositories.V1.Contracts;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime dateTime, CancellationToken cancellationToken = default);
}
