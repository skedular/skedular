using Enterprise.Shared.Version;
using HotChocolate;
using HotChocolate.Types;
using Notification.Api.Services;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Notification.Api.GraphQL;

[QueryType]
public class RootQuery(IVersionService versionService)
{
    [UseResolverScope]
    public Version NotificationVersion()
    {
        var version = versionService.GetVersion();

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public async Task<bool> NotificationCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);
}
