using Customer.Api.Services;
using Customer.Shared.Services.Cache;
using Enterprise.Shared.Version;
using HotChocolate;
using HotChocolate.Types;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Customer.Api.GraphQL;

[QueryType]
public class RootQuery(IVersionService versionService)
{
    [UseResolverScope]
    public Version CustomerVersion()
    {
        var version = versionService.GetVersion();

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public async Task<bool> CustomerReadinessSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ICustomerReadinessAccessService customerReadinessAccessService,
        CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetNullableAsync(cancellationToken);
        if (customer is null)
        {
            return false;
        }

        var result = await customerReadinessAccessService.CheckAccessAsync(customer.Id, customer.ProvisionedDomains?.ToList(), cancellationToken);

        return result.IsAllowed;
    }
}
