namespace Booking.Shared.Models;

/// <summary>
///     Input filter for the resource availability day view query.
///     <see cref="Date" /> and <see cref="OrganizationCustomDomain" /> are required for tenancy scoping.
///     All other fields are optional; omitted fields are treated as "no constraint".
/// </summary>
public sealed record ResourceAvailabilityDayFilter
{
    public required DateOnly Date { get; set; }

    // Not marked as `required` so callers that haven't yet resolved the organization
    // (e.g. anonymous or partially-authenticated flows) can still construct the filter
    // without a compile error. Callers SHOULD populate this for correct tenancy scoping;
    // leaving it empty will cause the repository to return no rows (the WHERE clause
    // always filters by organization, so an empty string matches nothing).
    public string OrganizationCustomDomain { get; set; } = string.Empty; // tenancy scope
    public IReadOnlyList<string> LocationIds { get; set; } = [];
    public string? FloorId { get; set; }
    public string? ZoneId { get; set; }
    public string? ResourceType { get; set; } // tag constant
    public IReadOnlyList<ResourceAvailabilityClassification> Statuses { get; set; } = [];
}
