using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Hybrid;
using Slack.Shared.Mappers;
using Slack.Shared.Models;
using PageInfo = Enterprise.Shared.GraphQL.Types.PageInfo;

namespace Slack.Shared.Services.CrossDomains;

public interface IOrganizationZoneService
{
    Task<OrganizationZone> AddAsync(
        string workspaceMemberId,
        string zoneId,
        string name,
        string description,
        string organizationId,
        CancellationToken cancellationToken);

    Task<OrganizationZone> UpdateAsync(
        string workspaceMemberId,
        string zoneId,
        string name,
        string description,
        CancellationToken cancellationToken);

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
    HybridCache hybridCache) : IOrganizationZoneService
{
    public async Task<OrganizationZone> AddAsync(
        string workspaceMemberId,
        string zoneId,
        string name,
        string description,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var zone = mapper.MapTo(
            await organizationServiceClient.AddZoneAsync(
                new AddZoneInput { Id = zoneId, Name = name, Description = description, OrganizationId = organizationId },
                organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([zone], cancellationToken);

        return zone;
    }

    public async Task<OrganizationZone> UpdateAsync(
        string workspaceMemberId,
        string zoneId,
        string name,
        string description,
        CancellationToken cancellationToken)
    {
        var zone = mapper.MapTo(
            await organizationServiceClient.UpdateZoneAsync(
                new UpdateZoneInput { Id = zoneId, Name = name, Description = description },
                organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([zone], cancellationToken);

        return zone;
    }

    public async Task RemoveAsync(string workspaceMemberId, string zoneId, CancellationToken cancellationToken)
    {
        await organizationServiceClient.RemoveZoneAsync(
            new RemoveZoneInput { Id = zoneId },
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var key = CreateKeyById(zoneId);

        await hybridCache.RemoveAsync(key, cancellationToken);
    }

    public async Task<OrganizationZone> GetAsync(string workspaceMemberId, string zoneId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(zoneId),
            async ct => mapper.MapTo(
                await organizationServiceClient.GetZoneAsync(
                    new GetZoneInput { Id = zoneId },
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);

    public async Task<Connection<OrganizationZoneEdge>> GetAllZonesAsync(
        string workspaceMemberId,
        string organizationId,
        CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyAllByOrganizationId(organizationId),
            async ct =>
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
                    cancellationToken: ct);

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

                await CacheAsync(result.Edges.Select(item => item.Node).ToList(), ct);

                return result;
            },
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);


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

        await CacheAsync(result.Edges.Select(item => item.Node).ToList(), cancellationToken);

        return result;
    }

    private async Task CacheAsync(ICollection<OrganizationZone> organizationZones, CancellationToken cancellationToken)
    {
        foreach (var organizationZone in organizationZones)
        {
            var key = CreateKeyById(organizationZone.Id);

            await hybridCache.RemoveAsync(key, cancellationToken);
            await hybridCache.SetAsync(
                key,
                organizationZone,
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
                cancellationToken: cancellationToken);
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-zone-id:{id}";

    private string CreateKeyAllByOrganizationId(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-all-zones-id:{id}";
}
