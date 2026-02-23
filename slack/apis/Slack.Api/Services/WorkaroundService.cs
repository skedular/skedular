using Enterprise.Shared.Database;
using Slack.Shared.Repositories;
using Slack.Shared.Services;
using Slack.Shared.Workflows;

namespace Slack.Api.Services;

public interface IWorkaroundService
{
    Task ReSyncSlackWorkspaceAsync(string workspaceId, CancellationToken cancellationToken);
    Task ReSyncAllSlackWorkspacesAsync(CancellationToken cancellationToken);
}

public class WorkaroundService(IRepositoryFactory repositoryFactory, ITemporalService temporalService) : IWorkaroundService
{
    public async Task ReSyncSlackWorkspaceAsync(string workspaceId, CancellationToken cancellationToken)
    {
        var workspace = await repositoryFactory.WorkspaceRepository.GetByIdAsync(workspaceId, cancellationToken);
        if (workspace is null || workspace.IsDeleted())
        {
            return;
        }

        await temporalService.StartWorkflowReSyncSlackWorkspaceAsync(new ReSyncSlackWorkspaceInput(workspace.Id, null), cancellationToken);
    }

    public async Task ReSyncAllSlackWorkspacesAsync(CancellationToken cancellationToken)
    {
        var workspaces = await repositoryFactory.WorkspaceRepository.GetAllAsync(cancellationToken);

        foreach (var workspace in workspaces)
        {
            await temporalService.StartWorkflowReSyncSlackWorkspaceAsync(new ReSyncSlackWorkspaceInput(workspace.Id, null), cancellationToken);
        }
    }
}
