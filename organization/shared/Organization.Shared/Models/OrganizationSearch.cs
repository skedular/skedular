using Enterprise.Shared.Pagination;

namespace Organization.Shared.Models;

public class OrganizationSearchCriteria(string? nameContains)
{
    public string CustomerId { get; set; } = string.Empty;
    public string? NameContains { get; } = nameContains;
}

public record OrganizationOrder(OrderDirection Direction, OrganizationOrderField Field);

public enum OrganizationOrderField
{
    Name
}
