using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public class LocationSearchCriteria(string? organizationId, ICollection<string> locationIds, string? nameContains, ICollection<string> tagIds)
{
    public string? CustomerId { get; set; }
    public string? OrganizationId { get; } = organizationId;
    public ICollection<string> LocationIds { get; } = locationIds;
    public string? NameContains { get; } = nameContains;
    public ICollection<string> TagIds { get; set; } = tagIds;
}

public record LocationOrder(OrderDirection Direction, LocationOrderField Field);

public enum LocationOrderField
{
    Name,
    About,
    Timezone
}
