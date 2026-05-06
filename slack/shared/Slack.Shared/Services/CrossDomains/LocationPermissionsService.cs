using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Location.Core.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Memory;
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
    Api.Shared.Grpc.Skedular.Location.Core.V1.LocationService.LocationServiceClient locationServiceClient,
    IMapper mapper,
    IMemoryCache memoryCache)
    : ILocationPermissionsService
{
    private readonly MemoryCacheEntryOptions _cacheEntryOptions = new() { SlidingExpiration = TimeSpan.FromSeconds(30) };

    public async Task<LocationPermissions> GetPermissionsAsync(string workspaceMemberId, string locationId, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyById(workspaceMemberId, locationId),
            async _ => mapper.MapTo(
                await locationServiceClient.GetPermissionsAsync(
                    new GetPermissionsInput { Id = locationId },
                    locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    private string CreateKeyById(string workspaceMemberId, string locationId) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:locationpermissions-id:{workspaceMemberId}:{locationId}";
}
