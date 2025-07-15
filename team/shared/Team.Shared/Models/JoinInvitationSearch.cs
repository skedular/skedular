using Enterprise.Shared.Pagination;

namespace Team.Shared.Models;

public class JoinInvitationSearchCriteria(string? teamId)
{
    public string? InviteeId { get; set; }
    public string? TeamId { get; set; } = teamId;
}

public record JoinTeamInvitationOrder(OrderDirection Direction, JoinTeamInvitationOrderField Field);

public enum JoinTeamInvitationOrderField
{
    CreatedAt
}
