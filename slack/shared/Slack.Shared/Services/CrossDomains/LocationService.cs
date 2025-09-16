using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Hybrid;
using Slack.Shared.Mappers;
using Location = Slack.Shared.Models.Location;

namespace Slack.Shared.Services.CrossDomains;

public interface ILocationService
{
    Task<Location> AdminGetAsync(string locationId, CancellationToken cancellationToken);
    Task<Location> AdminAddAsync(string locationId, string? name, string organizationId, CancellationToken cancellationToken);
    Task<Location> GetAsync(string workspaceMemberId, string locationId, CancellationToken cancellationToken);

    Task<(ICollection<Location>, LocationConnection)> GetPaginatedLocationsAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken);
}

public class LocationService(
    ApplicationConfiguration applicationConfiguration,
    LocationConfiguration locationConfiguration,
    Api.Shared.Services.Grpc.Skedular.Location.V1.LocationService.LocationServiceClient locationServiceClient,
    IMapper mapper,
    HybridCache hybridCache)
    : ILocationService
{
    public async Task<Location> AdminGetAsync(string locationId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(locationId),
            async ct => mapper.MapTo(
                await locationServiceClient.Admin_GetAsync(
                    new Admin_GetInput { Id = locationId },
                    locationConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);

    public async Task<Location> AdminAddAsync(string locationId, string? name, string organizationId, CancellationToken cancellationToken)
    {
        var location = mapper.MapTo(
            await locationServiceClient.Admin_AddAsync(
                new Admin_AddInput { Id = locationId, Name = name, OrganizationId = organizationId, Type = LocationType.Private },
                locationConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken));

        await CacheLocationsAsync([location], cancellationToken);

        return location;
    }

    public async Task<Location> GetAsync(string workspaceMemberId, string locationId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(locationId),
            async ct => mapper.MapTo(
                await locationServiceClient.GetAsync(
                    new GetInput { Id = locationId },
                    locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);

    public async Task<(ICollection<Location>, LocationConnection)> GetPaginatedLocationsAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken)
    {
        var getPaginatedLocationsInput = new GetPaginatedLocationsInput
        {
            First = first.ToNullInt(),
            After = after.ToSafeString(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new LocationWhereInput { OrganizationId = organizationId, NameContains = nameContains.ToSafeString() }
        };

        getPaginatedLocationsInput.OrderBy.Add(new LocationOrderInput { Direction = OrderDirection.Ascending, Field = LocationOrderField.Name });

        var locationsConnection = await locationServiceClient.GetPaginatedLocationsAsync(
            getPaginatedLocationsInput,
            locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var locations = locationsConnection.Edges.Select(item => mapper.MapTo(item.Node)).ToList();

        await CacheLocationsAsync(locations, cancellationToken);

        return (locations, locationsConnection);
    }

    private async Task CacheLocationsAsync(ICollection<Location> locations, CancellationToken cancellationToken)
    {
        foreach (var location in locations)
        {
            var key = CreateKeyById(location.Id);

            await hybridCache.RemoveAsync(key, cancellationToken);
            await hybridCache.SetAsync(
                key,
                location,
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
                cancellationToken: cancellationToken);
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:location-id:{id}";
}
