using Api.Shared.Grpc.Skedular.Organization.Core.V1;
using Api.Shared.Grpc.Skedular.Organization.Tags.V1;
using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Pagination;
using Grpc.Core;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;
using OrderDirection = Enterprise.Shared.Pagination.OrderDirection;
using Tag = Api.Shared.Grpc.Skedular.Organization.Core.V1.Tag;
using PageInfo = Api.Shared.Grpc.Skedular.Organization.Core.V1.PageInfo;

namespace Organization.Api.Grpc;

public class OrganizationTagsGrpcService(
    OrganizationConfiguration organizationConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    ITagService tagService,
    IGrpcMapper grpcMapper) : OrganizationTagsService.OrganizationTagsServiceBase
{
    public override async Task<TagConnection> GetPaginatedTags(GetPaginatedTagsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await tagService.GetPaginatedTagsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new TagSearchCriteria(request.Where.OrganizationId, null, request.Where.Types_, request.Where.NameContains),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Grpc.Skedular.Organization.Core.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    TagOrderField.Name => OrganizationTagOrderField.Name,
                    TagOrderField.Description => OrganizationTagOrderField.Description,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new TagOrder(direction, field);
            }).ToList(),
            false,
            context.CancellationToken);

        var connection = new TagConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor.ToSafeString(),
                EndCursor = paginatedInfo.EndCursor.ToSafeString()
            },
            TotalCount = totalCount
        };

        connection.Edges.AddRange(edges.Select(grpcMapper.MapToGrpcResponseTag));
        return connection;
    }

    public override async Task<Tag> Admin_GetTag(Admin_GetTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponseTag(await tagService.GetByIdAsync(request.Id, true, context.CancellationToken));
    }

    public override async Task<TagConnection> Admin_GetPaginatedTags(Admin_GetPaginatedTagsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await tagService.GetPaginatedTagsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new TagSearchCriteria(request.Where.OrganizationId, null, request.Where.Types_, request.Where.NameContains),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Grpc.Skedular.Organization.Core.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    TagOrderField.Name => OrganizationTagOrderField.Name,
                    TagOrderField.Description => OrganizationTagOrderField.Description,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new TagOrder(direction, field);
            }).ToList(),
            true,
            context.CancellationToken);

        var connection = new TagConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor.ToSafeString(),
                EndCursor = paginatedInfo.EndCursor.ToSafeString()
            },
            TotalCount = totalCount
        };

        connection.Edges.AddRange(edges.Select(grpcMapper.MapToGrpcResponseTag));
        return connection;
    }

    public override async Task<Tag> GetTag(GetTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponseTag(await tagService.GetByIdAsync(request.Id, false, context.CancellationToken));
    }

    public override async Task<Tag> AddTag(AddTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponseTag(await tagService.AddAsync(grpcMapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<Tag> UpdateTag(UpdateTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponseTag(await tagService.UpdateAsync(grpcMapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<Tag> RemoveTag(RemoveTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponseTag(await tagService.DeleteAsync(request.Id, context.CancellationToken));
    }

    public override async Task<CustomTagConnection> GetPaginatedCustomTags(GetPaginatedCustomTagsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await tagService.GetPaginatedTagsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new TagSearchCriteria(request.Where.OrganizationId, null, [OrganizationTagTypeConstants.Custom], request.Where.NameContains),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Grpc.Skedular.Organization.Core.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    CustomTagOrderField.Name => OrganizationTagOrderField.Name,
                    CustomTagOrderField.Description => OrganizationTagOrderField.Description,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new TagOrder(direction, field);
            }).ToList(),
            false,
            context.CancellationToken);

        var connection = new CustomTagConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor.ToSafeString(),
                EndCursor = paginatedInfo.EndCursor.ToSafeString()
            },
            TotalCount = totalCount
        };

        connection.Edges.AddRange(edges.Select(grpcMapper.MapToGrpcResponseCustomTag));
        return connection;
    }

    public override async Task<CustomTag> GetCustomTag(GetCustomTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponseCustomTag(await tagService.GetByIdAsync(request.Id, false, context.CancellationToken));
    }

    public override async Task<CustomTag> Admin_GetCustomTag(Admin_GetCustomTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponseCustomTag(await tagService.GetByIdAsync(request.Id, true, context.CancellationToken));
    }

    public override async Task<CustomTag> AddCustomTag(AddCustomTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponseCustomTag(await tagService.AddAsync(grpcMapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<CustomTag> UpdateCustomTag(UpdateCustomTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponseCustomTag(await tagService.UpdateAsync(grpcMapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<CustomTag> RemoveCustomTag(RemoveCustomTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponseCustomTag(await tagService.DeleteAsync(request.Id, context.CancellationToken));
    }

    public override async Task<ProductTagConnection> GetPaginatedProductTags(GetPaginatedProductTagsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await tagService.GetPaginatedTagsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new TagSearchCriteria(request.Where.OrganizationId, null, [OrganizationTagTypeConstants.Product], request.Where.NameContains),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Grpc.Skedular.Organization.Core.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    ProductTagOrderField.Name => OrganizationTagOrderField.Name,
                    ProductTagOrderField.Description => OrganizationTagOrderField.Description,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new TagOrder(direction, field);
            }).ToList(),
            false,
            context.CancellationToken);

        var connection = new ProductTagConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor.ToSafeString(),
                EndCursor = paginatedInfo.EndCursor.ToSafeString()
            },
            TotalCount = totalCount
        };

        connection.Edges.AddRange(edges.Select(grpcMapper.MapToGrpcResponseProductTag));
        return connection;
    }

    public override async Task<ProductTag> GetProductTag(GetProductTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponseProductTag(await tagService.GetByIdAsync(request.Id, false, context.CancellationToken));
    }

    public override async Task<ProductTag> Admin_GetProductTag(Admin_GetProductTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponseProductTag(await tagService.GetByIdAsync(request.Id, true, context.CancellationToken));
    }

    public override async Task<ProductTag> AddProductTag(AddProductTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponseProductTag(await tagService.AddAsync(grpcMapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<ProductTag> UpdateProductTag(UpdateProductTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponseProductTag(await tagService.UpdateAsync(grpcMapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<ProductTag> RemoveProductTag(RemoveProductTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponseProductTag(await tagService.DeleteAsync(request.Id, context.CancellationToken));
    }
}
