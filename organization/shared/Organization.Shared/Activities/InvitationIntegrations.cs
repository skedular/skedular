using Api.Shared.Services.Models;
using Organization.Shared.Repositories;
using Temporalio.Activities;

namespace Organization.Shared.Activities;

public class InvitationIntegrations(IRepositoryFactory repositoryFactory)
{
    [Activity]
    public async Task ExpireInvitationAsync(string organizationId, string inviterCustomerId, string inviteeCustomerId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByOrganizationInviterInviteeIdAsync(
            organizationId,
            inviterCustomerId,
            inviteeCustomerId,
            cancellationToken);
        if (joinInvitation is null)
        {
            // Invitation doesn't exist or was already processed
            return;
        }

        if (joinInvitation.Status != InvitationStatusConstants.Pending)
        {
            return;
        }

        // Update invitation status to Expired
        joinInvitation.Status = InvitationStatusConstants.Expired;
        repositoryFactory.JoinInvitationRepository.Update(joinInvitation);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
