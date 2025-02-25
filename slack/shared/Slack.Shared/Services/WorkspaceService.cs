using Slack.Shared.Mappers;
using Slack.Shared.Repositories;

namespace Slack.Shared.Services;

public interface IWorkspaceService
{
    Task RefreshWorkspaceAsync(string workspaceId, CancellationToken cancellationToken);
}

public class WorkspaceService(
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider) : IWorkspaceService
{
    public async Task RefreshWorkspaceAsync(string workspaceId, CancellationToken cancellationToken)
    {
        var existingWorkspace = await repositoryFactory.WorkspaceRepository.GetByIdAsync(workspaceId, cancellationToken);
        if (existingWorkspace is null)
        {
            return;
        }

        var team = await existingWorkspace.GetApiClient().Team.Info(cancellationToken);

        existingWorkspace = mapper.MergeToEntity(team, existingWorkspace);
        existingWorkspace.LastRefreshedAt = timeProvider.GetUtcNow();

        await repositoryFactory.WorkspaceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
