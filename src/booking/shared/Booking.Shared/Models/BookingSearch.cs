using Api.Shared.Services.Models;
using Enterprise.Shared.Pagination;

namespace Booking.Shared.Models;

public record BookingSearchCriteria(
    DateTimeOffset? FromGt,
    DateTimeOffset? FromGte,
    DateTimeOffset? FromLt,
    DateTimeOffset? FromLte,
    DateTimeOffset? ToGt,
    DateTimeOffset? ToGte,
    DateTimeOffset? ToLt,
    DateTimeOffset? ToLte,
    string? NotesContains,
    string? NameContains,
    BookingCategory? Category,
    BookingChannel? Channel,
    IReadOnlyList<PaymentStatus> PaymentStatuses,
    bool? IncludeMineOnly,
    bool? IncludeFutureBookingsOnly,
    string? OrganizationId,
    string? OrganizationCustomDomain,
    IReadOnlyList<string> LocationIds,
    IReadOnlyList<string> TeamIds,
    IReadOnlyList<string> CustomerIds,
    IReadOnlyList<string> RecurringBookingIds);

public record BookingAccessScope(
    IReadOnlyList<string> OrganizationIds,
    IReadOnlyList<string> LocationIds,
    IReadOnlyList<string> TeamIds);

public record BookingOrder(OrderDirection Direction, BookingOrderField Field);

public enum BookingOrderField
{
    From,
    To,
    Notes,
    Category,
    Channel,
}
