using Api.Shared.Grpc.Skedular.Location.Resources.V1;
using Api.Shared.Services.Configurations.Grpc;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Pagination;
using Grpc.Core;
using Location.Api.Mappers;
using Location.Api.Models;
using Location.Api.Services;
using Location.Shared.Models;
using OrderDirection = Enterprise.Shared.Pagination.OrderDirection;
using PageInfo = Api.Shared.Grpc.Skedular.Location.Core.V1.PageInfo;
using Resource = Api.Shared.Grpc.Skedular.Location.Core.V1.Resource;
using ResourceOrderField = Api.Shared.Grpc.Skedular.Location.Resources.V1.ResourceOrderField;
using LocationResourcesService = Api.Shared.Grpc.Skedular.Location.Resources.V1.LocationResourcesService;
using ResourcePatchField = Api.Shared.Grpc.Skedular.Location.Resources.V1.ResourcePatchField;

namespace Location.Api.Grpc;

public class LocationResourcesGrpcService(
    LocationConfiguration locationConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    IResourceService resourceService,
    IGrpcMapper grpcMapper) : LocationResourcesService.LocationResourcesServiceBase
{
    public override async Task<ResourceConnection> GetPaginatedResources(GetPaginatedResourcesInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await resourceService.GetPaginatedResourcesAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new ResourceSearchCriteria(request.Where.LocationId, request.Where.NameContains, request.Where.TagIds, request.Where.FloorPlanId),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Grpc.Skedular.Location.Core.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    ResourceOrderField.ResourceName => Shared.Models.ResourceOrderField.Name,
                    _ => throw new ArgumentOutOfRangeException(null,
                        "Unexpected value encountered. Update enum mapping or caller input to include this case."),
                };

                return new ResourceOrder(direction, field);
            }).ToList(),
            context.CancellationToken);

        var connection = new ResourceConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor.ToSafeString(),
                EndCursor = paginatedInfo.EndCursor.ToSafeString(),
            },
            TotalCount = totalCount,
        };

        connection.Edges.AddRange(edges.Select(grpcMapper.MapToGrpcResponse));
        return connection;
    }

    public override async Task<Resource> Admin_GetResource(Admin_GetResourceInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(await resourceService.GetByIdAsync(request.Id, true, context.CancellationToken));
    }

    public override async Task<Resource> GetResource(GetResourceInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(await resourceService.GetByIdAsync(request.Id, false, context.CancellationToken));
    }

    public override async Task<Resource> AddResource(AddResourceInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(await resourceService.AddAsync(grpcMapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<Resource> UpdateResource(UpdateResourceInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(
            await resourceService.UpdateAsync(
                new ResourcePatchRequest(
                    grpcMapper.MapTo(request),
                    request.FieldsToUpdate.Select(field => field switch
                    {
                        ResourcePatchField.Name =>
                            Models.ResourcePatchField.Name,
                        ResourcePatchField.Inactive =>
                            Models.ResourcePatchField.Inactive,
                        ResourcePatchField.RequireBookingApproval =>
                            Models.ResourcePatchField.RequireBookingApproval,
                        ResourcePatchField.Tags =>
                            Models.ResourcePatchField.Tags,
                        ResourcePatchField.Color =>
                            Models.ResourcePatchField.Color,
                        ResourcePatchField.Capacity =>
                            Models.ResourcePatchField.Capacity,
                        _ => throw new ArgumentOutOfRangeException(nameof(request.FieldsToUpdate), field,
                            $"Unexpected value for {nameof(request.FieldsToUpdate)}: {field}. Update enum mapping or caller input."),
                    }).ToHashSet()),
                context.CancellationToken));
    }

    public override async Task<Resource> RemoveResource(RemoveResourceInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(await resourceService.DeleteAsync(request.Id, context.CancellationToken));
    }
}
