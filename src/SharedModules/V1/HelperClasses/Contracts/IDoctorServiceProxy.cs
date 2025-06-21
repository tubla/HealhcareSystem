namespace shared.V1.HelperClasses.Contracts;

public interface IDoctorServiceProxy
{
    Task<bool> CheckDoctorAssigned(int deptId, CancellationToken cancellationToken);
}
