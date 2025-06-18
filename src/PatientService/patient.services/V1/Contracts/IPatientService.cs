using patient.models.V1.Dto;
using shared.Models;

namespace patient.services.V1.Contracts
{
    public interface IPatientService
    {
        Task<Response<PatientDto>> CreateAsync(CreatePatientDto dto, int userId, CancellationToken cancellationToken = default);
        Task<Response<bool>> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);
        Task<Response<IEnumerable<AppointmentDto>>> GetAppointmentsAsync(int patientId, int userId, CancellationToken cancellationToken = default);
        Task<Response<PatientDto>> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default);
        Task<Response<PatientDto>> UpdateAsync(int id, UpdatePatientDto dto, int userId, CancellationToken cancellationToken = default);
    }
}