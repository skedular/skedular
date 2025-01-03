using Enterprise.Shared.Pagination;

namespace Team.Shared.Models;

public class TeamMemberSearchCriteria
{
    public TeamMemberSearchCriteria(string teamId, string? nameContains)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

        TeamId = teamId;
        NameContains = nameContains;
    }

    public string TeamId { get; }
    public string? NameContains { get; }
}

public record TeamMemberOrder(OrderDirection Direction, TeamMemberOrderField Field);

public enum TeamMemberOrderField
{
    Role,
    Status,
    Name,
    GivenName,
    MiddleName,
    FamilyName,
    PhoneNumber
}
