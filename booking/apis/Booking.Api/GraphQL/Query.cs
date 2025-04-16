using System.Reflection;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Enterprise.Shared;
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

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public async Task<bool> BookingCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    [UseResolverScope]
    public async Task<BookingDetails?> BookingAsync(string id, [Service] IBookingService bookingService, CancellationToken cancellationToken) =>
        mapper.MapTo(await bookingService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    public async Task<BookingConnection> BookingsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        BookingWhereInput where,
        IEnumerable<BookingOrderInput>? orderBy,
        [Service] IBookingService bookingService,
        CancellationToken cancellationToken)
    {
        where.OrganizationIds = where.OrganizationIds.RemoveInvalidIds();
        where.LocationIds = where.LocationIds.RemoveInvalidIds();
        where.TeamIds = where.TeamIds.RemoveInvalidIds();
        where.CustomerIds = where.CustomerIds.RemoveInvalidIds();

        var (paginatedInfo, edges, totalCount) = await bookingService.GetPaginatedBookingsAsync(
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
                where.OrganizationIds.ToSafeCollection(),
                where.LocationIds.ToSafeCollection(),
                where.TeamIds.ToSafeCollection(),
                where.CustomerIds.ToSafeCollection()),
            orderBy.ToSafeCollection().Select(item => new BookingOrder(item.Direction, item.Field)).ToList(),
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
            Edges = edges.Select(mapper.MapTo),
            TotalCount = totalCount
        };
    }

    [UseResolverScope]
    public async Task<IEnumerable<BookingDetails>> AllBookingsAsync(
        BookingWhereInput where,
        [Service] IBookingService bookingService,
        CancellationToken cancellationToken)
    {
        var result = await BookingsAsync(null, null, null, null, where, [], bookingService, cancellationToken);
        return result.Edges.Select(item => item.Node);
    }

    [UseResolverScope]
    public async Task<OrganizationBookingPermissions> OrganizationBookingPermissionsAsync(
        string organizationId,
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
    public async Task<TeamBookingPermissions> TeamBookingPermissionsAsync(
        string teamId,
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
    public async Task<IEnumerable<BookingResourceDetails>> AvailableResourcesAsync(
        AvailableResourcesWhereInput where,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await resourceService.GetAvailableResourcesAsync(
            where.OrganizationId,
            where.LocationId,
            where.From,
            where.Until,
            where.CustomTagIds.ToSafeCollection(),
            where.ZoneIds.ToSafeCollection(),
            where.ResourceIdsToInclude.ToSafeCollection(),
            where.ProductId,
            cancellationToken));

    [UseResolverScope]
    public async Task<OrganizationAvailableResources> OrganizationResourcesAvailabilityAsync(
        OrganizationAvailableResourcesWhereInput where,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken)
    {
        var (resourcesCount, availableResourcesCount) = await resourceService.GetOrganizationResourceAvailabilityAsync(
            where.OrganizationId,
            where.From,
            where.Until,
            cancellationToken);

        return new OrganizationAvailableResources { ResourcesCount = resourcesCount, AvailableResourcesCount = availableResourcesCount };
    }
}
