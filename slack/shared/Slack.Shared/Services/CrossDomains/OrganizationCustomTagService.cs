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

public interface IOrganizationCustomTagService
{
    Task<OrganizationCustomTag> AddAsync(string workspaceMemberId, OrganizationCustomTag organizationCustomTag, CancellationToken cancellationToken);

    Task<OrganizationCustomTag> UpdateAsync(
        string workspaceMemberId,
        OrganizationCustomTag organizationCustomTag,
        CancellationToken cancellationToken);

    Task RemoveAsync(string workspaceMemberId, string customTagId, CancellationToken cancellationToken);
    Task<OrganizationCustomTag> GetAsync(string workspaceMemberId, string customTagId, CancellationToken cancellationToken);

    Task<Connection<OrganizationCustomTagEdge>> GetAllCustomTagsAsync(
        string workspaceMemberId,
        string organizationId,
        CancellationToken cancellationToken);

    Task<Connection<OrganizationCustomTagEdge>> GetPaginatedCustomTagsAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken);
}

public class OrganizationCustomTagService(
    ApplicationConfiguration applicationConfiguration,
    OrganizationConfiguration organizationConfiguration,
    Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService.OrganizationServiceClient organizationServiceClient,
    IMapper mapper,
    HybridCache hybridCache) : IOrganizationCustomTagService
{
    public async Task<OrganizationCustomTag> AddAsync(
        string workspaceMemberId,
        OrganizationCustomTag organizationCustomTag,
        CancellationToken cancellationToken)
    {
        var mappedOrganizationCustomTag = mapper.MapTo(
            await organizationServiceClient.AddCustomTagAsync(
                new AddCustomTagInput
                {
                    Id = organizationCustomTag.Id,
                    Name = organizationCustomTag.Name,
                    Description = organizationCustomTag.Description,
                    Color = organizationCustomTag.Color,
                    OrganizationId = organizationCustomTag.Organization.Id
                },
                organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedOrganizationCustomTag], cancellationToken);

        return mappedOrganizationCustomTag;
    }

    public async Task<OrganizationCustomTag> UpdateAsync(
        string workspaceMemberId,
        OrganizationCustomTag organizationCustomTag,
        CancellationToken cancellationToken)
    {
        var mappedOrganizationCustomTag = mapper.MapTo(
            await organizationServiceClient.UpdateCustomTagAsync(
                new UpdateCustomTagInput
                {
                    Id = organizationCustomTag.Id,
                    Name = organizationCustomTag.Name,
                    Description = organizationCustomTag.Description,
                    Color = organizationCustomTag.Color
                },
                organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedOrganizationCustomTag], cancellationToken);

        return mappedOrganizationCustomTag;
    }

    public async Task RemoveAsync(string workspaceMemberId, string customTagId, CancellationToken cancellationToken)
    {
        await organizationServiceClient.RemoveCustomTagAsync(
            new RemoveCustomTagInput { Id = customTagId },
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var key = CreateKeyById(customTagId);

        await hybridCache.RemoveAsync(key, cancellationToken);
    }

    public async Task<OrganizationCustomTag> GetAsync(string workspaceMemberId, string customTagId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(customTagId),
            async ct => mapper.MapTo(
                await organizationServiceClient.GetCustomTagAsync(
                    new GetCustomTagInput { Id = customTagId },
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);

    public async Task<Connection<OrganizationCustomTagEdge>> GetAllCustomTagsAsync(
        string workspaceMemberId,
        string organizationId,
        CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyAllByOrganizationId(organizationId),
            async ct =>
            {
                var getPaginatedCustomTagsInput = new GetPaginatedCustomTagsInput
                {
                    First = ((int?)null).ToNullInt(),
                    After = string.Empty,
                    Before = string.Empty,
                    Last = ((int?)null).ToNullInt(),
                    Where = new CustomTagWhereInput { OrganizationId = organizationId }
                };

                getPaginatedCustomTagsInput.OrderBy.Add(new CustomTagOrderInput
                {
                    Direction = OrderDirection.Ascending, Field = CustomTagOrderField.Name
                });

                var connection = await organizationServiceClient.GetPaginatedCustomTagsAsync(
                    getPaginatedCustomTagsInput,
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct);

                var result = new Connection<OrganizationCustomTagEdge>
                {
                    PageInfo = new PageInfo
                    {
                        StartCursor = connection.PageInfo.StartCursor,
                        EndCursor = connection.PageInfo.EndCursor,
                        HasNextPage = connection.PageInfo.HasNextPage,
                        HasPreviousPage = connection.PageInfo.HasPreviousPage
                    },
                    TotalCount = connection.TotalCount,
                    Edges = connection.Edges.Select(item => new OrganizationCustomTagEdge(mapper.MapTo(item.Node), item.Cursor)).ToList()
                };

                await CacheAsync(result.Edges.Select(item => item.Node).ToList(), ct);

                return result;
            },
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);


    public async Task<Connection<OrganizationCustomTagEdge>> GetPaginatedCustomTagsAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken)
    {
        var getPaginatedCustomTagsInput = new GetPaginatedCustomTagsInput
        {
            First = first.ToNullInt(),
            After = after.ToSafeString(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new CustomTagWhereInput { OrganizationId = organizationId, NameContains = nameContains.ToSafeString() }
        };

        getPaginatedCustomTagsInput.OrderBy.Add(new CustomTagOrderInput { Direction = OrderDirection.Ascending, Field = CustomTagOrderField.Name });

        var connection = await organizationServiceClient.GetPaginatedCustomTagsAsync(
            getPaginatedCustomTagsInput,
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var result = new Connection<OrganizationCustomTagEdge>
        {
            PageInfo = new PageInfo
            {
                StartCursor = connection.PageInfo.StartCursor,
                EndCursor = connection.PageInfo.EndCursor,
                HasNextPage = connection.PageInfo.HasNextPage,
                HasPreviousPage = connection.PageInfo.HasPreviousPage
            },
            TotalCount = connection.TotalCount,
            Edges = connection.Edges.Select(item => new OrganizationCustomTagEdge(mapper.MapTo(item.Node), item.Cursor)).ToList()
        };

        await CacheAsync(result.Edges.Select(item => item.Node).ToList(), cancellationToken);

        return result;
    }

    private async Task CacheAsync(ICollection<OrganizationCustomTag> organizationCustomTags, CancellationToken cancellationToken)
    {
        foreach (var organizationCustomTag in organizationCustomTags)
        {
            var key = CreateKeyById(organizationCustomTag.Id);

            await hybridCache.RemoveAsync(key, cancellationToken);
            await hybridCache.SetAsync(
                key,
                organizationCustomTag,
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
                cancellationToken: cancellationToken);
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-customtag-id:{id}";

    private string CreateKeyAllByOrganizationId(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-all-customtags-id:{id}";
}
