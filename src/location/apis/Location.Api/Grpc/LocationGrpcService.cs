using Api.Shared.Grpc.Skedular.Location.Core.V1;
using Api.Shared.Services;
using Api.Shared.Services.Configurations.Grpc;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Version;
using Grpc.Core;
using Location.Api.Mappers;
using Location.Api.Models;
using Location.Api.Services;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using LocationOrderField = Location.Shared.Models.LocationOrderField;
using LocationPatchField = Location.Api.Models.LocationPatchField;
using LocationService = Api.Shared.Grpc.Skedular.Location.Core.V1.LocationService;
using OrderDirection = Enterprise.Shared.Pagination.OrderDirection;
using PageInfo = Api.Shared.Grpc.Skedular.Location.Core.V1.PageInfo;
using Permissions = Api.Shared.Grpc.Skedular.Location.Core.V1.Permissions;
using Version = Api.Shared.Grpc.Skedular.Location.Core.V1.Version;
using LocationType = Api.Shared.Services.Models.LocationType;

namespace Location.Api.Grpc;

public class LocationGrpcService(
    IVersionService versionService,
    LocationConfiguration locationConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    ILocationService locationService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IGrpcMapper grpcMapper) : LocationService.LocationServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var version = versionService.GetVersion();

        return Task.FromResult(new Version
        {
            Major = version.Major,
            Minor = version.Minor,
            Build = version.Build,
            Revision = version.Revision,
        });
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Location.Core.V1.Location> Admin_Add(
        Admin_AddInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        ArgumentException.ThrowIfNullOrWhiteSpace(request.OrganizationId);

        return grpcMapper.MapToGrpcResponse(await locationService.AddAsync(grpcMapper.MapTo(request), true, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Location.Core.V1.Location> Admin_Update(
        Admin_UpdateInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        ArgumentException.ThrowIfNullOrWhiteSpace(request.OrganizationId);

        return grpcMapper.MapToGrpcResponse(
            await locationService.UpdateAsync(
                new LocationPatchRequest(grpcMapper.MapTo(request), MapToPatchFields(request.FieldsToUpdate)),
                true,
                context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Location.Core.V1.Location> Get(GetInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        var location = await locationService.GetByIdAsync(request.Id, false, context.CancellationToken) ?? throw new LocationNotFound();

        return grpcMapper.MapToGrpcResponse(location);
    }

    public override async Task<LocationConnection> Admin_GetPaginatedLocations(Admin_GetPaginatedLocationsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await locationService.GetPaginatedLocationsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new LocationSearchCriteria(
                request.Where.OrganizationId,
                request.Where.OrganizationCustomDomain,
                request.Where.LocationIds,
                request.Where.NameContains,
                request.Where.TagIds,
                null,
                request.Where.Types_.Select(item => item switch
                {
                    global::Api.Shared.Grpc.Skedular.Location.Core.V1.LocationType.Private => LocationType.Private,
                    global::Api.Shared.Grpc.Skedular.Location.Core.V1.LocationType.Marketplace => LocationType.Marketplace,
                    _ => throw new ArgumentOutOfRangeException(null,
                        "Unexpected value encountered. Update enum mapping or caller input to include this case."),
                }).ToList(),
                null,
                request.Where.NotContactedYet,
                null,
                null,
                []),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Grpc.Skedular.Location.Core.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    global::Api.Shared.Grpc.Skedular.Location.Core.V1.LocationOrderField.Name => LocationOrderField.Name,
                    global::Api.Shared.Grpc.Skedular.Location.Core.V1.LocationOrderField.Timezone => LocationOrderField.Timezone,
                    _ => throw new ArgumentOutOfRangeException(null,
                        "Unexpected value encountered. Update enum mapping or caller input to include this case."),
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
                EndCursor = paginatedInfo.EndCursor.ToSafeString(),
            },
            TotalCount = totalCount,
        };

        connection.Edges.AddRange(edges.Select(grpcMapper.MapToGrpcResponse));
        return connection;
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Location.Core.V1.Location> Admin_Get(
        Admin_GetInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        var location = await locationService.GetByIdAsync(request.Id, true, context.CancellationToken) ?? throw new LocationNotFound();

        return grpcMapper.MapToGrpcResponse(location);
    }

    public override async Task<LocationConnection> GetPaginatedLocations(GetPaginatedLocationsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await locationService.GetPaginatedLocationsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new LocationSearchCriteria(
                request.Where.OrganizationId,
                request.Where.OrganizationCustomDomain,
                request.Where.LocationIds,
                request.Where.NameContains,
                request.Where.TagIds,
                null,
                request.Where.Types_.Select(item => item switch
                {
                    global::Api.Shared.Grpc.Skedular.Location.Core.V1.LocationType.Private => LocationType.Private,
                    global::Api.Shared.Grpc.Skedular.Location.Core.V1.LocationType.Marketplace => LocationType.Marketplace,
                    _ => throw new ArgumentOutOfRangeException(null,
                        "Unexpected value encountered. Update enum mapping or caller input to include this case."),
                }).ToList(),
                null,
                request.Where.NotContactedYet,
                null,
                null,
                []),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Grpc.Skedular.Location.Core.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    global::Api.Shared.Grpc.Skedular.Location.Core.V1.LocationOrderField.Name => LocationOrderField.Name,
                    _ => throw new ArgumentOutOfRangeException(null,
                        "Unexpected value encountered. Update enum mapping or caller input to include this case."),
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
                EndCursor = paginatedInfo.EndCursor.ToSafeString(),
            },
            TotalCount = totalCount,
        };

        connection.Edges.AddRange(edges.Select(grpcMapper.MapToGrpcResponse));
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
            CanViewAnalytics = permissions.CanViewAnalytics,
        };
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Location.Core.V1.Location> Add(AddInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        ArgumentException.ThrowIfNullOrWhiteSpace(request.OrganizationId);

        return grpcMapper.MapToGrpcResponse(await locationService.AddAsync(grpcMapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Location.Core.V1.Location> Update(UpdateInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        ArgumentException.ThrowIfNullOrWhiteSpace(request.OrganizationId);

        return grpcMapper.MapToGrpcResponse(
            await locationService.UpdateAsync(
                new LocationPatchRequest(grpcMapper.MapTo(request), MapToPatchFields(request.FieldsToUpdate)),
                false,
                context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Location.Core.V1.Location> Remove(RemoveInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(locationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(await locationService.DeleteAsync(request.Id, context.CancellationToken));
    }

    private static HashSet<LocationPatchField> MapToPatchFields(
        IEnumerable<global::Api.Shared.Grpc.Skedular.Location.Core.V1.LocationPatchField> fields) =>
        fields.Select(field => field switch
        {
            global::Api.Shared.Grpc.Skedular.Location.Core.V1.LocationPatchField.Name => LocationPatchField.Name,
            global::Api.Shared.Grpc.Skedular.Location.Core.V1.LocationPatchField.ListingMetadata =>
                LocationPatchField.ListingMetadata,
            global::Api.Shared.Grpc.Skedular.Location.Core.V1.LocationPatchField.Organization =>
                LocationPatchField.Organization,
            global::Api.Shared.Grpc.Skedular.Location.Core.V1.LocationPatchField.Timezone => LocationPatchField.Timezone,
            global::Api.Shared.Grpc.Skedular.Location.Core.V1.LocationPatchField.Tags => LocationPatchField.Tags,
            global::Api.Shared.Grpc.Skedular.Location.Core.V1.LocationPatchField.FeatureImages =>
                LocationPatchField.FeatureImages,
            global::Api.Shared.Grpc.Skedular.Location.Core.V1.LocationPatchField.ExtraMetadata =>
                LocationPatchField.ExtraMetadata,
            global::Api.Shared.Grpc.Skedular.Location.Core.V1.LocationPatchField.PhysicalAddress =>
                LocationPatchField.PhysicalAddress,
            global::Api.Shared.Grpc.Skedular.Location.Core.V1.LocationPatchField.UniqueClaimCode =>
                LocationPatchField.UniqueClaimCode,
            _ => throw new ArgumentOutOfRangeException(nameof(fields), field,
                $"Unexpected value for {nameof(fields)}: {field}. Update enum mapping or caller input."),
        }).ToHashSet();
}
