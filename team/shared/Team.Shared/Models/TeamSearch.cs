using Enterprise.Shared.Pagination;

namespace Team.Shared.Models;

public class TeamSearchCriteria(string? organizationId, string? customerId, string? nameContains, ICollection<string>? primaryLocationIds)
{
    public string? CustomerId { get; set; } = customerId;
    public string? OrganizationId { get; } = organizationId;
    public string? NameContains { get; } = nameContains;
    public ICollection<string>? PrimaryLocationIds { get; } = primaryLocationIds;
}

public record TeamOrder(OrderDirection Direction, TeamOrderField Field);

public enum TeamOrderField
{
    Name,
    About
}
