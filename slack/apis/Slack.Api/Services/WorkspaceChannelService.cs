using Slack.Api.Mappers;
using Slack.Shared;
using Slack.Shared.Database.Entities;
using Slack.Shared.Repositories;
using Workspace = Slack.Shared.Database.Entities.Workspace;

namespace Slack.Api.Services;

public interface IWorkspaceChannelService
{
    Task<WorkspaceChannel> EnsureChannelResourcesAllExistAsync(
        Workspace workspace,
        string workspaceChannelId,
        CancellationToken cancellationToken);
}

public class WorkspaceChannelService(IRepositoryFactory repositoryFactory, IMapper mapper) : IWorkspaceChannelService
{
    public async Task<WorkspaceChannel> EnsureChannelResourcesAllExistAsync(
        Workspace workspace,
        string workspaceChannelId,
        CancellationToken cancellationToken)
    {
        var channel = workspace.Channels.FirstOrDefault(item => item.Id == workspaceChannelId);
        if (channel is not null)
        {
            return channel;
        }

        var slackApiClient = workspace.GetApiClient();
        var workspaceChannel = await slackApiClient.Conversations.Info(
            workspaceChannelId,
            true,
            false,
            cancellationToken);
        channel = repositoryFactory.WorkspaceChannelRepository.Add(mapper.MapTo(workspaceChannel, workspace));
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return channel;
    }
}
