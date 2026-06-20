using Api.Shared.Grpc.Skedular.Booking.Core.V1;
using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.Models;
using Booking.Api.Mappers;
using Booking.Api.Models;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Version;
using Grpc.Core;
using BookingOrderField = Booking.Shared.Models.BookingOrderField;
using BookingService = Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingService;
using OrderDirection = Enterprise.Shared.Pagination.OrderDirection;
using OrganizationPermissions = Api.Shared.Grpc.Skedular.Booking.Core.V1.OrganizationPermissions;
using PrivateBookingPatchField = Api.Shared.Grpc.Skedular.Booking.Core.V1.PrivateBookingPatchField;
using TeamPermissions = Api.Shared.Grpc.Skedular.Booking.Core.V1.TeamPermissions;
using Version = Api.Shared.Grpc.Skedular.Booking.Core.V1.Version;

namespace Booking.Api.Grpc;

public class BookingGrpcService(
    IVersionService versionService,
    BookingConfiguration bookingConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    IBookingService bookingService,
    IPrivateBookingService privateBookingService,
    IResourceService resourceService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ITeamAuthorizationService teamAuthorizationService,
    IGrpcMapper grpcMapper)
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
                request.Where.Category.ToNullableBookingCategory(),
                request.Where.Channel.ToNullableBookingChannel(),
                request.Where.PaymentStatuses.Select(item => item.ToPaymentStatus()).ToList(),
                request.Where.IncludeMineOnly,
                request.Where.IncludeFutureBookingsOnly,
                request.Where.OrganizationId.ToSafeString(),
                null,
                request.Where.LocationIds,
                request.Where.TeamIds,
                request.Where.CustomerIds,
                []),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Grpc.Skedular.Booking.Core.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingOrderField.From => BookingOrderField.From,
                    global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingOrderField.To => BookingOrderField.To,
                    global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingOrderField.Notes => BookingOrderField.Notes,
                    global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingOrderField.Category => BookingOrderField.Category,
                    global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingOrderField.Channel => BookingOrderField.Channel,
                    _ => throw new ArgumentOutOfRangeException(null,
                        "Unexpected value encountered. Update enum mapping or caller input to include this case.")
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

        connection.Edges.AddRange(edges.Select(grpcMapper.MapToGrpcResponse));
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
                request.Where.Category.ToNullableBookingCategory(),
                request.Where.Channel.ToNullableBookingChannel(),
                request.Where.PaymentStatuses.Select(x => x.ToPaymentStatus()).ToList(),
                request.Where.IncludeMineOnly,
                request.Where.IncludeFutureBookingsOnly,
                request.Where.OrganizationId.ToSafeString(),
                null,
                request.Where.LocationIds,
                request.Where.TeamIds,
                request.Where.CustomerIds,
                []),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Grpc.Skedular.Booking.Core.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingOrderField.From => BookingOrderField.From,
                    global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingOrderField.To => BookingOrderField.To,
                    global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingOrderField.Notes => BookingOrderField.Notes,
                    global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingOrderField.Category => BookingOrderField.Category,
                    global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingOrderField.Channel => BookingOrderField.Channel,
                    _ => throw new ArgumentOutOfRangeException(null,
                        "Unexpected value encountered. Update enum mapping or caller input to include this case.")
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

        connection.Edges.AddRange(edges.Select(grpcMapper.MapToGrpcResponse));
        return connection;
    }

    public override async Task<OrganizationPermissions> GetOrganizationPermissions(GetOrganizationPermissionsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(bookingConfiguration.ApiKey);

        var permissions = await organizationAuthorizationService.GetPermissionsAsync(request.OrganizationId, null, context.CancellationToken);
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


    public override async Task<global::Api.Shared.Grpc.Skedular.Booking.Core.V1.Booking> Get(GetInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(bookingConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(await bookingService.GetByIdAsync(request.Id, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Booking.Core.V1.Booking> AddPrivate(
        AddPrivateInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(bookingConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(await privateBookingService.AddAsync(grpcMapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Booking.Core.V1.Booking> UpdatePrivate(
        UpdatePrivateInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(bookingConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(
            await privateBookingService.UpdateAsync(
                new PrivateBookingPatchRequest(
                    grpcMapper.MapTo(request),
                    request.FieldsToUpdate.Select(field => field switch
                    {
                        PrivateBookingPatchField.Participants =>
                            Models.PrivateBookingPatchField.Participants,
                        PrivateBookingPatchField.Schedule =>
                            Models.PrivateBookingPatchField.Schedule,
                        PrivateBookingPatchField.Notes =>
                            Models.PrivateBookingPatchField.Notes,
                        PrivateBookingPatchField.Category =>
                            Models.PrivateBookingPatchField.Category,
                        PrivateBookingPatchField.Resources =>
                            Models.PrivateBookingPatchField.Resources,
                        _ => throw new ArgumentOutOfRangeException(nameof(request.FieldsToUpdate), field,
                            $"Unexpected value for {nameof(request.FieldsToUpdate)}: {field}. Update enum mapping or caller input.")
                    }).ToHashSet()),
                context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Booking.Core.V1.Booking> DeletePrivate(
        DeletePrivateInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(bookingConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(await privateBookingService.DeleteAsync(request.Id, context.CancellationToken));
    }

    public override async Task<AvailableResources> GetAvailableResources(GetAvailableResourcesInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(bookingConfiguration.ApiKey);

        var resources = await resourceService.GetAvailableResourcesAsync(
            request.OrganizationId,
            null,
            request.LocationId,
            request.From.ToDateTimeOffset(),
            request.Until.ToDateTimeOffset(),
            request.CustomTagIds,
            request.ZoneIds,
            request.ResourceIdsToInclude,
            request.ProductId,
            context.CancellationToken);

        var availableResources = new AvailableResources();
        availableResources.ResourceIds.AddRange(resources.Select(item => item.Id));

        return availableResources;
    }
}
