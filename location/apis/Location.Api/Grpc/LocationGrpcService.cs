using Api.Shared.Services;
using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Version;
using Grpc.Core;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using LocationOrderField = Location.Shared.Models.LocationOrderField;
using LocationService = Api.Shared.Services.Grpc.Skedular.Location.V1.LocationService;
using OrderDirection = Enterprise.Shared.Pagination.OrderDirection;
using PageInfo = Api.Shared.Services.Grpc.Skedular.Location.V1.PageInfo;
using Permissions = Api.Shared.Services.Grpc.Skedular.Location.V1.Permissions;
using Version = Api.Shared.Services.Grpc.Skedular.Location.V1.Version;
using Resource = Api.Shared.Services.Grpc.Skedular.Location.V1.Resource;
using ResourceOrderField = Api.Shared.Services.Grpc.Skedular.Location.V1.ResourceOrderField;
using LocationType = Api.Shared.Services.Models.LocationType;

namespace Location.Api.Grpc;

public class LocationGrpcService(
    IVersionService versionService,
    LocationConfiguration locationConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    ILocationService locationService,
    IResourceService resourceService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IMapper mapper) : LocationService.LocationServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var version = versionService.GetVersion();

        return Task.FromResult(new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision });
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location> Admin_Add(
        Admin_AddInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        ArgumentException.ThrowIfNullOrWhiteSpace(request.OrganizationId);

        return mapper.MapToGrpcResponse(await locationService.AddAsync(mapper.MapTo(request), true, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location> Get(GetInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        var location = await locationService.GetByIdAsync(request.Id, false, context.CancellationToken) ?? throw new LocationNotFound();

        return mapper.MapToGrpcResponse(location);
    }

    public override async Task<LocationConnection> Admin_GetPaginatedLocations(Admin_GetPaginatedLocationsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await locationService.GetPaginatedLocationsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new LocationSearchCriteria(
                request.Where.OrganizationId,
                null,
                request.Where.LocationIds,
                request.Where.NameContains,
                request.Where.TagIds,
                null,
                request.Where.Types_.Select(item => item switch
                {
                    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationType.Private => LocationType.Private,
                    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationType.Marketplace => LocationType.Marketplace,
                    _ => throw new ArgumentOutOfRangeException(nameof(item), item, null)
                }).ToList()),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Services.Grpc.Skedular.Location.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationOrderField.Name => LocationOrderField.Name,
                    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationOrderField.About => LocationOrderField.About,
                    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationOrderField.Timezone => LocationOrderField.Timezone,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new LocationOrder(direction, field);
            }).ToList(),
            true,
            context.CancellationToken);

        var connection = new LocationConnection
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

        connection.Edges.AddRange(edges.Select(mapper.MapToGrpcResponse));
        return connection;
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location> Admin_Get(
        Admin_GetInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        var location = await locationService.GetByIdAsync(request.Id, true, context.CancellationToken) ?? throw new LocationNotFound();

        return mapper.MapToGrpcResponse(location);
    }

    public override async Task<LocationConnection> GetPaginatedLocations(GetPaginatedLocationsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await locationService.GetPaginatedLocationsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new LocationSearchCriteria(
                request.Where.OrganizationId,
                null,
                request.Where.LocationIds,
                request.Where.NameContains,
                request.Where.TagIds,
                null,
                request.Where.Types_.Select(item => item switch
                {
                    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationType.Private => LocationType.Private,
                    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationType.Marketplace => LocationType.Marketplace,
                    _ => throw new ArgumentOutOfRangeException(nameof(item), item, null)
                }).ToList()),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Services.Grpc.Skedular.Location.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationOrderField.Name => LocationOrderField.Name,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new LocationOrder(direction, field);
            }).ToList(),
            false,
            context.CancellationToken);

        var connection = new LocationConnection
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

        connection.Edges.AddRange(edges.Select(mapper.MapToGrpcResponse));
        return connection;
    }

    public override async Task<Permissions> GetPermissions(GetPermissionsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        var permissions = await organizationAuthorizationService.GetPermissionsAsync(request.Id, context.CancellationToken);
        return new Permissions
        {
            CanView = permissions.CanView,
            CanModify = permissions.CanModify,
            CanDelete = permissions.CanDelete,
            CanViewAnalytics = permissions.CanViewAnalytics
        };
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location> Add(AddInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        ArgumentException.ThrowIfNullOrWhiteSpace(request.OrganizationId);

        return mapper.MapToGrpcResponse(await locationService.AddAsync(mapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location> Update(UpdateInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        ArgumentException.ThrowIfNullOrWhiteSpace(request.OrganizationId);

        return mapper.MapToGrpcResponse(await locationService.UpdateAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location> Remove(RemoveInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await locationService.DeleteAsync(request.Id, context.CancellationToken));
    }

    public override async Task<ResourceConnection> GetPaginatedResources(GetPaginatedResourcesInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await resourceService.GetPaginatedResourcesAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new ResourceSearchCriteria(request.Where.LocationId, request.Where.NameContains, request.Where.TagIds, request.Where.FloorPlanId),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Services.Grpc.Skedular.Location.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    ResourceOrderField.ResourceName => Shared.Models.ResourceOrderField.Name,
                    _ => throw new ArgumentOutOfRangeException()
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
                EndCursor = paginatedInfo.EndCursor.ToSafeString()
            },
            TotalCount = totalCount
        };

        connection.Edges.AddRange(edges.Select(mapper.MapToGrpcResponse));
        return connection;
    }

    public override async Task<Resource> GetResource(GetResourceInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await resourceService.GetByIdAsync(request.Id, context.CancellationToken));
    }

    public override async Task<Resource> AddResource(AddResourceInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await resourceService.AddAsync(mapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<Resource> UpdateResource(UpdateResourceInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await resourceService.UpdateAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<Resource> RemoveResource(RemoveResourceInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await resourceService.DeleteAsync(request.Id, context.CancellationToken));
    }
}
