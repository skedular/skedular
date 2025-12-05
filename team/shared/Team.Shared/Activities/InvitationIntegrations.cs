using Api.Shared.Services.Models;
using Team.Shared.Repositories;
using Temporalio.Activities;

namespace Team.Shared.Activities;

public class InvitationIntegrations(IRepositoryFactory repositoryFactory)
{
    [Activity]
    public async Task ExpireInvitationAsync(string joinInvitationId)
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
