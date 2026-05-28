namespace Customer.Api.Services;

public interface ICustomerSettingsService
{
    Task<Shared.Models.Customer> CompleteOnboardingAsync(CancellationToken cancellationToken);
}

public class CustomerSettingsService(ICustomerHelperService customerHelperService) : ICustomerSettingsService
{
    public async Task<Shared.Models.Customer> CompleteOnboardingAsync(CancellationToken cancellationToken)
    {
        var customer = await customerHelperService.GetCustomerAsync(cancellationToken);
        customer.IsOnboardingDone = true;
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }
}
