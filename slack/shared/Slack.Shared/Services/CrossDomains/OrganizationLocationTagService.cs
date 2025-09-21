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

public interface IOrganizationLocationTagService
{
    Task<OrganizationLocationTag> AddAsync(
        string workspaceMemberId,
        OrganizationLocationTag organizationLocationTag,
        CancellationToken cancellationToken);

    Task<OrganizationLocationTag> UpdateAsync(
        string workspaceMemberId,
        OrganizationLocationTag organizationLocationTag,
        CancellationToken cancellationToken);

    Task RemoveAsync(string workspaceMemberId, string locationTagId, CancellationToken cancellationToken);
    Task<OrganizationLocationTag> GetAsync(string workspaceMemberId, string locationTagId, CancellationToken cancellationToken);

    Task<Connection<OrganizationLocationTagEdge>> GetAllLocationTagsAsync(
        string workspaceMemberId,
        string organizationId,
        CancellationToken cancellationToken);

    Task<Connection<OrganizationLocationTagEdge>> GetPaginatedLocationTagsAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken);
}

public class OrganizationLocationTagService(
    ApplicationConfiguration applicationConfiguration,
    OrganizationConfiguration organizationConfiguration,
    Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService.OrganizationServiceClient organizationServiceClient,
    IMapper mapper,
    HybridCache hybridCache) : IOrganizationLocationTagService
{
    public async Task<OrganizationLocationTag> AddAsync(
        string workspaceMemberId,
        OrganizationLocationTag organizationLocationTag,
        CancellationToken cancellationToken)
    {
        var mappedOrganizationLocationTag = mapper.MapTo(
            await organizationServiceClient.AddLocationTagAsync(
                new AddLocationTagInput
                {
                    Id = organizationLocationTag.Id,
                    Name = organizationLocationTag.Name,
                    Description = organizationLocationTag.Description,
                    Color = organizationLocationTag.Color,
                    OrganizationId = organizationLocationTag.Organization.Id
                },
                organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedOrganizationLocationTag], cancellationToken);

        return mappedOrganizationLocationTag;
    }

    public async Task<OrganizationLocationTag> UpdateAsync(
        string workspaceMemberId,
        OrganizationLocationTag organizationLocationTag,
        CancellationToken cancellationToken)
    {
        var mappedOrganizationLocationTag = mapper.MapTo(
            await organizationServiceClient.UpdateLocationTagAsync(
                new UpdateLocationTagInput
                {
                    Id = organizationLocationTag.Id,
                    Name = organizationLocationTag.Name,
                    Description = organizationLocationTag.Description,
                    Color = organizationLocationTag.Color
                },
                organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedOrganizationLocationTag], cancellationToken);

        return mappedOrganizationLocationTag;
    }

    public async Task RemoveAsync(string workspaceMemberId, string locationTagId, CancellationToken cancellationToken)
    {
        await organizationServiceClient.RemoveLocationTagAsync(
            new RemoveLocationTagInput { Id = locationTagId },
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var key = CreateKeyById(locationTagId);

        await hybridCache.RemoveAsync(key, cancellationToken);
    }

    public async Task<OrganizationLocationTag> GetAsync(string workspaceMemberId, string locationTagId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(locationTagId),
            async ct => mapper.MapTo(
                await organizationServiceClient.GetLocationTagAsync(
                    new GetLocationTagInput { Id = locationTagId },
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);

    public async Task<Connection<OrganizationLocationTagEdge>> GetAllLocationTagsAsync(
        string workspaceMemberId,
        string organizationId,
        CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyAllByOrganizationId(organizationId),
            async ct =>
            {
                var getPaginatedLocationTagsInput = new GetPaginatedLocationTagsInput
                {
                    First = ((int?)null).ToNullInt(),
                    After = string.Empty,
                    Before = string.Empty,
                    Last = ((int?)null).ToNullInt(),
                    Where = new LocationTagWhereInput { OrganizationId = organizationId }
                };

                getPaginatedLocationTagsInput.OrderBy.Add(new LocationTagOrderInput
                {
                    Direction = OrderDirection.Ascending, Field = LocationTagOrderField.Name
                });

                var connection = await organizationServiceClient.GetPaginatedLocationTagsAsync(
                    getPaginatedLocationTagsInput,
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct);

                var result = new Connection<OrganizationLocationTagEdge>
                {
                    PageInfo = new PageInfo
                    {
                        StartCursor = connection.PageInfo.StartCursor,
                        EndCursor = connection.PageInfo.EndCursor,
                        HasNextPage = connection.PageInfo.HasNextPage,
                        HasPreviousPage = connection.PageInfo.HasPreviousPage
                    },
                    TotalCount = connection.TotalCount,
                    Edges = connection.Edges.Select(item => new OrganizationLocationTagEdge(mapper.MapTo(item.Node), item.Cursor)).ToList()
                };

                await CacheAsync(result.Edges.Select(item => item.Node).ToList(), ct);

                return result;
            },
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);


    public async Task<Connection<OrganizationLocationTagEdge>> GetPaginatedLocationTagsAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken)
    {
        var getPaginatedLocationTagsInput = new GetPaginatedLocationTagsInput
        {
            First = first.ToNullInt(),
            After = after.ToSafeString(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new LocationTagWhereInput { OrganizationId = organizationId, NameContains = nameContains.ToSafeString() }
        };

        getPaginatedLocationTagsInput.OrderBy.Add(new LocationTagOrderInput
        {
            Direction = OrderDirection.Ascending, Field = LocationTagOrderField.Name
        });

        var connection = await organizationServiceClient.GetPaginatedLocationTagsAsync(
            getPaginatedLocationTagsInput,
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var result = new Connection<OrganizationLocationTagEdge>
        {
            PageInfo = new PageInfo
            {
                StartCursor = connection.PageInfo.StartCursor,
                EndCursor = connection.PageInfo.EndCursor,
                HasNextPage = connection.PageInfo.HasNextPage,
                HasPreviousPage = connection.PageInfo.HasPreviousPage
            },
            TotalCount = connection.TotalCount,
            Edges = connection.Edges.Select(item => new OrganizationLocationTagEdge(mapper.MapTo(item.Node), item.Cursor)).ToList()
        };

        await CacheAsync(result.Edges.Select(item => item.Node).ToList(), cancellationToken);

        return result;
    }

    private async Task CacheAsync(ICollection<OrganizationLocationTag> organizationLocationTags, CancellationToken cancellationToken)
    {
        foreach (var organizationLocationTag in organizationLocationTags)
        {
            var key = CreateKeyById(organizationLocationTag.Id);

            await hybridCache.RemoveAsync(key, cancellationToken);
            await hybridCache.SetAsync(
                key,
                organizationLocationTag,
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
                cancellationToken: cancellationToken);
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-locationtag-id:{id}";

    private string CreateKeyAllByOrganizationId(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-all-locationtags-id:{id}";
}
