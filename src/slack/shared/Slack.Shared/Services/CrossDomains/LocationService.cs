using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Location.Core.V1;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Memory;
using Slack.Shared.Mappers;
using Location = Slack.Shared.Models.Location;
using LocationEdge = Slack.Shared.Models.LocationEdge;
using LocationType = Api.Shared.Services.Models.LocationType;
using PageInfo = Enterprise.Shared.GraphQL.Types.PageInfo;

namespace Slack.Shared.Services.CrossDomains;

public interface ILocationService
{
    Task<IReadOnlyList<Location>> AdminGetAllLocationsAsync(string organizationId, CancellationToken cancellationToken);
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
    Api.Shared.Grpc.Skedular.Location.Core.V1.LocationService.LocationServiceClient locationServiceClient,
    IGrpcMapper grpcMapper,
    IMemoryCache memoryCache)
    : ILocationService
{
    private readonly MemoryCacheEntryOptions _cacheEntryOptions = new()
    {
        SlidingExpiration = TimeSpan.FromSeconds(30),
    };

    public async Task<IReadOnlyList<Location>> AdminGetAllLocationsAsync(string organizationId, CancellationToken cancellationToken)
    {
        var getPaginatedLocationsInput = new Admin_GetPaginatedLocationsInput
        {
            First = ((int?)null).ToNullInt(),
            After = string.Empty,
            Before = string.Empty,
            Last = ((int?)null).ToNullInt(),
            Where = new LocationWhereInput
            {
                OrganizationId = organizationId,
            },
        };

        getPaginatedLocationsInput.OrderBy.Add(new LocationOrderInput
        {
            Direction = OrderDirection.Ascending,
            Field = LocationOrderField.Name,
        });

        var connection = await locationServiceClient.Admin_GetPaginatedLocationsAsync(
            getPaginatedLocationsInput,
            locationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        var locations = connection.Edges.Select(item => grpcMapper.MapTo(item.Node)).ToList();

        Cache(locations);

        return locations;
    }

    public async Task<Location> AdminGetAsync(string locationId, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyById(locationId),
            async _ => grpcMapper.MapTo(
                await locationServiceClient.Admin_GetAsync(
                    new Admin_GetInput
                    {
                        Id = locationId,
                    },
                    locationConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    public async Task<Location> AdminAddAsync(Location location, CancellationToken cancellationToken)
    {
        var mappedLocation = grpcMapper.MapTo(
            await locationServiceClient.Admin_AddAsync(
                new Admin_AddInput
                {
                    Id = location.Id,
                    Name = location.Name,
                    OrganizationId = location.Organization!.Id,
                    Type = location.Type switch
                    {
                        LocationType.Private => Api.Shared.Grpc.Skedular.Location.Core.V1.LocationType.Private,
                        LocationType.Marketplace => Api.Shared.Grpc.Skedular.Location.Core.V1.LocationType.Marketplace,
                        _ => throw new ArgumentOutOfRangeException(nameof(location.Type), location.Type,
                            $"Unexpected value for {nameof(location.Type)}: {location.Type}. Update enum mapping or caller input."),
                    },
                },
                locationConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken));

        Cache([mappedLocation]);

        return mappedLocation;
    }

    public async Task<Location> GetAsync(string workspaceMemberId, string locationId, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyById(locationId),
            async _ => grpcMapper.MapTo(
                await locationServiceClient.GetAsync(
                    new GetInput
                    {
                        Id = locationId,
                    },
                    locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    public async Task<Location> AddAsync(string workspaceMemberId, Location location, CancellationToken cancellationToken)
    {
        var mappedLocation = grpcMapper.MapTo(
            await locationServiceClient.AddAsync(
                new AddInput
                {
                    Id = location.Id,
                    Name = location.Name,
                    ListingMetadata = new ListingMetadata
                    {
                        About = location.ListingMetadata.About,
                        Title = location.ListingMetadata.Title,
                        SubTitle = location.ListingMetadata.SubTitle,
                    },
                    OrganizationId = location.Organization!.Id,
                    Timezone = location.Timezone,
                    Type = location.Type switch
                    {
                        LocationType.Private => Api.Shared.Grpc.Skedular.Location.Core.V1.LocationType.Private,
                        LocationType.Marketplace => Api.Shared.Grpc.Skedular.Location.Core.V1.LocationType.Marketplace,
                        _ => throw new ArgumentOutOfRangeException(nameof(location.Type), location.Type,
                            $"Unexpected value for {nameof(location.Type)}: {location.Type}. Update enum mapping or caller input."),
                    },
                },
                locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        Cache([mappedLocation]);

        return mappedLocation;
    }

    public async Task<Location> UpdateAsync(string workspaceMemberId, Location location, CancellationToken cancellationToken)
    {
        var mappedLocation = grpcMapper.MapTo(
            await locationServiceClient.UpdateAsync(
                new UpdateInput
                {
                    Id = location.Id,
                    Name = location.Name,
                    ListingMetadata =
                        new ListingMetadata
                        {
                            About = location.ListingMetadata.About,
                            Title = location.ListingMetadata.Title,
                            SubTitle = location.ListingMetadata.SubTitle,
                        },
                    Timezone = location.Timezone,
                    OrganizationId = location.Organization!.Id,
                    FieldsToUpdate =
                    {
                        LocationPatchField.Name,
                        LocationPatchField.ListingMetadata,
                        LocationPatchField.Organization,
                        LocationPatchField.Timezone,
                    },
                },
                locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        Cache([mappedLocation]);

        return mappedLocation;
    }

    public async Task RemoveAsync(string workspaceMemberId, string locationId, CancellationToken cancellationToken)
    {
        await locationServiceClient.RemoveAsync(
            new RemoveInput
            {
                Id = locationId,
            },
            locationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var key = CreateKeyById(locationId);

        memoryCache.Remove(key);
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
            Where = new LocationWhereInput
            {
                OrganizationId = organizationId,
                NameContains = nameContains.ToSafeString(),
            },
        };

        getPaginatedLocationsInput.OrderBy.Add(new LocationOrderInput
        {
            Direction = OrderDirection.Ascending,
            Field = LocationOrderField.Name,
        });

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
                HasPreviousPage = connection.PageInfo.HasPreviousPage,
            },
            TotalCount = connection.TotalCount,
            Edges = [.. connection.Edges.Select(item => new LocationEdge(grpcMapper.MapTo(item.Node), item.Cursor))],
        };

        Cache([.. result.Edges.Select(item => item.Node)]);

        return result;
    }

    private void Cache(IReadOnlyList<Location> locations)
    {
        foreach (var location in locations)
        {
            var key = CreateKeyById(location.Id);

            memoryCache.Remove(key);
            memoryCache.Set(key, location, _cacheEntryOptions);
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:location-id:{id}";
}
