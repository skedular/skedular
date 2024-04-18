using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public class DeskSearchCriteria
{
    public DeskSearchCriteria(string locationId, string? nameContains)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        LocationId = locationId;
        NameContains = nameContains;
    }

    public string LocationId { get; }
    public string? NameContains { get; }
}

public record DeskOrder(OrderDirection Direction, DeskOrderField Field);

public enum DeskOrderField
{
    Name
}
