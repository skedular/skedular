using Enterprise.Shared.Database;
using Slack.Api.Mappers;
using Slack.Shared.Configurations;
using Slack.Shared.Models;
using Slack.Shared.Publishers;
using Slack.Shared.Repositories;
using SlackNet;

namespace Slack.Api.Services;

public interface IWorkspaceService
{
    Task<Uri> InstallAsync(string code, string state, CancellationToken cancellationToken);
    Task<Workspace> AddAsync(Workspace workspace, CancellationToken cancellationToken);
}

public class WorkspaceService(
    SlackConfiguration slackConfiguration,
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IWorkspaceOnboardingService workspaceOnboardingService,
    IMapper mapper,
    ISlackInternalOutboxPublisher slackInternalOutboxPublisher)
    : IWorkspaceService
{
    public async Task<Uri> InstallAsync(string code, string state, CancellationToken cancellationToken)
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
        var organization =
            await repositoryFactory.OrganizationRepository.GetByWorkspaceIdAsync(response.Team.Id, cancellationToken);
        if (organization is null)
        {
            await workspaceOnboardingService.OnboardAsync(response, cancellationToken);
        }
        else
        {
            await using var transaction =
                await transactionBuilder.BeginTransactionAsync(repositoryFactory.OrganizationRepository.UnitOfWork,
                    cancellationToken);

            var workspace =
                await repositoryFactory.WorkspaceRepository.GetByIdAsync(response.Team.Id, cancellationToken);
            ArgumentNullException.ThrowIfNull(workspace);
            workspace = repositoryFactory.WorkspaceRepository.Update(mapper.MergeTo(response, workspace, organization));
            await slackInternalOutboxPublisher.PublishRefreshWorkspaceMembersAsync(
                [workspace.Id],
                repositoryFactory.WorkspaceRepository.UnitOfWork,
                cancellationToken);
            await slackInternalOutboxPublisher.PublishRefreshWorkspaceChannelsAsync(
                [workspace.Id],
                repositoryFactory.WorkspaceRepository.UnitOfWork,
                cancellationToken);
            await repositoryFactory.WorkspaceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        return slackConfiguration.SuccessInstallUrl!;
    }

    public async Task<Workspace> AddAsync(Workspace workspace, CancellationToken cancellationToken)
    {
        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.WorkspaceRepository.UnitOfWork,
                cancellationToken);

        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
            workspace.Organization.Id,
            cancellationToken);
        var existingWorkspace =
            await repositoryFactory.WorkspaceRepository.GetByIdAsync(workspace.Id, cancellationToken) ??
            repositoryFactory.WorkspaceRepository.Add(mapper.MapToEntity(workspace, organization));

        await slackInternalOutboxPublisher.PublishRefreshWorkspaceMembersAsync(
            [workspace.Id],
            repositoryFactory.WorkspaceRepository.UnitOfWork,
            cancellationToken);
        await slackInternalOutboxPublisher.PublishRefreshWorkspaceChannelsAsync(
            [workspace.Id],
            repositoryFactory.WorkspaceRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.WorkspaceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(existingWorkspace);
    }
}
