using department.repositories.V1.Context;
using department.repositories.V1.Contracts;

namespace department.repositories.V1.RepositoryImpl;

public class UnitOfWork(DepartmentsDbContext _context) : IUnitOfWork
{
    private IDepartmentRepository? _departments;
    public IDepartmentRepository DepartmentRepository
    {
        get
        {
            return _departments ??= new DepartmentRepositoryImpl(_context);
        }
    }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
