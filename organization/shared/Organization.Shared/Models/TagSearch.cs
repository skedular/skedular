using Enterprise.Shared.Pagination;

namespace Organization.Shared.Models;

public record TagSearchCriteria(string? OrganizationId, string? OrganizationUniqueAlphanumericName, string? Type, string? NameContains);

public record TagOrder(OrderDirection Direction, OrganizationTagOrderField Field);

public enum OrganizationTagOrderField
{
    Name,
    Description,
    Type
}
