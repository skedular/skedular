using Enterprise.Shared.Pagination;

namespace Organization.Shared.Models;

public class JoinInvitationSearchCriteria(string? organizationId)
{
    public string? InviteeId { get; set; }
    public string? OrganizationId { get; set; } = organizationId;
}

public record JoinOrganizationInvitationOrder(OrderDirection Direction, JoinOrganizationInvitationOrderField Field);

public enum JoinOrganizationInvitationOrderField
{
    CreatedAt
}
