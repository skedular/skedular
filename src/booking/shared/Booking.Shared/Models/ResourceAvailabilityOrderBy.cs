using Enterprise.Shared.Pagination;

namespace Booking.Shared.Models;

/// <summary>
///     Specifies the field to sort resource availability day view results by.
/// </summary>
public enum ResourceAvailabilityOrderByField
{
    ResourceName,
    ResourceType,
    LocationName,
    ZoneName,
}

/// <summary>
///     A single sort clause for the resource availability day view query.
///     Multiple clauses may be combined; the first clause is the primary sort,
///     subsequent clauses act as tie-breakers.
/// </summary>
/// <param name="Direction">Ascending or Descending.</param>
/// <param name="Field">The field to sort by.</param>
public record ResourceAvailabilityOrder(OrderDirection Direction, ResourceAvailabilityOrderByField Field);
