using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public class LocationSearchCriteria(string? organizationId, string? nameContains)
{
    public string? CustomerId { get; set; }
    public string? OrganizationId { get; } = organizationId;
    public string? NameContains { get; } = nameContains;
}

public record LocationOrder(OrderDirection Direction, LocationOrderField Field);

public enum LocationOrderField
{
    Name,
    About,
    Timezone
}
