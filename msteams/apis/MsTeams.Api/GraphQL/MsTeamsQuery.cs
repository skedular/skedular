using System.Reflection;
using Api.Shared.Services.GraphQL.UnityHub.V1.MsTeams;
using Version = Api.Shared.Services.GraphQL.UnityHub.V1.MsTeams.Version;

namespace MsTeams.Api.GraphQL;

public class MsTeamsQuery : Query
{
    public override Task<Version> MsteamsVersionAsync(IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return Task.FromResult(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public override Task<bool> MsteamsCustomerRecordSyncedAsync(IServiceProvider serviceProvider,
        CancellationToken cancellationToken) => throw new NotImplementedException();
}
