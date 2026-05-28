using Enterprise.Shared.Pagination;

namespace Team.Shared.Models;

public record TeamSearchCriteria(
    string? OrganizationId,
    string? OrganizationCustomDomain,
    string? CustomerId,
    string? NameContains,
    IReadOnlyList<string> PrimaryLocationIds);

public record TeamOrder(OrderDirection Direction, TeamOrderField Field);

public enum TeamOrderField
{
    Name,
    About
}
