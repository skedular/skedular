using Billing.Api.Services;
using Enterprise.Shared.Version;
using HotChocolate;
using HotChocolate.Types;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Billing.Api.GraphQL;

[QueryType]
public class Query(IVersionService versionService)
{
    [UseResolverScope]
    public Version BillingVersion()
    {
        var version = versionService.GetVersion();

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public async Task<bool> BillingCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) => await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);
}
