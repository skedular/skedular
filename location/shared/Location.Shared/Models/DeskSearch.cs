using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public class DeskSearchCriteria
{
    public DeskSearchCriteria(string locationId, string? nameContains, IEnumerable<string> tagIds)
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

public record DeskOrder(OrderDirection Direction, DeskOrderField Field);

public enum DeskOrderField
{
    Name
}
