using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public class DeskSearchCriteria
{
    public DeskSearchCriteria(
        string locationId,
        string? nameContains,
        ICollection<string>? zoneIds,
        ICollection<string>? deskTypeIds)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        LocationId = locationId;
        NameContains = nameContains;
        ZoneIds = zoneIds ?? [];
        DeskTypeIds = deskTypeIds ?? [];
    }

    public string LocationId { get; }
    public string? NameContains { get; }
    public ICollection<string> ZoneIds { get; }
    public ICollection<string> DeskTypeIds { get; }
}

public record DeskOrder(OrderDirection Direction, DeskOrderField Field);

public enum DeskOrderField
{
    Name
}
