using Microsoft.EntityFrameworkCore;
using prescription.models.V1.Db;
using prescription.repositories.V1.Context;
using prescription.repositories.V1.Contracts;

namespace prescription.repositories.V1.Repositories;

public class PrescriptionRepositoryImpl(PrescriptionDbContext _context) : IPrescriptionRepository
{
    public async Task<Prescription?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Prescriptions
            .FirstOrDefaultAsync(p => p.PrescriptionId == id, cancellationToken);
    }

    public async Task<IEnumerable<int>> GetMediaIdsAsync(int prescriptionId, CancellationToken cancellationToken = default)
    {
        return await _context.PrescriptionMedia
            .Where(p => p.PrescriptionId == prescriptionId)
            .Select(pm => pm.MediaId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Prescription prescription, CancellationToken cancellationToken = default)
    {
        await _context.Prescriptions.AddAsync(prescription, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var prescription = await GetByIdAsync(id, cancellationToken);
        if (prescription != null)
        {
            _context.Prescriptions.Remove(prescription);
        }
    }
}
