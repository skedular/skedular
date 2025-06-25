using Enterprise.Shared.Version;
using HotChocolate;
using HotChocolate.Types;
using Payment.Api.Services;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Payment.Api.GraphQL;

[QueryType]
public class Query(IVersionService versionService)
{
    [UseResolverScope]
    public Version PaymentVersion()
    {
        var version = versionService.GetVersion();

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public async Task<bool> PaymentCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);
}
