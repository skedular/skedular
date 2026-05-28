using Api.Shared.Services.Models;
using Enterprise.Shared.Pagination;

namespace Booking.Shared.Models;

public record RecurringBookingSearchCriteria(
    DateTimeOffset? FromGt,
    DateTimeOffset? FromGte,
    DateTimeOffset? FromLt,
    DateTimeOffset? FromLte,
    DateTimeOffset? ToGt,
    DateTimeOffset? ToGte,
    DateTimeOffset? ToLt,
    DateTimeOffset? ToLte,
    string? NameContains,
    BookingCategory? Category,
    BookingChannel? Channel,
    bool? IncludeMineOnly,
    bool? IncludeFutureBookingsOnly,
    string? OrganizationId,
    string? OrganizationCustomDomain,
    IReadOnlyList<string> TeamIds,
    IReadOnlyList<string> CustomerIds);

public record RecurringBookingAccessScope(
    IReadOnlyList<string> OrganizationIds,
    IReadOnlyList<string> TeamIds);

public record RecurringBookingOrder(OrderDirection Direction, RecurringBookingOrderField Field);

public enum RecurringBookingOrderField
{
    From,
    To,
    Category,
    Channel
}
