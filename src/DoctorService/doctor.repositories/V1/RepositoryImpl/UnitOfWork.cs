using doctor.repositories.V1.Context;
using doctor.repositories.V1.Contracts;

namespace doctor.repositories.V1.RepositoryImpl
{
    public class UnitOfWork(DoctorDbContext _context) : IUnitOfWork
    {
        private IDoctorRepository? _doctorRepository;
        public IDoctorRepository DoctorRepository
        {
            get
            {
                return _doctorRepository ??= new DoctorRepositoryImpl(_context);
            }
        }

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
