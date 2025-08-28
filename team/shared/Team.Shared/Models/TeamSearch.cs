using Enterprise.Shared.Pagination;

namespace Team.Shared.Models;

public record TeamSearchCriteria(
    string? OrganizationId,
    string? OrganizationUniqueAlphanumericName,
    string? CustomerId,
    string? NameContains,
    ICollection<string> PrimaryLocationIds);

public record TeamOrder(OrderDirection Direction, TeamOrderField Field);

public enum TeamOrderField
{
    Name,
    About
}
