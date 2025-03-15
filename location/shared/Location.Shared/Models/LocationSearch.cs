using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public class LocationSearchCriteria(string? organizationId, IEnumerable<string> locationIds, string? nameContains, IEnumerable<string> tagIds)
{
    public string? CustomerId { get; set; }
    public string? OrganizationId { get; } = organizationId;
    public ICollection<string> LocationIds { get; } = locationIds.ToList();
    public string? NameContains { get; } = nameContains;
    public ICollection<string> TagIds { get; set; } = tagIds.ToList();
}

public record LocationOrder(OrderDirection Direction, LocationOrderField Field);

public enum LocationOrderField
{
    Name,
    About,
    Timezone
}
