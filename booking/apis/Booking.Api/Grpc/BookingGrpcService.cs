using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Models;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Version;
using Grpc.Core;
using BookingOrderField = Booking.Shared.Models.BookingOrderField;
using BookingService = Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingService;
using OrderDirection = Enterprise.Shared.Pagination.OrderDirection;
using OrganizationPermissions = Api.Shared.Services.Grpc.Skedular.Booking.V1.OrganizationPermissions;
using TeamPermissions = Api.Shared.Services.Grpc.Skedular.Booking.V1.TeamPermissions;
using Version = Api.Shared.Services.Grpc.Skedular.Booking.V1.Version;

namespace Booking.Api.Grpc;

public class BookingGrpcService(
    IVersionService versionService,
    BookingConfiguration bookingConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    IBookingService bookingService,
    IResourceService resourceService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ITeamAuthorizationService teamAuthorizationService,
    IMapper mapper)
    : BookingService.BookingServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var version = versionService.GetVersion();

        return Task.FromResult(new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision });
    }

    public override async Task<BookingConnection> Admin_GetPaginatedBookings(Admin_GetPaginatedBookingsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(bookingConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await bookingService.GetPaginatedBookingsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new BookingSearchCriteria(
                request.Where.FromGt?.ToDateTimeOffset(),
                request.Where.FromGte?.ToDateTimeOffset(),
                request.Where.FromLt?.ToDateTimeOffset(),
                request.Where.FromLte?.ToDateTimeOffset(),
                request.Where.ToGt?.ToDateTimeOffset(),
                request.Where.ToGte?.ToDateTimeOffset(),
                request.Where.ToLt?.ToDateTimeOffset(),
                request.Where.ToLte?.ToDateTimeOffset(),
                request.Where.NotesContains,
                request.Where.NameContains,
                request.Where.Type.ToNullableBookingType(),
                request.Where.PaymentStatuses.Select(x => x.ToPaymentStatus()),
                request.Where.IncludeMineOnly,
                request.Where.IncludeFutureBookingsOnly,
                request.Where.OrganizationIds,
                request.Where.LocationIds,
                request.Where.TeamIds,
                request.Where.CustomerIds),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Services.Grpc.Skedular.Booking.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingOrderField.From => BookingOrderField.From,
                    global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingOrderField.To => BookingOrderField.To,
                    global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingOrderField.Notes => BookingOrderField.Notes,
                    global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingOrderField.Type => BookingOrderField.Type,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new BookingOrder(direction, field);
            }).ToList(),
            true,
            context.CancellationToken);

        var connection = new BookingConnection
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

    public override async Task<BookingConnection> GetPaginatedBookings(GetPaginatedBookingsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(bookingConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await bookingService.GetPaginatedBookingsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new BookingSearchCriteria(
                request.Where.FromGt?.ToDateTimeOffset(),
                request.Where.FromGte?.ToDateTimeOffset(),
                request.Where.FromLt?.ToDateTimeOffset(),
                request.Where.FromLte?.ToDateTimeOffset(),
                request.Where.ToGt?.ToDateTimeOffset(),
                request.Where.ToGte?.ToDateTimeOffset(),
                request.Where.ToLt?.ToDateTimeOffset(),
                request.Where.ToLte?.ToDateTimeOffset(),
                request.Where.NotesContains,
                request.Where.NameContains,
                request.Where.Type.ToNullableBookingType(),
                request.Where.PaymentStatuses.Select(x => x.ToPaymentStatus()),
                request.Where.IncludeMineOnly,
                request.Where.IncludeFutureBookingsOnly,
                request.Where.OrganizationIds,
                request.Where.LocationIds,
                request.Where.TeamIds,
                request.Where.CustomerIds),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Services.Grpc.Skedular.Booking.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingOrderField.From => BookingOrderField.From,
                    global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingOrderField.To => BookingOrderField.To,
                    global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingOrderField.Notes => BookingOrderField.Notes,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new BookingOrder(direction, field);
            }).ToList(),
            false,
            context.CancellationToken);

        var connection = new BookingConnection
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

    public override async Task<OrganizationPermissions> GetOrganizationPermissions(GetOrganizationPermissionsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(bookingConfiguration.ApiKey);

        var permissions = await organizationAuthorizationService.GetPermissionsAsync(request.OrganizationId, context.CancellationToken);
        return new OrganizationPermissions
        {
            CanViewBookings = permissions.CanViewBookings,
            CanAddBooking = permissions.CanAddBooking,
            CanUpdateBooking = permissions.CanUpdateBooking,
            CanDeleteBooking = permissions.CanDeleteBooking
        };
    }

    public override async Task<TeamPermissions> GetTeamPermissions(GetTeamPermissionsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(bookingConfiguration.ApiKey);

        var permissions = await teamAuthorizationService.GetPermissionsAsync(request.TeamId, context.CancellationToken);
        return new TeamPermissions
        {
            CanViewBookings = permissions.CanViewBookings,
            CanAddBooking = permissions.CanAddBooking,
            CanUpdateBooking = permissions.CanUpdateBooking,
            CanDeleteBooking = permissions.CanDeleteBooking
        };
    }


    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking> Get(GetInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(bookingConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await bookingService.GetByIdAsync(request.Id, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking> Add(AddInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(bookingConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await bookingService.AddAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking> Update(UpdateInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(bookingConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await bookingService.UpdateAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking> Delete(DeleteInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(bookingConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await bookingService.DeleteAsync(request.Id, context.CancellationToken));
    }

    public override async Task<AvailableResources> GetAvailableResources(GetAvailableResourcesInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(bookingConfiguration.ApiKey);

        var resources = await resourceService.GetAvailableResourcesAsync(
            request.OrganizationId,
            request.LocationId,
            request.From.ToDateTimeOffset(),
            request.Until.ToDateTimeOffset(),
            request.CustomTagIds,
            request.ZoneIds,
            request.ResourceIdsToInclude,
            request.ProductId,
            context.CancellationToken);

        var availableResources = new AvailableResources();
        availableResources.Resources.AddRange(mapper.MapToGrpcResponse(resources));

        return availableResources;
    }
}
