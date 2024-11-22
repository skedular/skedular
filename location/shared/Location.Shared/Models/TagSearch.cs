using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public class TagSearchCriteria
{
    public TagSearchCriteria(string locationId, string? type, string? nameContains)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        LocationId = locationId;
        Type = type;
        NameContains = nameContains;
    }

    public string LocationId { get; }
    public string? Type { get; }
    public string? NameContains { get; }
}

public record TagOrder(OrderDirection Direction, TagOrderField Field);

public enum TagOrderField
{
    Name,
    Description,
    TagType
}
