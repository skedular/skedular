using Enterprise.Shared.Pagination;

namespace Location.Shared.Models;

public record ResourceSearchCriteria(string LocationId, string? NameContains, ICollection<string> TagIds, string? FloorPlanId);

public record ResourceOrder(OrderDirection Direction, ResourceOrderField Field);

public enum ResourceOrderField
{
    Name
}
