using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Memory;
using Slack.Shared.Mappers;
using Slack.Shared.Models;
using PageInfo = Enterprise.Shared.GraphQL.Types.PageInfo;

namespace Slack.Shared.Services.CrossDomains;

public interface IOrganizationZoneService
{
    Task<OrganizationZone> AdminGetAsync(string zoneId, CancellationToken cancellationToken);
    Task<OrganizationZone> AddAsync(string workspaceMemberId, OrganizationZone organizationZone, CancellationToken cancellationToken);
    Task<OrganizationZone> UpdateAsync(string workspaceMemberId, OrganizationZone organizationZone, CancellationToken cancellationToken);
    Task RemoveAsync(string workspaceMemberId, string zoneId, CancellationToken cancellationToken);
    Task<OrganizationZone> GetAsync(string workspaceMemberId, string zoneId, CancellationToken cancellationToken);
    Task<Connection<OrganizationZoneEdge>> GetAllZonesAsync(string workspaceMemberId, string organizationId, CancellationToken cancellationToken);

    Task<Connection<OrganizationZoneEdge>> GetPaginatedZonesAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken);
}

public class OrganizationZoneService(
    ApplicationConfiguration applicationConfiguration,
    OrganizationConfiguration organizationConfiguration,
    Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService.OrganizationServiceClient organizationServiceClient,
    IMapper mapper,
    IMemoryCache memoryCache) : IOrganizationZoneService
{
    private readonly MemoryCacheEntryOptions _cacheEntryOptions = new() { SlidingExpiration = TimeSpan.FromSeconds(30) };

    public async Task<OrganizationZone> AdminGetAsync(string zoneId, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyById(zoneId),
            async _ => mapper.MapTo(
                await organizationServiceClient.Admin_GetZoneAsync(
                    new Admin_GetZoneInput { Id = zoneId },
                    organizationConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    public async Task<OrganizationZone> AddAsync(string workspaceMemberId, OrganizationZone organizationZone, CancellationToken cancellationToken)
    {
        var mappedOrganizationZone = mapper.MapTo(
            await organizationServiceClient.AddZoneAsync(
                new AddZoneInput
                {
                    Id = organizationZone.Id,
                    Name = organizationZone.Name,
                    Description = organizationZone.Description,
                    Color = organizationZone.Color,
                    OrganizationId = organizationZone.Organization.Id
                },
                organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        Cache([mappedOrganizationZone]);

        return mappedOrganizationZone;
    }

    public async Task<OrganizationZone> UpdateAsync(string workspaceMemberId, OrganizationZone organizationZone, CancellationToken cancellationToken)
    {
        var mappedOrganizationZone = mapper.MapTo(
            await organizationServiceClient.UpdateZoneAsync(
                new UpdateZoneInput
                {
                    Id = organizationZone.Id,
                    Name = organizationZone.Name,
                    Description = organizationZone.Description,
                    Color = organizationZone.Color
                },
                organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        Cache([mappedOrganizationZone]);

        return mappedOrganizationZone;
    }

    public async Task RemoveAsync(string workspaceMemberId, string zoneId, CancellationToken cancellationToken)
    {
        await organizationServiceClient.RemoveZoneAsync(
            new RemoveZoneInput { Id = zoneId },
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var key = CreateKeyById(zoneId);

        memoryCache.Remove(key);
    }

    public async Task<OrganizationZone> GetAsync(string workspaceMemberId, string zoneId, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyById(zoneId),
            async _ => mapper.MapTo(
                await organizationServiceClient.GetZoneAsync(
                    new GetZoneInput { Id = zoneId },
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    public async Task<Connection<OrganizationZoneEdge>> GetAllZonesAsync(
        string workspaceMemberId,
        string organizationId,
        CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyAllByOrganizationId(organizationId),
            async _ =>
            {
                var getPaginatedZonesInput = new GetPaginatedZonesInput
                {
                    First = ((int?)null).ToNullInt(),
                    After = string.Empty,
                    Before = string.Empty,
                    Last = ((int?)null).ToNullInt(),
                    Where = new ZoneWhereInput { OrganizationId = organizationId }
                };

                getPaginatedZonesInput.OrderBy.Add(new ZoneOrderInput { Direction = OrderDirection.Ascending, Field = ZoneOrderField.Name });

                var connection = await organizationServiceClient.GetPaginatedZonesAsync(
                    getPaginatedZonesInput,
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: cancellationToken);

                var result = new Connection<OrganizationZoneEdge>
                {
                    PageInfo = new PageInfo
                    {
                        StartCursor = connection.PageInfo.StartCursor,
                        EndCursor = connection.PageInfo.EndCursor,
                        HasNextPage = connection.PageInfo.HasNextPage,
                        HasPreviousPage = connection.PageInfo.HasPreviousPage
                    },
                    TotalCount = connection.TotalCount,
                    Edges = connection.Edges.Select(item => new OrganizationZoneEdge(mapper.MapTo(item.Node), item.Cursor)).ToList()
                };

                Cache(result.Edges.Select(item => item.Node).ToList());

                return result;
            },
            _cacheEntryOptions))!;


    public async Task<Connection<OrganizationZoneEdge>> GetPaginatedZonesAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken)
    {
        var getPaginatedZonesInput = new GetPaginatedZonesInput
        {
            First = first.ToNullInt(),
            After = after.ToSafeString(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new ZoneWhereInput { OrganizationId = organizationId, NameContains = nameContains.ToSafeString() }
        };

        getPaginatedZonesInput.OrderBy.Add(new ZoneOrderInput { Direction = OrderDirection.Ascending, Field = ZoneOrderField.Name });

        var connection = await organizationServiceClient.GetPaginatedZonesAsync(
            getPaginatedZonesInput,
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var result = new Connection<OrganizationZoneEdge>
        {
            PageInfo = new PageInfo
            {
                StartCursor = connection.PageInfo.StartCursor,
                EndCursor = connection.PageInfo.EndCursor,
                HasNextPage = connection.PageInfo.HasNextPage,
                HasPreviousPage = connection.PageInfo.HasPreviousPage
            },
            TotalCount = connection.TotalCount,
            Edges = connection.Edges.Select(item => new OrganizationZoneEdge(mapper.MapTo(item.Node), item.Cursor)).ToList()
        };

        Cache(result.Edges.Select(item => item.Node).ToList());

        return result;
    }

    private void Cache(IReadOnlyList<OrganizationZone> organizationZones)
    {
        foreach (var organizationZone in organizationZones)
        {
            var key = CreateKeyById(organizationZone.Id);

            memoryCache.Remove(key);
            memoryCache.Set(key, organizationZone, _cacheEntryOptions);
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-zone-id:{id}";

    private string CreateKeyAllByOrganizationId(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-all-zones-id:{id}";
}
