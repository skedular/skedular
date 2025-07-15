using Api.Shared.Services.Models;
using Enterprise.Shared.Pagination;

namespace Organization.Shared.Models;

public class JoinInvitationSearchCriteria(string? organizationId, InvitationStatus? status)
{
    public string? InviteeId { get; set; }
    public string? OrganizationId { get; set; } = organizationId;
    public InvitationStatus? Status { get; set; } = status;
}

public record JoinOrganizationInvitationOrder(OrderDirection Direction, JoinOrganizationInvitationOrderField Field);

public enum JoinOrganizationInvitationOrderField
{
    CreatedAt,
    Status
}
