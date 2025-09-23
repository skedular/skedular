using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Hybrid;
using Slack.Shared.Mappers;
using Slack.Shared.Models;

namespace Slack.Shared.Services.CrossDomains;

public interface ILocationPermissionsService
{
    Task<LocationPermissions> GetPermissionsAsync(string workspaceMemberId, string locationId, CancellationToken cancellationToken);
}

public class LocationPermissionsService(
    ApplicationConfiguration applicationConfiguration,
    LocationConfiguration locationConfiguration,
    Api.Shared.Services.Grpc.Skedular.Location.V1.LocationService.LocationServiceClient locationServiceClient,
    IMapper mapper,
    HybridCache hybridCache)
    : ILocationPermissionsService
{
    public async Task<LocationPermissions> GetPermissionsAsync(string workspaceMemberId, string locationId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(workspaceMemberId, locationId),
            async ct => mapper.MapTo(
                await locationServiceClient.GetPermissionsAsync(
                    new GetPermissionsInput { Id = locationId },
                    locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);

    private string CreateKeyById(string workspaceMemberId, string locationId) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:locationpermissions-id:{workspaceMemberId}:{locationId}";
}
