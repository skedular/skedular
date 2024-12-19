using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Enterprise.Shared.Grpc;
using Slack.Api.Mappers;
using Slack.Shared.Models;
using Organization = Slack.Shared.Models.Organization;
using OrganizationConfiguration = Slack.Shared.Configurations.OrganizationConfiguration;

namespace Slack.Api.Services;

public interface IOrganizationService
{
    ValueTask<Organization> GetOrganizationAsync(
        string organizationId,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken);

    ValueTask<OrganizationPermissions> GetPermissionsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken);
}

public class OrganizationService(
    OrganizationConfiguration organizationConfiguration,
    IMapper mapper,
    global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService.OrganizationServiceClient
        organizationServiceClient) : IOrganizationService, IDisposable
{
    private readonly SemaphoreSlim _cachedOrganizationLock = new(1, 1);
    private readonly SemaphoreSlim _cachedPermissionsLock = new(1, 1);
    private Organization? _cachedOrganization;
    private OrganizationPermissions? _cachedPermissions;
    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask<Organization>
        GetOrganizationAsync(
            string organizationId,
            WorkspaceMember workspaceMember,
            CancellationToken cancellationToken)
    {
        if (_cachedOrganization is not null)
        {
            return _cachedOrganization;
        }

        try
        {
            await _cachedOrganizationLock.WaitAsync(cancellationToken);
            _cachedOrganization = mapper.MapTo(await organizationServiceClient.GetAsync(
                new GetInput { Id = organizationId },
                organizationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                cancellationToken: cancellationToken));

            return _cachedOrganization;
        }
        finally
        {
            _cachedOrganizationLock.Release();
        }
    }

    public async ValueTask<OrganizationPermissions>
        GetPermissionsAsync(
            Workspace workspace,
            WorkspaceMember workspaceMember,
            CancellationToken cancellationToken)
    {
        if (_cachedPermissions is not null)
        {
            return _cachedPermissions;
        }

        try
        {
            await _cachedPermissionsLock.WaitAsync(cancellationToken);
            _cachedPermissions = mapper.MapTo(
                await organizationServiceClient.GetPermissionsAsync(
                    new GetPermissionsInput { Id = workspace.Organization.Id },
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                    cancellationToken: cancellationToken));

            return _cachedPermissions;
        }
        finally
        {
            _cachedPermissionsLock.Release();
        }
    }

    ~OrganizationService() => Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _cachedOrganizationLock.Dispose();
            _cachedPermissionsLock.Dispose();
        }

        _disposed = true;
    }
}
