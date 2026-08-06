using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Organization.Core.V1;
using Api.Shared.Grpc.Skedular.Organization.Tags.V1;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Caching.Memory;
using Slack.Shared.Mappers;
using Slack.Shared.Models;
using PageInfo = Enterprise.Shared.GraphQL.Types.PageInfo;

namespace Slack.Shared.Services.CrossDomains;

public interface IOrganizationCustomTagService
{
    Task<OrganizationCustomTag> AdminGetAsync(string customTagId, CancellationToken cancellationToken);
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
    OrganizationTagsService.OrganizationTagsServiceClient organizationTagsServiceClient,
    IGrpcMapper grpcMapper,
    IMemoryCache memoryCache) : IOrganizationCustomTagService
{
    private readonly MemoryCacheEntryOptions _cacheEntryOptions = new()
    {
        SlidingExpiration = TimeSpan.FromSeconds(30),
    };

    public async Task<OrganizationCustomTag> AdminGetAsync(string customTagId, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyById(customTagId),
            async _ => grpcMapper.MapTo(
                await organizationTagsServiceClient.Admin_GetCustomTagAsync(
                    new Admin_GetCustomTagInput
                    {
                        Id = customTagId,
                    },
                    organizationConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    public async Task<OrganizationCustomTag> AddAsync(
        string workspaceMemberId,
        OrganizationCustomTag organizationCustomTag,
        CancellationToken cancellationToken)
    {
        var mappedOrganizationCustomTag = grpcMapper.MapTo(
            await organizationTagsServiceClient.AddCustomTagAsync(
                new AddCustomTagInput
                {
                    Id = organizationCustomTag.Id,
                    Name = organizationCustomTag.Name,
                    Description = organizationCustomTag.Description,
                    Color = organizationCustomTag.Color,
                    OrganizationId = organizationCustomTag.Organization.Id,
                },
                organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        Cache([mappedOrganizationCustomTag]);

        return mappedOrganizationCustomTag;
    }

    public async Task<OrganizationCustomTag> UpdateAsync(
        string workspaceMemberId,
        OrganizationCustomTag organizationCustomTag,
        CancellationToken cancellationToken)
    {
        var mappedOrganizationCustomTag = grpcMapper.MapTo(
            await organizationTagsServiceClient.UpdateCustomTagAsync(
                new UpdateTagInput
                {
                    Id = organizationCustomTag.Id,
                    Name = organizationCustomTag.Name,
                    Description = organizationCustomTag.Description,
                    Color = organizationCustomTag.Color,
                    FieldsToUpdate =
                    {
                        TagPatchField.Name,
                        TagPatchField.Description,
                        TagPatchField.Color,
                    },
                },
                organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                cancellationToken: cancellationToken));

        Cache([mappedOrganizationCustomTag]);

        return mappedOrganizationCustomTag;
    }

    public async Task RemoveAsync(string workspaceMemberId, string customTagId, CancellationToken cancellationToken)
    {
        await organizationTagsServiceClient.RemoveCustomTagAsync(
            new RemoveCustomTagInput
            {
                Id = customTagId,
            },
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var key = CreateKeyById(customTagId);

        memoryCache.Remove(key);
    }

    public async Task<OrganizationCustomTag> GetAsync(string workspaceMemberId, string customTagId, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyById(customTagId),
            async _ => grpcMapper.MapTo(
                await organizationTagsServiceClient.GetCustomTagAsync(
                    new GetCustomTagInput
                    {
                        Id = customTagId,
                    },
                    organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
                    cancellationToken: cancellationToken)),
            _cacheEntryOptions))!;

    public async Task<Connection<OrganizationCustomTagEdge>> GetAllCustomTagsAsync(
        string workspaceMemberId,
        string organizationId,
        CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            CreateKeyAllByOrganizationId(organizationId),
            async _ =>
            {
                var getPaginatedCustomTagsInput = new GetPaginatedCustomTagsInput
                {
                    First = ((int?)null).ToNullInt(),
                    After = string.Empty,
                    Before = string.Empty,
                    Last = ((int?)null).ToNullInt(),
                    Where = new CustomTagWhereInput
                    {
                        OrganizationId = organizationId,
                    },
                };

                getPaginatedCustomTagsInput.OrderBy.Add(new CustomTagOrderInput
                {
                    Direction = OrderDirection.Ascending,
                    Field = CustomTagOrderField.Name,
                });

                var connection = await organizationTagsServiceClient.GetPaginatedCustomTagsAsync(
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
                        HasPreviousPage = connection.PageInfo.HasPreviousPage,
                    },
                    TotalCount = connection.TotalCount,
                    Edges = connection.Edges.Select(item => new OrganizationCustomTagEdge(grpcMapper.MapTo(item.Node), item.Cursor)).ToList(),
                };

                Cache(result.Edges.Select(item => item.Node).ToList());

                return result;
            },
            _cacheEntryOptions))!;


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
            Where = new CustomTagWhereInput
            {
                OrganizationId = organizationId,
                NameContains = nameContains.ToSafeString(),
            },
        };

        getPaginatedCustomTagsInput.OrderBy.Add(new CustomTagOrderInput
        {
            Direction = OrderDirection.Ascending,
            Field = CustomTagOrderField.Name,
        });

        var connection = await organizationTagsServiceClient.GetPaginatedCustomTagsAsync(
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
                HasPreviousPage = connection.PageInfo.HasPreviousPage,
            },
            TotalCount = connection.TotalCount,
            Edges = connection.Edges.Select(item => new OrganizationCustomTagEdge(grpcMapper.MapTo(item.Node), item.Cursor)).ToList(),
        };

        Cache(result.Edges.Select(item => item.Node).ToList());

        return result;
    }

    private void Cache(IReadOnlyList<OrganizationCustomTag> organizationCustomTags)
    {
        foreach (var organizationCustomTag in organizationCustomTags)
        {
            var key = CreateKeyById(organizationCustomTag.Id);

            memoryCache.Remove(key);
            memoryCache.Set(key, organizationCustomTag, _cacheEntryOptions);
        }
    }

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-customtag-id:{id}";

    private string CreateKeyAllByOrganizationId(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:crossdomain:organization-all-customtags-id:{id}";
}
