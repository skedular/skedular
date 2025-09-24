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

public interface IOrganizationTagService
{
    Task<OrganizationTag> AdminGetAsync(string tagId, CancellationToken cancellationToken);
    Task<OrganizationTag> AddAsync(string workspaceMemberId, OrganizationTag organizationTag, CancellationToken cancellationToken);
    Task<OrganizationTag> UpdateAsync(string workspaceMemberId, OrganizationTag organizationTag, CancellationToken cancellationToken);
    Task RemoveAsync(string workspaceMemberId, string tagId, CancellationToken cancellationToken);
    Task<OrganizationTag> GetAsync(string workspaceMemberId, string tagId, CancellationToken cancellationToken);
    Task<Connection<OrganizationTagEdge>> GetAllTagsAsync(string workspaceMemberId, string organizationId, CancellationToken cancellationToken);

    Task<Connection<OrganizationTagEdge>> GetPaginatedTagsAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken);
}

public class OrganizationTagService(
    ApplicationConfiguration applicationConfiguration,
    OrganizationConfiguration organizationConfiguration,
    Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService.OrganizationServiceClient organizationServiceClient,
    IMapper mapper,
    HybridCache hybridCache) : IOrganizationTagService
{
    public async Task<OrganizationTag> AdminGetAsync(string tagId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(tagId),
            async ct => mapper.MapTo(
                await organizationServiceClient.Admin_GetTagAsync(
                    new Admin_GetTagInput { Id = tagId },
                    organizationConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromSeconds(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
            cancellationToken: cancellationToken);

    public async Task<OrganizationTag> AddAsync(
        string workspaceMemberId,
        OrganizationTag organizationTag,
        CancellationToken cancellationToken)
    {
        var mappedOrganizationTag = mapper.MapTo(
            await organizationServiceClient.AddTagAsync(
                new AddTagInput
                {
                    Id = organizationTag.Id,
                    Name = organizationTag.Name,
                    Description = organizationTag.Description,
                    Color = organizationTag.Color,
                    OrganizationId = organizationTag.Organization.Id
                },
                organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedOrganizationTag], cancellationToken);

        return mappedOrganizationTag;
    }

    public async Task<OrganizationTag> UpdateAsync(
        string workspaceMemberId,
        OrganizationTag organizationTag,
        CancellationToken cancellationToken)
    {
        var mappedOrganizationTag = mapper.MapTo(
            await organizationServiceClient.UpdateTagAsync(
                new UpdateTagInput
                {
                    Id = organizationTag.Id, Name = organizationTag.Name, Description = organizationTag.Description, Color = organizationTag.Color
                },
                organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedOrganizationTag], cancellationToken);

        return mappedOrganizationTag;
    }

    public async Task RemoveAsync(string workspaceMemberId, string tagId, CancellationToken cancellationToken)
    {
        await organizationServiceClient.RemoveTagAsync(
            new RemoveTagInput { Id = tagId },
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var key = CreateKeyById(tagId);

        await hybridCache.RemoveAsync(key, cancellationToken);
    }

    public async Task<OrganizationTag> GetAsync(string workspaceMemberId, string tagId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(tagId),
            async ct => mapper.MapTo(
                await organizationServiceClient.GetTagAsync(
                    new GetTagInput { Id = tagId },
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromSeconds(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
            cancellationToken: cancellationToken);

    public async Task<Connection<OrganizationTagEdge>> GetAllTagsAsync(
        string workspaceMemberId,
        string organizationId,
        CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyAllByOrganizationId(organizationId),
            async ct =>
            {
                var getPaginatedTagsInput = new GetPaginatedTagsInput
                {
                    First = ((int?)null).ToNullInt(),
                    After = string.Empty,
                    Before = string.Empty,
                    Last = ((int?)null).ToNullInt(),
                    Where = new TagWhereInput { OrganizationId = organizationId }
                };

                getPaginatedTagsInput.OrderBy.Add(new TagOrderInput { Direction = OrderDirection.Ascending, Field = TagOrderField.Name });

                var connection = await organizationServiceClient.GetPaginatedTagsAsync(
                    getPaginatedTagsInput,
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct);

                var result = new Connection<OrganizationTagEdge>
                {
                    PageInfo = new PageInfo
                    {
                        StartCursor = connection.PageInfo.StartCursor,
                        EndCursor = connection.PageInfo.EndCursor,
                        HasNextPage = connection.PageInfo.HasNextPage,
                        HasPreviousPage = connection.PageInfo.HasPreviousPage
                    },
                    TotalCount = connection.TotalCount,
                    Edges = connection.Edges.Select(item => new OrganizationTagEdge(mapper.MapTo(item.Node), item.Cursor)).ToList()
                };

                await CacheAsync(result.Edges.Select(item => item.Node).ToList(), ct);

                return result;
            },
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromSeconds(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
            cancellationToken: cancellationToken);


    public async Task<Connection<OrganizationTagEdge>> GetPaginatedTagsAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken)
    {
        var getPaginatedTagsInput = new GetPaginatedTagsInput
        {
            First = first.ToNullInt(),
            After = after.ToSafeString(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new TagWhereInput { OrganizationId = organizationId, NameContains = nameContains.ToSafeString() }
        };

        getPaginatedTagsInput.OrderBy.Add(new TagOrderInput { Direction = OrderDirection.Ascending, Field = TagOrderField.Name });

        var connection = await organizationServiceClient.GetPaginatedTagsAsync(
            getPaginatedTagsInput,
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var result = new Connection<OrganizationTagEdge>
        {
            PageInfo = new PageInfo
            {
                StartCursor = connection.PageInfo.StartCursor,
                EndCursor = connection.PageInfo.EndCursor,
                HasNextPage = connection.PageInfo.HasNextPage,
                HasPreviousPage = connection.PageInfo.HasPreviousPage
            },
            TotalCount = connection.TotalCount,
            Edges = connection.Edges.Select(item => new OrganizationTagEdge(mapper.MapTo(item.Node), item.Cursor)).ToList()
        };

        await CacheAsync(result.Edges.Select(item => item.Node).ToList(), cancellationToken);

        return result;
    }

    private async Task CacheAsync(ICollection<OrganizationTag> organizationTags, CancellationToken cancellationToken)
    {
        foreach (var organizationTag in organizationTags)
        {
            var key = CreateKeyById(organizationTag.Id);

            await hybridCache.RemoveAsync(key, cancellationToken);
            await hybridCache.SetAsync(
                key,
                organizationTag,
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromSeconds(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-tag-id:{id}";

    private string CreateKeyAllByOrganizationId(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-all-tags-id:{id}";
}
