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
    ICollection<string> OrganizationIds,
    ICollection<string> OrganizationUniqueAlphanumericNames,
    ICollection<string> LocationIds,
    ICollection<string> TeamIds,
    ICollection<string> CustomerIds);

public record BookingOrder(OrderDirection Direction, BookingOrderField Field);

public enum BookingOrderField
{
    From,
    To,
    Notes,
    Category,
    PaymentStatus,
    Channel
}
