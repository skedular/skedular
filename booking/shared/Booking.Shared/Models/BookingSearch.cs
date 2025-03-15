using Enterprise.Shared.Pagination;

namespace Booking.Shared.Models;

// ReSharper disable InconsistentNaming
public class BookingSearchCriteria(
    DateTimeOffset? fromGT,
    DateTimeOffset? fromGTE,
    DateTimeOffset? fromLT,
    DateTimeOffset? fromLTE,
    DateTimeOffset? toGT,
    DateTimeOffset? toGTE,
    DateTimeOffset? toLT,
    DateTimeOffset? toLTE,
    string? notesContains,
    string? nameContains,
    string? bookingType,
    bool? includeMineOnly,
    bool? includeFutureBookingsOnly,
    bool? combineOrganizationsLocationsTeams,
    IEnumerable<string> organizationIds,
    IEnumerable<string> locationIds,
    IEnumerable<string> teamIds,
    IEnumerable<string> customerIds)
{
    public DateTimeOffset? FromGT { get; } = fromGT;
    public DateTimeOffset? FromGTE { get; } = fromGTE;
    public DateTimeOffset? FromLT { get; } = fromLT;
    public DateTimeOffset? FromLTE { get; } = fromLTE;
    public DateTimeOffset? ToGT { get; } = toGT;
    public DateTimeOffset? ToGTE { get; } = toGTE;
    public DateTimeOffset? ToLT { get; } = toLT;
    public DateTimeOffset? ToLTE { get; } = toLTE;
    public string? NotesContains { get; } = notesContains;
    public string? NameContains { get; } = nameContains;
    public bool? IncludeMineOnly { get; } = includeMineOnly;
    public bool? IncludeFutureBookingsOnly { get; } = includeFutureBookingsOnly;
    public bool? CombineOrganizationsLocationsTeams { get; } = combineOrganizationsLocationsTeams;
    public string? BookingType { get; } = bookingType;
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
    Name,
    GivenName,
    MiddleName,
    FamilyName,
    OrganizationName,
    LocationName,
    TeamName,
    BookingType
}
