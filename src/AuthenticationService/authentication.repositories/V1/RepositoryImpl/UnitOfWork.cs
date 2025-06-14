using authentication.models.V1.Context;
using authentication.repositories.V1.Contracts;

namespace authentication.repositories.V1.RepositoryImpl;

public class UnitOfWork(AuthDbContext _context) : IUnitOfWork
{
    private IUserRepository? _userRepository;
    public IUserRepository Users
    {
        get
        {
            _userRepository = _userRepository ?? new UserRepository(_context);
            return _userRepository;
        }
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
