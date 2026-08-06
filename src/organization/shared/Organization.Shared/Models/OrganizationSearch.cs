using Enterprise.Shared.Pagination;

namespace Organization.Shared.Models;

public record OrganizationSearchCriteria(string? NameContains, string? CustomerId);

public record OrganizationOrder(OrderDirection Direction, OrganizationOrderField Field);

public enum OrganizationOrderField
{
    Name,
}
