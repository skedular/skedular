using Enterprise.Shared.Pagination;

namespace Marketplace.Shared.Models;

public record ProductSearchCriteria(
    ICollection<string> OrganizationUniqueAlphanumericNames,
    ICollection<string> ProductIds,
    bool IncludeInactive);

public record ProductOrder(OrderDirection Direction, ProductOrderField Field);

public enum ProductOrderField
{
    Name
}
