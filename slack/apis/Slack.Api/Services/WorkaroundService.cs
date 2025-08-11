using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Enterprise.Shared.Temporal.Configurations;
using Slack.Shared.Repositories;
using Slack.Shared.Workflows.ReSyncSlackWorkspace;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Slack.Api.Services;

public interface IWorkaroundService
{
    Task ReSyncSlackWorkspace(string workspaceId, CancellationToken cancellationToken);
    Task ReSyncAllSlackWorkspaces(CancellationToken cancellationToken);
}

public class WorkaroundService(
    TemporalConfiguration temporalConfiguration,
    IRandomHelper randomHelper,
    ITemporalClient temporalClient,
    IRepositoryFactory repositoryFactory) : IWorkaroundService
{
    public async Task ReSyncSlackWorkspace(string workspaceId, CancellationToken cancellationToken)
    {
        var workspace = await repositoryFactory.WorkspaceRepository.GetByIdAsync(workspaceId, cancellationToken);
        if (workspace is null || workspace.IsDeleted())
        {
            return;
        }

        await temporalClient.StartWorkflowAsync(
            (ReSyncSlackWorkspace workflow) =>
                workflow.ExecuteAsync(new ReSyncSlackWorkspaceInput(workspaceId, null)),
            new WorkflowOptions
            {
                Id = randomHelper.Generate(),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.RejectDuplicate,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });
    }

    public async Task ReSyncAllSlackWorkspaces(CancellationToken cancellationToken)
    {
        var workspaces = await repositoryFactory.WorkspaceRepository.GetAllAsync(cancellationToken);

        foreach (var workspace in workspaces)
        {
            await temporalClient.StartWorkflowAsync(
                (ReSyncSlackWorkspace workflow) =>
                    workflow.ExecuteAsync(new ReSyncSlackWorkspaceInput(workspace.Id, null)),
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
}
