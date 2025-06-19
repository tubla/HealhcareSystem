using Microsoft.EntityFrameworkCore;
using patient.models.V1.Db;
using patient.repositories.V1.Context;
using patient.repositories.V1.Contracts;

namespace patient.repositories.V1.RepositoryImpl;

public class PatientRepositoryImpl(PatientDbContext _context) : IPatientRepository
{
    public async Task<Patient?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Patients
            .FirstOrDefaultAsync(p => p.PatientId == id, cancellationToken);
    }

    public async Task<Patient?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Patients
            .FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
    }

    public async Task<Patient?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return await _context.Patients
            .FirstOrDefaultAsync(p => p.Phone == phone, cancellationToken);
    }

    public async Task<Patient?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Patients
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        await _context.Patients.AddAsync(patient, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var patient = await GetByIdAsync(id, cancellationToken);
        if (patient != null)
        {
            _context.Patients.Remove(patient);
        }
    }
}
