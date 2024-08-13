using System.Reflection;
using Api.Shared.Services.GraphQL.UnityHub.V1.MsTeams;
using Enterprise.Shared.Context;
using MsTeams.Api.Services;
using Version = Api.Shared.Services.GraphQL.UnityHub.V1.MsTeams.Version;

namespace MsTeams.Api.GraphQL;

public class MsTeamsQuery : Query
{
    public override Task<Version> MsTeamsVersionAsync(IServiceProvider serviceProvider,
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

    public override async Task<bool> MsTeamsCustomerRecordSyncedAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        return await service.DoesCustomerExistAsync(cancellationToken);
    }

    public override async Task<bool> TenantInstalledAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ITenantService>();
        return await service.DoesTenantExistAsync(cancellationToken);
    }

    public override async Task<string> AdminConsentUrlAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ITenantService>();
        return await service.GenerateAdminConsentUrlAsync(cancellationToken);
    }
}
