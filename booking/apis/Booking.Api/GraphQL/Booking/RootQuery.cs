using Api.Shared.Services.Models;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Fusion.SourceSchema.Types;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Booking;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public IEnumerable<BookingCategoryDetails> BookingCategories() =>
    [
        new() { Category = BookingCategory.WorkingFromHome, Name = BookingCategoryConstants.WorkingFromHome.ToBookingCategoryName() },
        new() { Category = BookingCategory.WorkingFromOffice, Name = BookingCategoryConstants.WorkingFromOffice.ToBookingCategoryName() },
        new()
        {
            Category = BookingCategory.WorkingFromCoworkingSpace,
            Name = BookingCategoryConstants.WorkingFromCoworkingSpace.ToBookingCategoryName()
        },
        new() { Category = BookingCategory.SickLeave, Name = BookingCategoryConstants.SickLeave.ToBookingCategoryName() },
        new() { Category = BookingCategory.AnnualLeave, Name = BookingCategoryConstants.AnnualLeave.ToBookingCategoryName() },
        new() { Category = BookingCategory.WellbeingLeave, Name = BookingCategoryConstants.WellbeingLeave.ToBookingCategoryName() },
        new() { Category = BookingCategory.ClientOffice, Name = BookingCategoryConstants.ClientOffice.ToBookingCategoryName() },
        new() { Category = BookingCategory.Vacation, Name = BookingCategoryConstants.Vacation.ToBookingCategoryName() },
        new() { Category = BookingCategory.TravelingForWork, Name = BookingCategoryConstants.TravelingForWork.ToBookingCategoryName() },
        new() { Category = BookingCategory.NonWorkingDay, Name = BookingCategoryConstants.NonWorkingDay.ToBookingCategoryName() }
    ];

    [UseResolverScope]
    public IEnumerable<BookingCategoryDetails> MarketplaceBookingCategories() =>
    [
        new() { Category = BookingCategory.WorkingFromOffice, Name = BookingCategoryConstants.WorkingFromOffice.ToBookingCategoryName() },
        new()
        {
            Category = BookingCategory.WorkingFromCoworkingSpace,
            Name = BookingCategoryConstants.WorkingFromCoworkingSpace.ToBookingCategoryName()
        },
        new() { Category = BookingCategory.ClientOffice, Name = BookingCategoryConstants.ClientOffice.ToBookingCategoryName() }
    ];

    [UseResolverScope]
    public async Task<BookingDetails?> BookingAsync(string id, [Service] IPrivateBookingService privateBookingService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await privateBookingService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<BookingDetails?> BookingByIdAsync(
        string id,
        [Service] IPrivateBookingService privateBookingService,
        CancellationToken cancellationToken) =>
        await BookingAsync(id, privateBookingService, cancellationToken);

    [UseResolverScope]
    public async Task<Connection<BookingEdge>> BookingsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        BookingWhereInput where,
        IEnumerable<BookingOrderInput>? orderBy,
        [Service] IPrivateBookingService privateBookingService,
        CancellationToken cancellationToken)
    {
        where.OrganizationIds = where.OrganizationIds.RemoveInvalidIds();
        where.OrganizationUniqueAlphanumericNames = where.OrganizationUniqueAlphanumericNames.RemoveInvalidIds();
        where.LocationIds = where.LocationIds.RemoveInvalidIds();
        where.TeamIds = where.TeamIds.RemoveInvalidIds();
        where.CustomerIds = where.CustomerIds.RemoveInvalidIds();

        var (paginatedInfo, edges, totalCount) = await privateBookingService.GetPaginatedBookingsAsync(
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
                where.Category,
                where.PaymentStatuses.ToSafeCollection(),
                where.IncludeMineOnly,
                where.IncludeFutureBookingsOnly,
                where.OrganizationIds.ToSafeCollection(),
                where.OrganizationUniqueAlphanumericNames.ToSafeCollection(),
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
        [Service] IPrivateBookingService privateBookingService,
        CancellationToken cancellationToken)
    {
        var result = await BookingsAsync(null, null, null, null, where, [], privateBookingService, cancellationToken);
        return result.Edges.Select(item => item.Node);
    }
}
