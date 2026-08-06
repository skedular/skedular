using Api.Shared.Services.Models;
using Enterprise.Shared.Pagination;

namespace Team.Shared.Models;

public record JoinInvitationSearchCriteria(string? OrganizationUniqueCustomDomain, string? TeamId, InvitationStatus? Status, string? InviteeId);

public record JoinTeamInvitationOrder(OrderDirection Direction, JoinTeamInvitationOrderField Field);

public enum JoinTeamInvitationOrderField
{
    CreatedAt,
    Status,
}
