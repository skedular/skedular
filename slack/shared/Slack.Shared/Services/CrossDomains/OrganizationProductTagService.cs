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

public interface IOrganizationProductTagService
{
    Task<OrganizationProductTag> AdminGetAsync(string productTagId, CancellationToken cancellationToken);

    Task<OrganizationProductTag> AddAsync(
        string workspaceMemberId,
        OrganizationProductTag organizationProductTag,
        CancellationToken cancellationToken);

    Task<OrganizationProductTag> UpdateAsync(
        string workspaceMemberId,
        OrganizationProductTag organizationProductTag,
        CancellationToken cancellationToken);

    Task RemoveAsync(string workspaceMemberId, string productTagId, CancellationToken cancellationToken);
    Task<OrganizationProductTag> GetAsync(string workspaceMemberId, string productTagId, CancellationToken cancellationToken);

    Task<Connection<OrganizationProductTagEdge>> GetAllProductTagsAsync(
        string workspaceMemberId,
        string organizationId,
        CancellationToken cancellationToken);

    Task<Connection<OrganizationProductTagEdge>> GetPaginatedProductTagsAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken);
}

public class OrganizationProductTagService(
    ApplicationConfiguration applicationConfiguration,
    OrganizationConfiguration organizationConfiguration,
    Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService.OrganizationServiceClient organizationServiceClient,
    IMapper mapper,
    HybridCache hybridCache) : IOrganizationProductTagService
{
    public async Task<OrganizationProductTag> AdminGetAsync(string productTagId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(productTagId),
            async ct => mapper.MapTo(
                await organizationServiceClient.Admin_GetProductTagAsync(
                    new Admin_GetProductTagInput { Id = productTagId },
                    organizationConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);

    public async Task<OrganizationProductTag> AddAsync(
        string workspaceMemberId,
        OrganizationProductTag organizationProductTag,
        CancellationToken cancellationToken)
    {
        var mappedOrganizationProductTag = mapper.MapTo(
            await organizationServiceClient.AddProductTagAsync(
                new AddProductTagInput
                {
                    Id = organizationProductTag.Id,
                    Name = organizationProductTag.Name,
                    Description = organizationProductTag.Description,
                    Color = organizationProductTag.Color,
                    OrganizationId = organizationProductTag.Organization.Id
                },
                organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedOrganizationProductTag], cancellationToken);

        return mappedOrganizationProductTag;
    }

    public async Task<OrganizationProductTag> UpdateAsync(
        string workspaceMemberId,
        OrganizationProductTag organizationProductTag,
        CancellationToken cancellationToken)
    {
        var mappedOrganizationProductTag = mapper.MapTo(
            await organizationServiceClient.UpdateProductTagAsync(
                new UpdateProductTagInput
                {
                    Id = organizationProductTag.Id,
                    Name = organizationProductTag.Name,
                    Description = organizationProductTag.Description,
                    Color = organizationProductTag.Color
                },
                organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        await CacheAsync([mappedOrganizationProductTag], cancellationToken);

        return mappedOrganizationProductTag;
    }

    public async Task RemoveAsync(string workspaceMemberId, string productTagId, CancellationToken cancellationToken)
    {
        await organizationServiceClient.RemoveProductTagAsync(
            new RemoveProductTagInput { Id = productTagId },
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var key = CreateKeyById(productTagId);

        await hybridCache.RemoveAsync(key, cancellationToken);
    }

    public async Task<OrganizationProductTag> GetAsync(string workspaceMemberId, string productTagId, CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyById(productTagId),
            async ct => mapper.MapTo(
                await organizationServiceClient.GetProductTagAsync(
                    new GetProductTagInput { Id = productTagId },
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct)),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);

    public async Task<Connection<OrganizationProductTagEdge>> GetAllProductTagsAsync(
        string workspaceMemberId,
        string organizationId,
        CancellationToken cancellationToken) =>
        await hybridCache.GetOrCreateAsync(
            CreateKeyAllByOrganizationId(organizationId),
            async ct =>
            {
                var getPaginatedProductTagsInput = new GetPaginatedProductTagsInput
                {
                    First = ((int?)null).ToNullInt(),
                    After = string.Empty,
                    Before = string.Empty,
                    Last = ((int?)null).ToNullInt(),
                    Where = new ProductTagWhereInput { OrganizationId = organizationId }
                };

                getPaginatedProductTagsInput.OrderBy.Add(new ProductTagOrderInput
                {
                    Direction = OrderDirection.Ascending, Field = ProductTagOrderField.Name
                });

                var connection = await organizationServiceClient.GetPaginatedProductTagsAsync(
                    getPaginatedProductTagsInput,
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: ct);

                var result = new Connection<OrganizationProductTagEdge>
                {
                    PageInfo = new PageInfo
                    {
                        StartCursor = connection.PageInfo.StartCursor,
                        EndCursor = connection.PageInfo.EndCursor,
                        HasNextPage = connection.PageInfo.HasNextPage,
                        HasPreviousPage = connection.PageInfo.HasPreviousPage
                    },
                    TotalCount = connection.TotalCount,
                    Edges = connection.Edges.Select(item => new OrganizationProductTagEdge(mapper.MapTo(item.Node), item.Cursor)).ToList()
                };

                await CacheAsync(result.Edges.Select(item => item.Node).ToList(), ct);

                return result;
            },
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
            cancellationToken: cancellationToken);


    public async Task<Connection<OrganizationProductTagEdge>> GetPaginatedProductTagsAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken)
    {
        var getPaginatedProductTagsInput = new GetPaginatedProductTagsInput
        {
            First = first.ToNullInt(),
            After = after.ToSafeString(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new ProductTagWhereInput { OrganizationId = organizationId, NameContains = nameContains.ToSafeString() }
        };

        getPaginatedProductTagsInput.OrderBy.Add(new ProductTagOrderInput
        {
            Direction = OrderDirection.Ascending, Field = ProductTagOrderField.Name
        });

        var connection = await organizationServiceClient.GetPaginatedProductTagsAsync(
            getPaginatedProductTagsInput,
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var result = new Connection<OrganizationProductTagEdge>
        {
            PageInfo = new PageInfo
            {
                StartCursor = connection.PageInfo.StartCursor,
                EndCursor = connection.PageInfo.EndCursor,
                HasNextPage = connection.PageInfo.HasNextPage,
                HasPreviousPage = connection.PageInfo.HasPreviousPage
            },
            TotalCount = connection.TotalCount,
            Edges = connection.Edges.Select(item => new OrganizationProductTagEdge(mapper.MapTo(item.Node), item.Cursor)).ToList()
        };

        await CacheAsync(result.Edges.Select(item => item.Node).ToList(), cancellationToken);

        return result;
    }

    private async Task CacheAsync(ICollection<OrganizationProductTag> organizationProductTags, CancellationToken cancellationToken)
    {
        foreach (var organizationProductTag in organizationProductTags)
        {
            var key = CreateKeyById(organizationProductTag.Id);

            await hybridCache.RemoveAsync(key, cancellationToken);
            await hybridCache.SetAsync(
                key,
                organizationProductTag,
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromMinutes(5) },
                cancellationToken: cancellationToken);
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-producttag-id:{id}";

    private string CreateKeyAllByOrganizationId(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-all-producttags-id:{id}";
}
