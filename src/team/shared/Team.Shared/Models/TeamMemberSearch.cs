using Enterprise.Shared.Pagination;

namespace Team.Shared.Models;

public record TeamMemberSearchCriteria(string TeamId, string? NameContains);

public record TeamMemberOrder(OrderDirection Direction, TeamMemberOrderField Field);

public enum TeamMemberOrderField
{
    Role,
    Status,
    Name,
    GivenName,
    MiddleName,
    FamilyName
}
