using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public record FloorPlanSearchCriteria(string LocationId);

public record FloorPlanOrder(OrderDirection Direction, FloorPlanOrderField Field);

public enum FloorPlanOrderField
{
    Name,
}
