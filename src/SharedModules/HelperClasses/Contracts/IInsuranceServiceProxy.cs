namespace shared.HelperClasses.Contracts;

public interface IInsuranceServiceProxy
{
    Task<bool> CheckInsuranceProviderAsync(int insuranceProviderId, CancellationToken cancellationToken = default);
}
