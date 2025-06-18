using patient.repositories.V1.Context;
using patient.repositories.V1.Contracts;

namespace patient.repositories.V1.RepositoryImpl
{
    public class UnitOfWork(PatientDbContext _context) : IUnitOfWork
    {
        private IPatientRepository? _patient;

        public IPatientRepository PatientRepository
        {
            get
            {
                _patient ??= new PatientRepositoryImpl(_context);
                return _patient;
            }
        }

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
