using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public class DeskSearchCriteria
{
    public DeskSearchCriteria(
        string locationId,
        string? nameContains,
        ICollection<string>? zoneIds,
        ICollection<string>? customTagIds)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        LocationId = locationId;
        NameContains = nameContains;
        ZoneIds = zoneIds ?? [];
        CustomTagIds = customTagIds ?? [];
    }

    public string LocationId { get; }
    public string? NameContains { get; }
    public ICollection<string> ZoneIds { get; }
    public ICollection<string> CustomTagIds { get; }
}

public record DeskOrder(OrderDirection Direction, DeskOrderField Field);

public enum DeskOrderField
{
    Name
}
