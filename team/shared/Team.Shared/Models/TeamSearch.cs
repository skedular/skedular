using Enterprise.Shared.Pagination;

namespace Team.Shared.Models;

public class TeamSearchCriteria(string? organizationId, string? nameContains)
{
    public string? CustomerId { get; set; }
    public string? OrganizationId { get; } = organizationId;
    public string? NameContains { get; } = nameContains;
}

public record TeamOrder(OrderDirection Direction, TeamOrderField Field);

public enum TeamOrderField
{
    Name,
    About
}
