using Enterprise.Shared.Pagination;

namespace Marketplace.Shared.Models;

public class ProductSearchCriteria(string? organizationId, IEnumerable<string> productIds, string? nameContains)
{
    public string? OrganizationId { get; } = organizationId;
    public ICollection<string> ProductIds { get; } = productIds.ToList();
    public string? NameContains { get; } = nameContains;
}

public record ProductOrder(OrderDirection Direction, ProductOrderField Field);

public enum ProductOrderField
{
    Name
}
