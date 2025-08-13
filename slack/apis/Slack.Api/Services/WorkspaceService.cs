using Enterprise.Shared.Database;
using Flurl;
using Slack.Api.Mappers;
using Slack.Shared.Configurations;
using Slack.Shared.Publishers;
using Slack.Shared.Repositories;
using Slack.Shared.Workflows.NewSlackWorkspaceJoined;
using SlackNet;

namespace Slack.Api.Services;

public interface IWorkspaceService
{
    Task<string> InstallAsync(string code, string? state, CancellationToken cancellationToken);
}

public class WorkspaceService(
    SlackConfigurationService slackConfigurationService,
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IWorkspaceOnboardingService workspaceOnboardingService,
    IMapper mapper,
    ITemporalOutboxPublisher temporalOutboxPublisher)
    : IWorkspaceService
{
    public async Task<string> InstallAsync(string code, string? state, CancellationToken cancellationToken)
    {
        var response = await new SlackServiceBuilder().GetApiClient().OAuthV2.Access(
            slackConfigurationService.ClientId,
            slackConfigurationService.ClientSecret,
            code,
            null,
            slackConfigurationService.RedirectUrl!.ToString(),
            null,
            cancellationToken);
        ArgumentNullException.ThrowIfNull(response.Team);

        var organization = await repositoryFactory.OrganizationRepository.GetByWorkspaceIdAsync(response.Team.Id, cancellationToken);
        if (organization is null)
        {
            await workspaceOnboardingService.OnboardAsync(response, cancellationToken);
        }
        else
        {
            await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

            var workspace = await repositoryFactory.WorkspaceRepository.GetByIdAsync(response.Team.Id, cancellationToken);
            ArgumentNullException.ThrowIfNull(workspace);

            workspace = repositoryFactory.WorkspaceRepository.Update(mapper.MergeTo(response, workspace, organization));
            temporalOutboxPublisher.StartWorkflowNewSlackWorkspaceJoined(
                new NewSlackWorkspaceJoinedInput(workspace.Id),
                repositoryFactory.UnitOfWork);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        return slackConfigurationService.SuccessInstallUrl!.ToString().SetQueryParam("app", slackConfigurationService.AppId);
    }
}
