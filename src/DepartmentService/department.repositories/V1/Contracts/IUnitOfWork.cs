namespace department.repositories.V1.Contracts;

public interface IUnitOfWork
{
    IDepartmentRepository DepartmentRepository { get; }
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
