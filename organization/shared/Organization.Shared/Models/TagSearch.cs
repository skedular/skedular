using Enterprise.Shared.Pagination;

namespace Organization.Shared.Models;

public record TagSearchCriteria(string? OrganizationId, string? OrganizationUniqueAlphanumericName, ICollection<string> Types, string? NameContains);

public record TagOrder(OrderDirection Direction, OrganizationTagOrderField Field);

public enum OrganizationTagOrderField
{
    Name,
    Description,
    Type
}
