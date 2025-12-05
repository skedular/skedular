using Api.Shared.Services.Models;
using Enterprise.Shared.Pagination;

namespace Organization.Shared.Models;

public record JoinInvitationSearchCriteria(
    string? OrganizationUniqueAlphanumericName,
    InvitationStatus? Status,
    string? InviteeId,
    ICollection<string>? CustomerEmails);

public record JoinOrganizationInvitationOrder(OrderDirection Direction, JoinOrganizationInvitationOrderField Field);

public enum JoinOrganizationInvitationOrderField
{
    CreatedAt,
    Status
}
