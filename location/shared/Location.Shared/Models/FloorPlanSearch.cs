using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public class FloorPlanSearchCriteria(string locationId)
{
    public string LocationId { get; set; } = locationId;
}

public record FloorPlanOrder(OrderDirection Direction, FloorPlanOrderField Field);

public enum FloorPlanOrderField
{
    Name
}
