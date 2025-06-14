using appointment.models.V1.Db;

namespace appointment.repositories.V1.Contracts;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(int id);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
    Task AddAsync(Appointment appointment);
    Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime dateTime);
}
