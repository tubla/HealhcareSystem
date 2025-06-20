using prescription.repositories.V1.Context;
using prescription.repositories.V1.Contracts;

namespace prescription.repositories.V1.Repositories;

public class UnitOfWork(PrescriptionDbContext _context) : IUnitOfWork
{
    private IPrescriptionRepository? _prescriptionRepository;
    public IPrescriptionRepository Prescriptions
    {
        get
        {
            return _prescriptionRepository ??= new PrescriptionRepositoryImpl(_context);
        }
    }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
