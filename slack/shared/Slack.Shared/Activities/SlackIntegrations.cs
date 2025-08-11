using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Enterprise.Shared.Temporal.Configurations;
using Slack.Shared.Repositories;
using Slack.Shared.Services;
using Slack.Shared.Workflows.ReSyncSlackWorkspace;
using Temporalio.Activities;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Slack.Shared.Activities;

public class SlackIntegrations(
    TemporalConfiguration temporalConfiguration,
    IRepositoryFactory repositoryFactory,
    IWorkspaceService workspaceService,
    IWorkspaceMemberService workspaceMemberService,
    IWorkspaceChannelService workspaceChannelService,
    IRandomHelper randomHelper,
    ITemporalClient temporalClient,
    TimeProvider timeProvider)
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
        await temporalClient.StartWorkflowAsync(
            (ReSyncSlackWorkspace workflow) =>
                workflow.ExecuteAsync(new ReSyncSlackWorkspaceInput(workspaceId, timeProvider.GetUtcNow().AddDays(1))),
            new WorkflowOptions
            {
                Id = randomHelper.Generate(),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.RejectDuplicate,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });
    }
}
