using System.Reflection;
using HotChocolate;
using HotChocolate.Types;
using MsTeams.Api.Services;

namespace MsTeams.Api.GraphQL;

public class MsTeamsQuery
{
    [UseServiceScope]
    public Version MsTeamsVersion()
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        };
    }

    [UseServiceScope]
    public async Task<bool> MsTeamsCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);
}
