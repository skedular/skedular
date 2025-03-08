using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public class RoomSearchCriteria
{
    public RoomSearchCriteria(string locationId, string? nameContains, ICollection<string>? tagIds)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        LocationId = locationId;
        NameContains = nameContains;
        TagIds = tagIds ?? [];
    }

    public string LocationId { get; }
    public string? NameContains { get; }
    public ICollection<string> TagIds { get; }
}

public record RoomOrder(OrderDirection Direction, RoomOrderField Field);

public enum RoomOrderField
{
    Name
}
