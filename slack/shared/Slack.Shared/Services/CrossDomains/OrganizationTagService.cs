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
    IMemoryCache memoryCache) : IOrganizationTagService
{
    private readonly MemoryCacheEntryOptions _cacheEntryOptions = new() { SlidingExpiration = TimeSpan.FromSeconds(30) };

    public async Task<OrganizationTag> AdminGetAsync(string tagId, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyById(tagId),
            async _ => mapper.MapTo(
                await organizationServiceClient.Admin_GetTagAsync(
                    new Admin_GetTagInput { Id = tagId },
                    organizationConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

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

        Cache([mappedOrganizationTag]);

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

        Cache([mappedOrganizationTag]);

        return mappedOrganizationTag;
    }

    public async Task RemoveAsync(string workspaceMemberId, string tagId, CancellationToken cancellationToken)
    {
        await organizationServiceClient.RemoveTagAsync(
            new RemoveTagInput { Id = tagId },
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var key = CreateKeyById(tagId);

        memoryCache.Remove(key);
    }

    public async Task<OrganizationTag> GetAsync(string workspaceMemberId, string tagId, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyById(tagId),
            async _ => mapper.MapTo(
                await organizationServiceClient.GetTagAsync(
                    new GetTagInput { Id = tagId },
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    public async Task<Connection<OrganizationTagEdge>> GetAllTagsAsync(
        string workspaceMemberId,
        string organizationId,
        CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyAllByOrganizationId(organizationId),
            async _ =>
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

                Cache(result.Edges.Select(item => item.Node).ToList());

                return result;
            },
            _cacheEntryOptions))!;


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

        Cache(result.Edges.Select(item => item.Node).ToList());

        return result;
    }

    private void Cache(IReadOnlyList<OrganizationTag> organizationTags)
    {
        foreach (var organizationTag in organizationTags)
        {
            var key = CreateKeyById(organizationTag.Id);

            memoryCache.Remove(key);
            memoryCache.Set(key, organizationTag, _cacheEntryOptions);
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-tag-id:{id}";

    private string CreateKeyAllByOrganizationId(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-all-tags-id:{id}";
}
