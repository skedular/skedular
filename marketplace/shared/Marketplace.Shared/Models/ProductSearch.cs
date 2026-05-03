using Enterprise.Shared.Pagination;

namespace Marketplace.Shared.Models;

public record ProductSearchCriteria(
    IReadOnlyList<string> OrganizationIds,
    IReadOnlyList<string> OrganizationCustomDomains,
    IReadOnlyList<string> ProductIds,
    bool IncludeInactive);

public record ProductOrder(OrderDirection Direction, ProductOrderField Field);

public enum ProductOrderField
{
    Name
}
