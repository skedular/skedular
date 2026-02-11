using Api.Shared.Services.Models;

namespace Slack.Shared.Models;

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
    ICollection<PaymentStatus> PaymentStatuses,
    bool? IncludeMineOnly,
    bool? IncludeFutureBookingsOnly,
    ICollection<string> OrganizationIds,
    ICollection<string> LocationIds,
    ICollection<string> TeamIds,
    ICollection<string> CustomerIds);
