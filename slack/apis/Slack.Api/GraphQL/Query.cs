using System.Reflection;
using HotChocolate;
using HotChocolate.Types;
using Slack.Api.Services;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Slack.Api.GraphQL;

[QueryType]
public class Query
{
    [UseResolverScope]
    public Version SlackVersion()
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public async Task<bool> SlackCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);
}
