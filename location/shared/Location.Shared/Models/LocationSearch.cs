using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public class LocationSearchCriteria(
    string? organizationId,
    string[]? locationIds,
    string? nameContains,
    string[] zoneIds,
    string[] customTagIds)
{
    public string? CustomerId { get; set; }
    public string? OrganizationId { get; } = organizationId;
    public string[]? LocationIds { get; } = locationIds;
    public string? NameContains { get; } = nameContains;
    public string[] ZoneIds { get; set; } = zoneIds;
    public string[] CustomTagIds { get; set; } = customTagIds;
}

public record LocationOrder(OrderDirection Direction, LocationOrderField Field);

public enum LocationOrderField
{
    Name,
    About,
    Timezone
}
