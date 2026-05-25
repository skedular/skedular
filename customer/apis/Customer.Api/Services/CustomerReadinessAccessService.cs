using Customer.Shared.Models;

namespace Customer.Api.Services;

public sealed record CustomerReadinessAccessResult(bool IsAllowed, IReadOnlyList<string> MissingDomains)
{
    public static CustomerReadinessAccessResult Allowed { get; } = new(true, []);
    public static CustomerReadinessAccessResult Blocked(IReadOnlyList<string> missingDomains) => new(false, missingDomains);
}

public interface ICustomerReadinessAccessService
{
    ValueTask<CustomerReadinessAccessResult> CheckAccessAsync(
        string customerId,
        IReadOnlyList<string>? provisionedDomains,
        CancellationToken cancellationToken);
}

public class CustomerReadinessAccessService(ILogger<CustomerReadinessAccessService> logger) : ICustomerReadinessAccessService
{
    public ValueTask<CustomerReadinessAccessResult> CheckAccessAsync(
        string customerId,
        IReadOnlyList<string>? provisionedDomains,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

        var domains = provisionedDomains ?? [];
        if (CustomerReadinessState.RequiredDomains.All(domains.Contains))
        {
            logger.LogInformation("Readiness access allowed for customer {CustomerId}", customerId);

            return ValueTask.FromResult(CustomerReadinessAccessResult.Allowed);
        }

        var missing = CustomerReadinessState.RequiredDomains.Except(domains).ToList();

        logger.LogInformation("Readiness access blocked for customer {CustomerId}: missing domains {MissingDomains}", customerId, missing);

        return ValueTask.FromResult(CustomerReadinessAccessResult.Blocked(missing));
    }
}
