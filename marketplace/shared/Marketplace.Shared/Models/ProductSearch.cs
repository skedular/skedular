using Enterprise.Shared.Pagination;

namespace Marketplace.Shared.Models;

public class ProductSearchCriteria(IEnumerable<string> organizationIds, IEnumerable<string> productIds, string? nameContains, bool includeInactive)
{
    public ICollection<string> OrganizationIds { get; } = organizationIds.ToList();
    public ICollection<string> ProductIds { get; } = productIds.ToList();
    public string? NameContains { get; } = nameContains;
    public bool IncludeInactive { get; } = includeInactive;
}

public record ProductOrder(OrderDirection Direction, ProductOrderField Field);

public enum ProductOrderField
{
    Name
}
