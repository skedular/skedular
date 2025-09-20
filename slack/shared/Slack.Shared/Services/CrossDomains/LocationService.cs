using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Hybrid;
using Slack.Shared.Mappers;
using Location = Slack.Shared.Models.Location;
using LocationEdge = Slack.Shared.Models.LocationEdge;
using LocationType = Api.Shared.Services.Models.LocationType;
using PageInfo = Enterprise.Shared.GraphQL.Types.PageInfo;

namespace Slack.Shared.Services.CrossDomains;

public interface ILocationService
{
    Task<Location> AdminGetAsync(string locationId, CancellationToken cancellationToken);
    Task<Location> AdminAddAsync(Location location, CancellationToken cancellationToken);
    Task<Location> GetAsync(string workspaceMemberId, string locationId, CancellationToken cancellationToken);
    Task<Location> AddAsync(string workspaceMemberId, Location location, CancellationToken cancellationToken);
    Task<Location> UpdateAsync(string workspaceMemberId, Location location, CancellationToken cancellationToken);
    Task RemoveAsync(string workspaceMemberId, string locationId, CancellationToken cancellationToken);

    Task<Connection<LocationEdge>> GetPaginatedLocationsAsync(
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

    public async Task<Location> AdminAddAsync(Location location, CancellationToken cancellationToken)
    {
        var mappedLocation = mapper.MapTo(
            await locationServiceClient.Admin_AddAsync(
                new Admin_AddInput
                {
                    Id = location.Id,
                    Name = location.Name,
                    OrganizationId = location.Organization!.Id,
                    Type = Api.Shared.Services.Grpc.Skedular.Location.V1.LocationType.Private
                },
                locationConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedLocation], cancellationToken);

        return mappedLocation;
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

    public async Task<Location> AddAsync(string workspaceMemberId, Location location, CancellationToken cancellationToken)
    {
        var mappedLocation = mapper.MapTo(
            await locationServiceClient.AddAsync(
                new AddInput
                {
                    Id = location.Id,
                    Name = location.Name,
                    About = location.About,
                    OrganizationId = location.Organization!.Id,
                    Timezone = location.Timezone,
                    Type = location.Type switch
                    {
                        LocationType.Private => Api.Shared.Services.Grpc.Skedular.Location.V1.LocationType.Private,
                        LocationType.Marketplace => Api.Shared.Services.Grpc.Skedular.Location.V1.LocationType.Marketplace,
                        _ => throw new ArgumentOutOfRangeException()
                    }
                },
                locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedLocation], cancellationToken);

        return mappedLocation;
    }


    public async Task<Location> UpdateAsync(string workspaceMemberId, Location location, CancellationToken cancellationToken)
    {
        var mappedLocation = mapper.MapTo(
            await locationServiceClient.UpdateAsync(
                new UpdateInput
                {
                    Id = location.Id,
                    Name = location.Name,
                    About = location.About,
                    Timezone = location.Timezone,
                    OrganizationId = location.Organization!.Id,
                    Type = location.Type switch
                    {
                        LocationType.Private => Api.Shared.Services.Grpc.Skedular.Location.V1.LocationType.Private,
                        LocationType.Marketplace => Api.Shared.Services.Grpc.Skedular.Location.V1.LocationType.Marketplace,
                        _ => throw new ArgumentOutOfRangeException()
                    }
                },
                locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedLocation], cancellationToken);

        return mappedLocation;
    }

    public async Task RemoveAsync(string workspaceMemberId, string locationId, CancellationToken cancellationToken)
    {
        await locationServiceClient.RemoveAsync(
            new RemoveInput { Id = locationId },
            locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var key = CreateKeyById(locationId);

        await hybridCache.RemoveAsync(key, cancellationToken);
    }

    public async Task<Connection<LocationEdge>> GetPaginatedLocationsAsync(
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

        var connection = await locationServiceClient.GetPaginatedLocationsAsync(
            getPaginatedLocationsInput,
            locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var result = new Connection<LocationEdge>
        {
            PageInfo = new PageInfo
            {
                StartCursor = connection.PageInfo.StartCursor,
                EndCursor = connection.PageInfo.EndCursor,
                HasNextPage = connection.PageInfo.HasNextPage,
                HasPreviousPage = connection.PageInfo.HasPreviousPage
            },
            TotalCount = connection.TotalCount,
            Edges = connection.Edges.Select(item => new LocationEdge(mapper.MapTo(item.Node), item.Cursor)).ToList()
        };

        await CacheAsync(result.Edges.Select(item => item.Node).ToList(), cancellationToken);

        return result;
    }

    private async Task CacheAsync(ICollection<Location> locations, CancellationToken cancellationToken)
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
