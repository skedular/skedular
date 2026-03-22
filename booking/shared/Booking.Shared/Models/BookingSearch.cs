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
    ICollection<PaymentStatus> PaymentStatuses,
    bool? IncludeMineOnly,
    bool? IncludeFutureBookingsOnly,
    string? OrganizationId,
    string? OrganizationCustomDomain,
    ICollection<string> LocationIds,
    ICollection<string> TeamIds,
    ICollection<string> CustomerIds,
    ICollection<string> RecurringBookingIds);

public record BookingAccessScope(
    ICollection<string> OrganizationIds,
    ICollection<string> LocationIds,
    ICollection<string> TeamIds);

public record BookingOrder(OrderDirection Direction, BookingOrderField Field);

public enum BookingOrderField
{
    From,
    To,
    Notes,
    Category,
    Channel
}
