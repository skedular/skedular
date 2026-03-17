using Enterprise.Shared.Pagination;

namespace Marketplace.Shared.Models;

public record ProductSearchCriteria(
    ICollection<string> OrganizationIds,
    ICollection<string> OrganizationCustomDomains,
    ICollection<string> ProductIds,
    bool IncludeInactive);

public record ProductOrder(OrderDirection Direction, ProductOrderField Field);

public enum ProductOrderField
{
    Name
}
