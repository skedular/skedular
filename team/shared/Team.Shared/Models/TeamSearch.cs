using Enterprise.Shared;
using Enterprise.Shared.Pagination;

namespace Team.Shared.Models;

public class TeamSearchCriteria(string? organizationId, string? customerId, string? nameContains, IEnumerable<string>? primaryLocationIds)
{
    public string? CustomerId { get; set; } = customerId;
    public string? OrganizationId { get; } = organizationId;
    public string? NameContains { get; } = nameContains;
    public ICollection<string> PrimaryLocationIds { get; } = primaryLocationIds.ToSafeCollection();
}

public record TeamOrder(OrderDirection Direction, TeamOrderField Field);

public enum TeamOrderField
{
    Name,
    About
}
