using Enterprise.Shared.Pagination;

namespace Team.Shared.Models;

public class JoinInvitationSearchCriteria(string? organizationId, string? teamId)
{
    public string? InviteeId { get; set; }
    public string? OrganizationId { get; set; } = organizationId;
    public string? TeamId { get; set; } = teamId;
}

public record JoinTeamInvitationOrder(OrderDirection Direction, JoinTeamInvitationOrderField Field);

public enum JoinTeamInvitationOrderField
{
    CreatedAt
}
