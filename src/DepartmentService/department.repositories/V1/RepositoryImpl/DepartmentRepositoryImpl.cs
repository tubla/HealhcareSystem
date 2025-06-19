using department.models.V1.Db;
using department.repositories.V1.Context;
using department.repositories.V1.Contracts;
using Microsoft.EntityFrameworkCore;

namespace department.repositories.V1.RepositoryImpl;

public class DepartmentRepositoryImpl(DepartmentsDbContext _context) : IDepartmentRepository
{
    public async Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(d => d.DeptId == id, cancellationToken);
    }

    public async Task<Department?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(d => d.Name == name, cancellationToken);
    }

    public async Task AddAsync(Department department, CancellationToken cancellationToken = default)
    {
        await _context.Departments.AddAsync(department, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var department = await GetByIdAsync(id, cancellationToken);
        if (department != null)
        {
            _context.Departments.Remove(department);
        }
    }
}
