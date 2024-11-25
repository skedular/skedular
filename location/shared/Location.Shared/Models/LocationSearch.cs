using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public class LocationSearchCriteria(
    string? organizationId,
    string? nameContains,
    string[] zoneIds,
    string[] deskTypeIds)
{
    public string? CustomerId { get; set; }
    public string? OrganizationId { get; } = organizationId;
    public string? NameContains { get; } = nameContains;
    public string[] ZoneIds { get; set; } = zoneIds;
    public string[] DeskTypeIds { get; set; } = deskTypeIds;
}

public record LocationOrder(OrderDirection Direction, LocationOrderField Field);

public enum LocationOrderField
{
    Name,
    About,
    Timezone
}
