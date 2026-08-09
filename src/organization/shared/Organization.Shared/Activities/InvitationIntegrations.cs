using Api.Shared.Services.Models;
using Organization.Shared.Repositories;
using Temporalio.Activities;

namespace Organization.Shared.Activities;

public class InvitationIntegrations(IRepositoryFactory repositoryFactory)
{
    [Activity]
    public virtual async Task ExpireOrganizationInvitationAsync(string joinInvitationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(joinInvitationId, cancellationToken);
        if (joinInvitation is null || joinInvitation.Status != InvitationStatusConstants.Pending)
        {
            // Invitation doesn't exist or was already processed
            return;
        }

        // Update invitation status to Expired
        joinInvitation.Status = InvitationStatusConstants.Expired;
        repositoryFactory.JoinInvitationRepository.Update(joinInvitation);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
