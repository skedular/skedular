using Api.Shared.Services.Models;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Booking;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public IEnumerable<BookingTypeDetails> BookingTypes() =>
    [
        new() { Type = BookingType.WorkingFromHome, Name = BookingTypeConstants.WorkingFromHome.ToBookingTypeName() },
        new() { Type = BookingType.WorkingFromOffice, Name = BookingTypeConstants.WorkingFromOffice.ToBookingTypeName() },
        new() { Type = BookingType.WorkingFromCoworkingSpace, Name = BookingTypeConstants.WorkingFromCoworkingSpace.ToBookingTypeName() },
        new() { Type = BookingType.SickLeave, Name = BookingTypeConstants.SickLeave.ToBookingTypeName() },
        new() { Type = BookingType.AnnualLeave, Name = BookingTypeConstants.AnnualLeave.ToBookingTypeName() },
        new() { Type = BookingType.WellbeingLeave, Name = BookingTypeConstants.WellbeingLeave.ToBookingTypeName() },
        new() { Type = BookingType.ClientOffice, Name = BookingTypeConstants.ClientOffice.ToBookingTypeName() },
        new() { Type = BookingType.Vacation, Name = BookingTypeConstants.Vacation.ToBookingTypeName() },
        new() { Type = BookingType.TravelingForWork, Name = BookingTypeConstants.TravelingForWork.ToBookingTypeName() },
        new() { Type = BookingType.NonWorkingDay, Name = BookingTypeConstants.NonWorkingDay.ToBookingTypeName() }
    ];

    [UseResolverScope]
    public IEnumerable<BookingTypeDetails> MarketplaceBookingTypes() =>
    [
        new() { Type = BookingType.WorkingFromOffice, Name = BookingTypeConstants.WorkingFromOffice.ToBookingTypeName() },
        new() { Type = BookingType.WorkingFromCoworkingSpace, Name = BookingTypeConstants.WorkingFromCoworkingSpace.ToBookingTypeName() },
        new() { Type = BookingType.ClientOffice, Name = BookingTypeConstants.ClientOffice.ToBookingTypeName() }
    ];

    [UseResolverScope]
    public async Task<BookingDetails?> BookingAsync(string id, [Service] IBookingService bookingService, CancellationToken cancellationToken) =>
        mapper.MapTo(await bookingService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    public async Task<Connection<BookingEdge>> BookingsAsync(
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
                where.FromGt,
                where.FromGte,
                where.FromLt,
                where.FromLte,
                where.ToGt,
                where.ToGte,
                where.ToLt,
                where.ToLte,
                where.NotesContains,
                where.NameContains,
                where.Type,
                where.PaymentStatuses.ToSafeCollection(),
                where.IncludeMineOnly,
                where.IncludeFutureBookingsOnly,
                where.OrganizationIds.ToSafeCollection(),
                where.LocationIds.ToSafeCollection(),
                where.TeamIds.ToSafeCollection(),
                where.CustomerIds.ToSafeCollection()),
            orderBy.ToSafeCollection().Select(item => new BookingOrder(item.Direction, item.Field)).ToList(),
            false,
            cancellationToken);

        return new Connection<BookingEdge>
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
}
