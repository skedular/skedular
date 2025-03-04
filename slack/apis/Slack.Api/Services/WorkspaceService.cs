using Enterprise.Shared.Database;
using Flurl;
using Slack.Api.Mappers;
using Slack.Shared.Configurations;
using Slack.Shared.Models;
using Slack.Shared.Publishers;
using Slack.Shared.Repositories;
using SlackNet;

namespace Slack.Api.Services;

public interface IWorkspaceService
{
    Task<string> InstallAsync(string code, string? state, CancellationToken cancellationToken);
    Task<Workspace> AddAsync(Workspace workspace, CancellationToken cancellationToken);
}

public class WorkspaceService(
    SlackConfiguration slackConfiguration,
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IWorkspaceOnboardingService workspaceOnboardingService,
    IMapper mapper,
    ISlackInternalOutboxPublisher slackInternalOutboxPublisher,
    INotificationOutboxPublisher notificationOutboxPublisher)
    : IWorkspaceService
{
    public async Task<string> InstallAsync(string code, string? state, CancellationToken cancellationToken)
    {
        var response = await new SlackServiceBuilder().GetApiClient().OAuthV2.Access(
            slackConfiguration.ClientId,
            slackConfiguration.ClientSecret,
            code,
            null,
            slackConfiguration.RedirectUrl!.ToString(),
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
            await using var transaction =
                await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

            var workspace = await repositoryFactory.WorkspaceRepository.GetByIdAsync(response.Team.Id, cancellationToken);
            ArgumentNullException.ThrowIfNull(workspace);

            workspace = repositoryFactory.WorkspaceRepository.Update(mapper.MergeTo(response, workspace, organization));
            await slackInternalOutboxPublisher.PublishRefreshWorkspaceAsync([workspace.Id], repositoryFactory.UnitOfWork, cancellationToken);
            await slackInternalOutboxPublisher.PublishRefreshWorkspaceMembersAsync([workspace.Id], repositoryFactory.UnitOfWork, cancellationToken);
            await slackInternalOutboxPublisher.PublishRefreshWorkspaceChannelsAsync([workspace.Id], repositoryFactory.UnitOfWork, cancellationToken);
            await notificationOutboxPublisher.PublishNewSlackWorkspaceJoinedSubmittedAsync(
                mapper.MapTo(workspace),
                repositoryFactory.UnitOfWork,
                cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        return slackConfiguration.SuccessInstallUrl!.ToString().SetQueryParam("app", slackConfiguration.AppId);
    }

    public async Task<Workspace> AddAsync(Workspace workspace, CancellationToken cancellationToken)
    {
        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(workspace.Organization.Id, cancellationToken);
        var existingWorkspace = await repositoryFactory.WorkspaceRepository.GetByIdAsync(workspace.Id, cancellationToken) ??
                                repositoryFactory.WorkspaceRepository.Add(mapper.MapToEntity(workspace, organization));

        await slackInternalOutboxPublisher.PublishRefreshWorkspaceMembersAsync([workspace.Id], repositoryFactory.UnitOfWork, cancellationToken);
        await slackInternalOutboxPublisher.PublishRefreshWorkspaceChannelsAsync([workspace.Id], repositoryFactory.UnitOfWork, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(existingWorkspace);
    }
}
