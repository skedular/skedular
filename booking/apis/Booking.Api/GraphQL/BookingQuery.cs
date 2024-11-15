using System.Reflection;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Enterprise.Shared.Context;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Sanitization;

namespace Booking.Api.GraphQL;

public class BookingQuery(IServiceProvider serviceProvider, IMapper mapper)
{
    public Task<Version> BookingVersionAsync(CancellationToken cancellationToken)
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

    public async Task<bool> BookingCustomerRecordSyncedAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        return await service.DoesCustomerExistAsync(cancellationToken);
    }

    public async Task<BookingDetails?> BookingAsync(string id, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var booking = await service.GetByIdAsync(id, cancellationToken);
        return mapper.MapTo(booking);
    }

    public async Task<BookingConnection?> BookingsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        BookingWhereInput where,
        BookingOrderInput[]? orderBy,
        CancellationToken cancellationToken)
    {
        where.OrganizationIds = where.OrganizationIds.RemoveInvalidIds();
        where.LocationIds = where.LocationIds.RemoveInvalidIds();
        where.TeamIds = where.TeamIds.RemoveInvalidIds();

        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var service = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var (paginatedInfo, edges, totalCount) =
            await service.GetPaginatedBookingsAsync(
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
                    where.IncludeMineOnly,
                    where.OrganizationIds ?? [],
                    where.LocationIds ?? [],
                    where.TeamIds ?? []),
                orderBy is null
                    ? []
                    : orderBy.Select(item =>
                    {
                        var direction = item.Direction == OrderDirection.Ascending
                            ? Enterprise.Shared.Pagination.OrderDirection.Ascending
                            : Enterprise.Shared.Pagination.OrderDirection.Descending;
                        var field = item.Field switch
                        {
                            BookingOrderField.from => Shared.Models.BookingOrderField.From,
                            BookingOrderField.to => Shared.Models.BookingOrderField.To,
                            BookingOrderField.notes => Shared.Models.BookingOrderField.Notes,
                            BookingOrderField.name => Shared.Models.BookingOrderField.Name,
                            BookingOrderField.givenName => Shared.Models.BookingOrderField.GivenName,
                            BookingOrderField.middleName => Shared.Models.BookingOrderField.MiddleName,
                            BookingOrderField.familyName => Shared.Models.BookingOrderField.FamilyName,
                            BookingOrderField.organizationName => Shared.Models.BookingOrderField.OrganizationName,
                            BookingOrderField.locationName => Shared.Models.BookingOrderField.LocationName,
                            BookingOrderField.teamName => Shared.Models.BookingOrderField.TeamName,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        return new BookingOrder(direction, field);
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

    public async Task<BookingDetails[]?> AllBookingsAsync(BookingWhereInput where, CancellationToken cancellationToken)
    {
        var result = await BookingsAsync(null, null, null, null, where, [], cancellationToken);
        return result?.Edges.Select(item => item.Node).ToArray();
    }

    public async Task<BookingDeskDetails[]?> AvailableLocationDesksAsync(
        string locationId,
        DateTimeOffset date,
        string[] deskIdsToInclude,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var service = scope.ServiceProvider.GetRequiredService<IDeskService>();
        var desks = await service.GetAvailableDesksAsync(locationId, date, deskIdsToInclude, cancellationToken);
        return mapper.MapTo(desks).ToArray();
    }

    public async Task<OrganizationBookingPermissions?> OrganizationBookingPermissionsAsync(
        string organizationId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var organizationAuthorizationService =
            scope.ServiceProvider.GetRequiredService<IOrganizationAuthorizationService>();
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

    public async Task<LocationBookingPermissions?> LocationBookingPermissionsAsync(
        string locationId,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var locationAuthorizationService =
            scope.ServiceProvider.GetRequiredService<ILocationAuthorizationService>();
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

    public async Task<TeamBookingPermissions?> TeamBookingPermissionsAsync(
        string teamId,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var teamAuthorizationService =
            scope.ServiceProvider.GetRequiredService<ITeamAuthorizationService>();
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
}
