using System.Reflection;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Enterprise.Shared;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Pagination;
using Grpc.Core;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Api.Services.Authorization;
using Location.Shared.Configurations;
using Location.Shared.Models;
using Desk = Api.Shared.Services.Grpc.Skedular.Location.V1.Desk;
using DeskOrderField = Api.Shared.Services.Grpc.Skedular.Location.V1.DeskOrderField;
using LocationOrderField = Location.Shared.Models.LocationOrderField;
using LocationService = Api.Shared.Services.Grpc.Skedular.Location.V1.LocationService;
using OrderDirection = Enterprise.Shared.Pagination.OrderDirection;
using PageInfo = Api.Shared.Services.Grpc.Skedular.Location.V1.PageInfo;
using Permissions = Api.Shared.Services.Grpc.Skedular.Location.V1.Permissions;
using Version = Api.Shared.Services.Grpc.Skedular.Location.V1.Version;

namespace Location.Api.Grpc;

public class LocationGrpcService(
    LocationConfiguration locationConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    ILocationService locationService,
    ILocationMemberService locationMemberService,
    IDeskService deskService,
    ILocationAuthorizationService locationAuthorizationService,
    IMapper mapper) : LocationService.LocationServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return Task.FromResult(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location> Admin_Add(
        Admin_AddInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await locationService.AddAsync(mapper.MapTo(request), true, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location> Get(
        GetInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        var location = await locationService.GetByIdAsync(request.Id, false, context.CancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        return mapper.MapToGrpcResponse(location);
    }

    public override async Task<LocationConnection> Admin_GetPaginatedLocations(
        Admin_GetPaginatedLocationsInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await locationService.GetPaginatedLocationsAsync(
            new PaginationInputParam(
                request.After,
                request.First.FromNullInt(),
                request.Before,
                request.Last.FromNullInt()),
            new LocationSearchCriteria(
                request.Where.OrganizationId,
                request.Where.NameContains,
                request.Where.ZoneIds.ToArray(),
                request.Where.DeskTypeIds.ToArray()),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction ==
                                global::Api.Shared.Services.Grpc.Skedular.Location.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationOrderField.Name => LocationOrderField
                        .Name,
                    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationOrderField.About => LocationOrderField
                        .About,
                    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationOrderField.Timezone =>
                        LocationOrderField
                            .Timezone,
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
        Admin_GetInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        var location = await locationService.GetByIdAsync(request.Id, true, context.CancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        return mapper.MapToGrpcResponse(location);
    }

    public override async Task<LocationConnection> GetPaginatedLocations(
        GetPaginatedLocationsInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await locationService.GetPaginatedLocationsAsync(
            new PaginationInputParam(
                request.After,
                request.First.FromNullInt(),
                request.Before,
                request.Last.FromNullInt()),
            new LocationSearchCriteria(
                request.Where.OrganizationId,
                request.Where.NameContains,
                request.Where.ZoneIds.ToArray(),
                request.Where.DeskTypeIds.ToArray()),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction ==
                                global::Api.Shared.Services.Grpc.Skedular.Location.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationOrderField.Name => LocationOrderField
                        .Name,
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

        var permissions =
            await locationAuthorizationService.GetPermissionsAsync(request.Id, context.CancellationToken);
        return new Permissions
        {
            CanView = permissions.CanView,
            CanModify = permissions.CanModify,
            CanDelete = permissions.CanDelete,
            CanInvitePeople = permissions.CanInvitePeople,
            CanCancelPeopleExistingInvitations = permissions.CanCancelPeopleExistingInvitations,
            CanViewAnalytics = permissions.CanViewAnalytics
        };
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location> Add(
        AddInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await locationService.AddAsync(mapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location> Update(
        UpdateInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await locationService.UpdateAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location> Remove(
        RemoveInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await locationService.DeleteAsync(request.Id, context.CancellationToken));
    }

    public override async Task<DeskConnection> GetPaginatedDesks(GetPaginatedDesksInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await deskService.GetPaginatedDesksAsync(
            new PaginationInputParam(
                request.After,
                request.First.FromNullInt(),
                request.Before,
                request.Last.FromNullInt()),
            new DeskSearchCriteria(
                request.Where.LocationId,
                request.Where.NameContains, 
                request.Where.ZoneIds,
                request.Where.DeskTypeIds),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction ==
                                global::Api.Shared.Services.Grpc.Skedular.Location.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    DeskOrderField.DeskName => Shared.Models.DeskOrderField
                        .Name,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new DeskOrder(direction, field);
            }).ToList(),
            context.CancellationToken);

        var connection = new DeskConnection
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

    public override async Task<Desk> GetDesk(GetDeskInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await deskService.GetByIdAsync(request.Id, context.CancellationToken));
    }

    public override async Task<Desk> AddDesk(AddDeskInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await deskService.AddAsync(
            mapper.MapTo(request),
            false,
            context.CancellationToken));
    }

    public override async Task<BulkAddDesksResponse> BulkAddDesks(BulkAddDesksInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        var bulkAddDesksResponse = new BulkAddDesksResponse();

        var desks = await deskService.BulkAddAsync(
            request.LocationId,
            request.NamePrefix,
            request.Count,
            request.DeskTypeIds,
            request.ZoneIds,
            request.Deactivated,
            request.RequireBookingApproval,
            context.CancellationToken);

        bulkAddDesksResponse.Desks.AddRange(desks.Select(mapper.MapToGrpcResponse));

        return bulkAddDesksResponse;
    }

    public override async Task<Desk> UpdateDesk(UpdateDeskInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await deskService.UpdateAsync(mapper.MapTo(request),
            context.CancellationToken));
    }

    public override async Task<Desk> RemoveDesk(RemoveDeskInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await deskService.DeleteAsync(request.Id, context.CancellationToken));
    }
}
