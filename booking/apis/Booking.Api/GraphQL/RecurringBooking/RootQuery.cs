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

namespace Booking.Api.GraphQL.RecurringBooking;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public async Task<RecurringBookingDetails?> RecurringBookingAsync(string id, [Service] IRecurringBookingService recurringBookingService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await recurringBookingService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<RecurringBookingDetails?> RecurringBookingByIdAsync(
        string id,
        [Service] IRecurringBookingService recurringBookingService,
        CancellationToken cancellationToken) =>
        await RecurringBookingAsync(id, recurringBookingService, cancellationToken);

    [UseResolverScope]
    public async Task<Connection<RecurringBookingEdge>> RecurringBookingsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        RecurringBookingWhereInput where,
        IEnumerable<RecurringBookingOrderInput>? orderBy,
        [Service] IRecurringBookingService recurringBookingService,
        CancellationToken cancellationToken)
    {
        where.TeamIds = where.TeamIds.RemoveInvalidIds();
        where.CustomerIds = where.CustomerIds.RemoveInvalidIds();

        var (paginatedInfo, edges, totalCount) = await recurringBookingService.GetPaginatedRecurringBookingsAsync(
            new PaginationInputParam(after, first, before, last),
            new RecurringBookingSearchCriteria(
                where.FromGt,
                where.FromGte,
                where.FromLt,
                where.FromLte,
                where.ToGt,
                where.ToGte,
                where.ToLt,
                where.ToLte,
                where.NameContains,
                where.Category,
                where.Channel,
                where.IncludeMineOnly,
                where.IncludeFutureBookingsOnly,
                where.OrganizationId,
                where.OrganizationCustomDomain,
                where.TeamIds.ToSafeCollection(),
                where.CustomerIds.ToSafeCollection()),
            orderBy.ToSafeCollection().Select(item => new RecurringBookingOrder(item.Direction, item.Field)).ToList(),
            false,
            cancellationToken);

        return new Connection<RecurringBookingEdge>
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
    public async Task<IEnumerable<RecurringBookingDetails>> AllRecurringBookingsAsync(
        RecurringBookingWhereInput where,
        [Service] IRecurringBookingService recurringBookingService,
        CancellationToken cancellationToken)
    {
        var result = await RecurringBookingsAsync(null, null, null, null, where, [], recurringBookingService, cancellationToken);
        return result.Edges.Select(item => item.Node);
    }
}
