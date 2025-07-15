using Api.Shared.Services.Models;
using Enterprise.Shared.Pagination;

namespace Team.Shared.Models;

public class JoinInvitationSearchCriteria(string? organizationId, string? teamId, InvitationStatus? status)
{
    public string? InviteeId { get; set; }
    public string? OrganizationId { get; set; } = organizationId;
    public string? TeamId { get; set; } = teamId;
    public InvitationStatus? Status { get; set; } = status;
}

public record JoinTeamInvitationOrder(OrderDirection Direction, JoinTeamInvitationOrderField Field);

public enum JoinTeamInvitationOrderField
{
    CreatedAt,
    Status
}
