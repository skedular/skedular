using System.Reflection;
using HotChocolate;
using HotChocolate.Types;
using Marketplace.Api.Services;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Marketplace.Api.GraphQL;

[QueryType]
public class Query
{
    [UseResolverScope]
    public Version MarketplaceVersion()
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public async Task<bool> MarketplaceCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);
}
