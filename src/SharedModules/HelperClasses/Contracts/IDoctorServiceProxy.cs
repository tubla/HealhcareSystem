namespace shared.HelperClasses.Contracts;

public interface IDoctorServiceProxy
{
    Task<bool> CheckDoctorAssigned(int deptId, CancellationToken cancellationToken);
}
