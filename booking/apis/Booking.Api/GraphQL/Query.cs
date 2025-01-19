using System.Reflection;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Types;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Booking.Api.GraphQL;

[QueryType]
public class Query(IMapper mapper)
{
    [UseResolverScope]
    public Version BookingVersion()
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        };
    }

    [UseResolverScope]
    public async Task<bool> BookingCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    [UseResolverScope]
    public async Task<BookingDetails?> BookingAsync(
        string id,
        [Service] IBookingService bookingService,
        CancellationToken cancellationToken)
    {
        var booking = await bookingService.GetByIdAsync(id, cancellationToken);
        return mapper.MapTo(booking);
    }

    [UseResolverScope]
    public async Task<BookingConnection?> BookingsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        BookingWhereInput where,
        BookingOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IBookingService bookingService,
        CancellationToken cancellationToken)
    {
        where.OrganizationIds = where.OrganizationIds.RemoveInvalidIds();
        where.LocationIds = where.LocationIds.RemoveInvalidIds();
        where.TeamIds = where.TeamIds.RemoveInvalidIds();

        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) =
            await bookingService.GetPaginatedBookingsAsync(
                new PaginationInputParam(after, first, before, last),
                new BookingSearchCriteria(
                    where.FromGT,
                    where.FromGTE,
                    where.FromLT,
                    where.FromLTE,
                    where.ToGT,
                    where.ToGTE,
                    where.ToLT,
                    where.ToLTE,
                    where.NotesContains,
                    where.NameContains,
                    string.IsNullOrWhiteSpace(where.Type) ? null : where.Type,
                    where.IncludeMineOnly,
                    where.IncludeFutureBookingsOnly,
                    where.CombineOrganizationsLocationsTeams,
                    where.OrganizationIds ?? [],
                    where.LocationIds ?? [],
                    where.TeamIds ?? []),
                orderBy is null
                    ? []
                    : orderBy.Select(item =>
                    {
                        var direction = item.Direction == OrderDirection.Ascending ? OrderDirection.Ascending: OrderDirection.Descending;
                        return new BookingOrder(direction, item.Field);
                    }).ToList(),
                false,
                cancellationToken);

        return new BookingConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor
            },
            Edges = edges.Select(mapper.MapTo).ToArray(),
            TotalCount = totalCount
        };
    }

    [UseResolverScope]
    public async Task<BookingDetails[]?> AllBookingsAsync(
        BookingWhereInput where,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IBookingService bookingService,
        CancellationToken cancellationToken)
    {
        var result = await BookingsAsync(
            null,
            null,
            null,
            null,
            where,
            [],
            cachedCustomerService,
            bookingService,
            cancellationToken);
        return result?.Edges.Select(item => item.Node).ToArray();
    }

    [UseResolverScope]
    public async Task<BookingDeskDetails[]?> AvailableDesksAsync(
        AvailableDesksWhereInput where,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IDeskService deskService,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var desks = await deskService.GetAvailableDesksAsync(
            where.OrganizationId,
            where.LocationId,
            where.Date,
            where.DeskIdsToInclude ?? [],
            where.CustomTagIds ?? [],
            where.ZoneIds ?? [],
            where.CombineCustomTagsZones ?? false,
            cancellationToken);
        return mapper.MapTo(desks).ToArray();
    }

    [UseResolverScope]
    public async Task<OrganizationBookingPermissions?> OrganizationBookingPermissionsAsync(
        string organizationId,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationAuthorizationService organizationAuthorizationService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(organizationId))
        {
            return new OrganizationBookingPermissions
            {
                CanAddBooking = false,
                CanUpdateBooking = false,
                CanDeleteBooking = false,
                CanAddBookingOnBehalf = false,
                CanUpdateBookingOnBehalf = false,
                CanDeleteBookingOnBehalf = false
            };
        }

        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var permissions = await organizationAuthorizationService.GetPermissionsAsync(organizationId, cancellationToken);

        return new OrganizationBookingPermissions
        {
            CanAddBooking = permissions.CanAddBooking,
            CanUpdateBooking = permissions.CanUpdateBooking,
            CanDeleteBooking = permissions.CanDeleteBooking,
            CanAddBookingOnBehalf = permissions.CanAddBookingOnBehalf,
            CanUpdateBookingOnBehalf = permissions.CanUpdateBookingOnBehalf,
            CanDeleteBookingOnBehalf = permissions.CanDeleteBookingOnBehalf
        };
    }

    [UseResolverScope]
    public async Task<LocationBookingPermissions?> LocationBookingPermissionsAsync(
        string locationId,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ILocationAuthorizationService locationAuthorizationService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(locationId))
        {
            return new LocationBookingPermissions
            {
                CanAddBooking = false,
                CanUpdateBooking = false,
                CanDeleteBooking = false,
                CanAddBookingOnBehalf = false,
                CanUpdateBookingOnBehalf = false,
                CanDeleteBookingOnBehalf = false
            };
        }

        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var permissions = await locationAuthorizationService.GetPermissionsAsync(locationId, cancellationToken);

        return new LocationBookingPermissions
        {
            CanAddBooking = permissions.CanAddBooking,
            CanUpdateBooking = permissions.CanUpdateBooking,
            CanDeleteBooking = permissions.CanDeleteBooking,
            CanAddBookingOnBehalf = permissions.CanAddBookingOnBehalf,
            CanUpdateBookingOnBehalf = permissions.CanUpdateBookingOnBehalf,
            CanDeleteBookingOnBehalf = permissions.CanDeleteBookingOnBehalf
        };
    }

    [UseResolverScope]
    public async Task<TeamBookingPermissions?> TeamBookingPermissionsAsync(
        string teamId,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ITeamAuthorizationService teamAuthorizationService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(teamId))
        {
            return new TeamBookingPermissions
            {
                CanAddBooking = false,
                CanUpdateBooking = false,
                CanDeleteBooking = false,
                CanAddBookingOnBehalf = false,
                CanUpdateBookingOnBehalf = false,
                CanDeleteBookingOnBehalf = false
            };
        }

        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var permissions = await teamAuthorizationService.GetPermissionsAsync(teamId, cancellationToken);

        return new TeamBookingPermissions
        {
            CanAddBooking = permissions.CanAddBooking,
            CanUpdateBooking = permissions.CanUpdateBooking,
            CanDeleteBooking = permissions.CanDeleteBooking,
            CanAddBookingOnBehalf = permissions.CanAddBookingOnBehalf,
            CanUpdateBookingOnBehalf = permissions.CanUpdateBookingOnBehalf,
            CanDeleteBookingOnBehalf = permissions.CanDeleteBookingOnBehalf
        };
    }

    [UseResolverScope]
    public async Task<OrganizationAvailableDesks?> OrganizationDesksAvailabilityAsync(
        OrganizationAvailableDesksWhereInput where,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IDeskService deskService,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (desksCount, availableDesksCount) = await deskService.GetOrganizationDesksAvailabilityAsync(
            where.OrganizationId,
            where.Date,
            cancellationToken);

        return new OrganizationAvailableDesks { DesksCount = desksCount, AvailableDesksCount = availableDesksCount };
    }
}
