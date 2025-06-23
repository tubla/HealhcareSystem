using patient.models.V1.Dto;
using shared.V1.Models;

namespace patient.services.V1.Contracts
{
    public interface IPatientService
    {
        Task<Response<PatientResponseDto>> CreateAsync(CreatePatientRequestDto dto, int userId, CancellationToken cancellationToken = default);
        Task<Response<bool>> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);
        Task<Response<IEnumerable<AppointmentResponseDto>>> GetAppointmentsAsync(int patientId, int userId, CancellationToken cancellationToken = default);
        Task<Response<PatientResponseDto>> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default);
        Task<bool> CheckPatientExistsAsync(int id, int userId, CancellationToken cancellationToken = default);
        Task<Response<PatientResponseDto>> UpdateAsync(int id, UpdatePatientRequestDto dto, int userId, CancellationToken cancellationToken = default);
    }
}