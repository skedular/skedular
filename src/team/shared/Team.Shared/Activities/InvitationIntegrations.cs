using Api.Shared.Services.Models;
using Microsoft.Extensions.Logging;
using Team.Shared.Repositories;
using Temporalio.Activities;

namespace Team.Shared.Activities;

public class InvitationIntegrations(IRepositoryFactory repositoryFactory, ILogger<InvitationIntegrations> logger)
{
    [Activity]
    public async Task ExpireTeamInvitationAsync(string joinInvitationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(joinInvitationId, cancellationToken);
        if (joinInvitation is null || joinInvitation.Status != InvitationStatusConstants.Pending)
        {
            // Invitation doesn't exist or was already processed
            logger.LogInformation("Expire invitation activity skipped for invitation {JoinInvitationId}", joinInvitationId);
            return;
        }

        // Update invitation status to Expired
        joinInvitation.Status = InvitationStatusConstants.Expired;
        repositoryFactory.JoinInvitationRepository.Update(joinInvitation);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Expire invitation activity completed for invitation {JoinInvitationId}", joinInvitationId);
    }
}
