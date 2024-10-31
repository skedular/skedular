using System.Reflection;
using Api.Shared.Services.GraphQL.UnityHub.V1.Slack;
using Enterprise.Shared.Context;
using Slack.Api.Services;
using Version = Api.Shared.Services.GraphQL.UnityHub.V1.Slack.Version;

namespace Slack.Api.GraphQL;

public class SlackQuery : Query
{
    public override Task<Version> SlackVersionAsync(
        IServiceProvider serviceProvider,
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

    public override async Task<bool> SlackCustomerRecordSyncedAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        return await service.DoesCustomerExistAsync(cancellationToken);
    }
}
