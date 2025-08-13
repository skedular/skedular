using Enterprise.Shared.Database;
using Slack.Shared.Repositories;
using Slack.Shared.Services;
using Slack.Shared.Workflows.ReSyncSlackWorkspace;
using Temporalio.Activities;

namespace Slack.Shared.Activities;

public class SlackIntegrations(
    IRepositoryFactory repositoryFactory,
    IWorkspaceService workspaceService,
    IWorkspaceMemberService workspaceMemberService,
    IWorkspaceChannelService workspaceChannelService,
    TimeProvider timeProvider,
    ITemporalService temporalService)
{
    [Activity]
    public async Task<bool> ReSyncWorkspaceAsync(string workspaceId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var workspace = await repositoryFactory.WorkspaceRepository.GetByIdAsync(workspaceId, cancellationToken);
        if (workspace is null || workspace.IsDeleted())
        {
            return false;
        }

        await workspaceService.ReSyncWorkspaceAsync(workspaceId, cancellationToken);

        return true;
    }

    [Activity]
    public async Task<bool> ReSyncWorkspaceMembersAsync(string workspaceId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var workspace = await repositoryFactory.WorkspaceRepository.GetByIdAsync(workspaceId, cancellationToken);
        if (workspace is null || workspace.IsDeleted())
        {
            return false;
        }

        await workspaceMemberService.ReSyncWorkspaceMembersAsync(workspaceId, cancellationToken);

        return true;
    }

    [Activity]
    public async Task<bool> ReSyncWorkspaceChannelsAsync(string workspaceId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var workspace = await repositoryFactory.WorkspaceRepository.GetByIdAsync(workspaceId, cancellationToken);
        if (workspace is null || workspace.IsDeleted())
        {
            return false;
        }

        await workspaceChannelService.ReSyncWorkspaceChannelsAsync(workspaceId, cancellationToken);

        return true;
    }

    [Activity]
    public async Task ExecuteNextReSyncWorkspaceWorkflowAsync(string workspaceId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        await temporalService.StartWorkflowReSyncSlackWorkspaceAsync(
            new ReSyncSlackWorkspaceInput(workspaceId, timeProvider.GetUtcNow().AddDays(1)),
            cancellationToken);
    }
}
