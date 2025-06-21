namespace shared.V1.HelperClasses.Contracts;

public interface IInsuranceServiceProxy
{
    Task<bool> CheckInsuranceProviderAsync(int insuranceProviderId, CancellationToken cancellationToken = default);
}
