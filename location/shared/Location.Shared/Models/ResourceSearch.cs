using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public class ResourceSearchCriteria
{
    public ResourceSearchCriteria(string locationId, string? nameContains, IEnumerable<string> tagIds)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        LocationId = locationId;
        NameContains = nameContains;
        TagIds = tagIds.ToList();
    }

    public string LocationId { get; }
    public string? NameContains { get; }
    public ICollection<string> TagIds { get; }
}

public record ResourceOrder(OrderDirection Direction, ResourceOrderField Field);

public enum ResourceOrderField
{
    Name
}
