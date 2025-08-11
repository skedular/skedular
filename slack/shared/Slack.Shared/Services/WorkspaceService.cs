using Slack.Shared.Mappers;
using Slack.Shared.Repositories;

namespace Slack.Shared.Services;

public interface IWorkspaceService
{
    Task ReSyncWorkspaceAsync(string workspaceId, CancellationToken cancellationToken);
}

public class WorkspaceService(IMapper mapper, IRepositoryFactory repositoryFactory) : IWorkspaceService
{
    public async Task ReSyncWorkspaceAsync(string workspaceId, CancellationToken cancellationToken)
    {
        var existingWorkspace = await repositoryFactory.WorkspaceRepository.GetByIdAsync(workspaceId, cancellationToken);
        if (existingWorkspace is null)
        {
            return;
        }

        var team = await existingWorkspace.GetApiClient().Team.Info(cancellationToken);

        _ = mapper.MergeToEntity(team, existingWorkspace);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
