using Customer.Shared.Models;
using Customer.Shared.Services.Cache;

namespace Customer.Api.Services;

public interface ICustomerReadinessAccessService
{
    Task<bool> IsReadyAsync(CancellationToken cancellationToken);
}

public class CustomerReadinessAccessService(ICachedCustomerService cachedCustomerService, ILogger<CustomerReadinessAccessService> logger)
    : ICustomerReadinessAccessService
{
    public async Task<bool> IsReadyAsync(CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetNullableAsync(cancellationToken);
        if (customer is null)
        {
            return false;
        }

        var domains = customer.ProvisionedDomains ?? [];
        if (CustomerReadinessState.RequiredDomains.All(domains.Contains))
        {
            logger.LogDebug("Readiness access allowed for customer {CustomerId}", customer.Id);

            return true;
        }

        // If it is not provisioned, clear the cache and read again, also cache it at the same time
        await cachedCustomerService.RemoveAsync([customer], cancellationToken);
        customer = await cachedCustomerService.GetAsync(cancellationToken);

        domains = customer.ProvisionedDomains ?? [];
        if (CustomerReadinessState.RequiredDomains.All(domains.Contains))
        {
            logger.LogDebug("Readiness access allowed for customer {CustomerId}", customer.Id);

            return true;
        }

        var missing = CustomerReadinessState.RequiredDomains.Except(domains).ToList();

        logger.LogInformation("Readiness access blocked for customer {CustomerId}: missing domains {MissingDomains}", customer.Id, missing);

        return false;
    }
}
