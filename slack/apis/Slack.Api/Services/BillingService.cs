using Api.Shared.Services.Grpc.UnityHub.Billing.V1;
using Enterprise.Shared.Grpc;
using Slack.Api.Mappers;
using Slack.Shared.Configurations;
using Slack.Shared.Models;

namespace Slack.Api.Services;

public interface IBillingService
{
    ValueTask<OrganizationBillingPermissions> GetPermissionsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken);
}

public class BillingService(
    BillingConfiguration billingConfiguration,
    IMapper mapper,
    global::Api.Shared.Services.Grpc.UnityHub.Billing.V1.BillingService.BillingServiceClient billingServiceClient)
    : IBillingService, IDisposable
{
    private readonly SemaphoreSlim _cachedOrganizationPermissionsLock = new(1, 1);
    private OrganizationBillingPermissions? _cachedOrganizationPermissions;
    private bool _disposed;

    public async ValueTask<OrganizationBillingPermissions>
        GetPermissionsAsync(
            Workspace workspace,
            WorkspaceMember workspaceMember,
            CancellationToken cancellationToken)
    {
        if (_cachedOrganizationPermissions is not null)
        {
            return _cachedOrganizationPermissions;
        }

        try
        {
            await _cachedOrganizationPermissionsLock.WaitAsync(cancellationToken);
            _cachedOrganizationPermissions = mapper.MapTo(
                await billingServiceClient.GetOrganizationPermissionsAsync(
                    new GetOrganizationPermissionsInput { OrganizationId = workspace.Organization.Id },
                    billingConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                    cancellationToken: cancellationToken));

            return _cachedOrganizationPermissions;
        }
        finally
        {
            _cachedOrganizationPermissionsLock.Release();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~BillingService() => Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _cachedOrganizationPermissionsLock.Dispose();
        }

        _disposed = true;
    }
}
