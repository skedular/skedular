using Customer.Shared.Models;

namespace Customer.Shared.Services;

public interface IRequiredCustomerReadinessDomainService
{
    IReadOnlySet<string> GetRequiredDomains();
}

public sealed class RequiredCustomerReadinessDomainService : IRequiredCustomerReadinessDomainService
{
    public IReadOnlySet<string> GetRequiredDomains() => CustomerReadinessState.RequiredDomains;
}
