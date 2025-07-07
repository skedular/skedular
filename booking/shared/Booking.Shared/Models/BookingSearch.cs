using Api.Shared.Services.Models;
using Enterprise.Shared.Pagination;

namespace Booking.Shared.Models;

// ReSharper disable InconsistentNaming
public class BookingSearchCriteria(
    DateTimeOffset? fromGt,
    DateTimeOffset? fromGte,
    DateTimeOffset? fromLt,
    DateTimeOffset? fromLte,
    DateTimeOffset? toGt,
    DateTimeOffset? toGte,
    DateTimeOffset? toLt,
    DateTimeOffset? toLte,
    string? notesContains,
    string? nameContains,
    BookingType? type,
    IEnumerable<PaymentStatus> paymentStatuses,
    bool? includeMineOnly,
    bool? includeFutureBookingsOnly,
    IEnumerable<string> organizationIds,
    IEnumerable<string> locationIds,
    IEnumerable<string> teamIds,
    IEnumerable<string> customerIds)
{
    public DateTimeOffset? FromGt { get; } = fromGt;
    public DateTimeOffset? FromGte { get; } = fromGte;
    public DateTimeOffset? FromLt { get; } = fromLt;
    public DateTimeOffset? FromLte { get; } = fromLte;
    public DateTimeOffset? ToGt { get; } = toGt;
    public DateTimeOffset? ToGte { get; } = toGte;
    public DateTimeOffset? ToLt { get; } = toLt;
    public DateTimeOffset? ToLte { get; } = toLte;
    public string? NotesContains { get; } = notesContains;
    public string? NameContains { get; } = nameContains;
    public bool? IncludeMineOnly { get; } = includeMineOnly;
    public bool? IncludeFutureBookingsOnly { get; } = includeFutureBookingsOnly;
    public BookingType? Type { get; } = type;
    public ICollection<PaymentStatus> PaymentStatuses { get; } = paymentStatuses.ToList();
    public ICollection<string> OrganizationIds { get; set; } = organizationIds.ToList();
    public ICollection<string> LocationIds { get; set; } = locationIds.ToList();
    public ICollection<string> TeamIds { get; set; } = teamIds.ToList();
    public ICollection<string> CustomerIds { get; set; } = customerIds.ToList();
}
// ReSharper restore InconsistentNaming

public record BookingOrder(OrderDirection Direction, BookingOrderField Field);

public enum BookingOrderField
{
    From,
    To,
    Notes,
    Type,
    Status
}
