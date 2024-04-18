using Api.Shared.Services.Grpc.UnityHub.Location.V1;
using Enterprise.Shared.Grpc;
using Slack.Api.Mappers;
using Slack.Shared.Models;
using Location = Slack.Shared.Models.Location;
using LocationConfiguration = Slack.Shared.Configurations.LocationConfiguration;

namespace Slack.Api.Services;

public interface ILocationService
{
    ValueTask<Location> GetLocationAsync(
        string locationId,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken);

    ValueTask<ICollection<Location>> GetLocationsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken);

    ValueTask<LocationPermissions> GetPermissionsAsync(
        string locationId,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken);
}

public class LocationService(
    LocationConfiguration locationConfiguration,
    IMapper mapper,
    global::Api.Shared.Services.Grpc.UnityHub.Location.V1.LocationService.LocationServiceClient
        locationServiceClient) : ILocationService, IDisposable
{
    private readonly SemaphoreSlim _cachedLocationLock = new(1, 1);
    private readonly SemaphoreSlim _cachedLocationsLock = new(1, 1);
    private readonly SemaphoreSlim _cachedPermissionsLock = new(1, 1);
    private Location? _cachedLocation;
    private ICollection<Location>? _cachedLocations;
    private LocationPermissions? _cachedPermissions;
    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask<Location> GetLocationAsync(
        string locationId,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken)
    {
        if (_cachedLocation is not null)
        {
            return _cachedLocation;
        }

        try
        {
            await _cachedLocationLock.WaitAsync(cancellationToken);
            _cachedLocation = mapper.MapTo(await locationServiceClient.GetAsync(
                new GetInput { Id = locationId },
                locationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                cancellationToken: cancellationToken));

            return _cachedLocation;
        }
        finally
        {
            _cachedLocationLock.Release();
        }
    }

    public async ValueTask<ICollection<Location>> GetLocationsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken)
    {
        if (_cachedLocations is not null)
        {
            return _cachedLocations;
        }

        try
        {
            await _cachedLocationsLock.WaitAsync(cancellationToken);
            var locationConnection = await locationServiceClient.GetPaginatedLocationsAsync(
                new GetPaginatedLocationsInput
                {
                    First = -1,
                    Last = -1,
                    Where = new LocationWhereInput { OrganizationId = workspace.Organization.Id }
                },
                locationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                cancellationToken: cancellationToken);
            _cachedLocations = locationConnection.Edges.Select(item => mapper.MapTo(item.Node)).ToList();

            return _cachedLocations;
        }
        finally
        {
            _cachedLocationsLock.Release();
        }
    }

    public async ValueTask<LocationPermissions> GetPermissionsAsync(
        string locationId,
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
                await locationServiceClient.GetPermissionsAsync(
                    new GetPermissionsInput { Id = locationId },
                    locationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                    cancellationToken: cancellationToken));

            return _cachedPermissions;
        }
        finally
        {
            _cachedPermissionsLock.Release();
        }
    }

    ~LocationService() => Dispose(false);

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _cachedLocationLock.Dispose();
            _cachedLocationsLock.Dispose();
            _cachedPermissionsLock.Dispose();
        }

        _disposed = true;
    }
}
