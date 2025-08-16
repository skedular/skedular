using Slack.Shared.Mappers;
using Slack.Shared.Repositories;
using SlackNet;
using SlackNet.WebApi;

namespace Slack.Shared.Services;

public interface IWorkspaceChannelService
{
    Task ReSyncWorkspaceChannelsAsync(string workspaceId, CancellationToken cancellationToken);
}

public class WorkspaceChannelService(IMapper mapper, IRepositoryFactory repositoryFactory) : IWorkspaceChannelService
{
    public async Task ReSyncWorkspaceChannelsAsync(string workspaceId, CancellationToken cancellationToken)
    {
        var existingWorkspace = await repositoryFactory.WorkspaceRepository.GetByIdAsync(workspaceId, cancellationToken);
        if (existingWorkspace is null)
        {
            return;
        }

        var nextCursor = string.Empty;
        var channels = new List<Conversation>();

        do
        {
            var response = await existingWorkspace.GetApiClient().Conversations.List(
                true,
                cursor: nextCursor,
                types: [ConversationType.PublicChannel],
                cancellationToken: cancellationToken);
            channels.AddRange(response.Channels);
            nextCursor = response.ResponseMetadata.NextCursor;
        } while (!string.IsNullOrWhiteSpace(nextCursor));

        var workspaceChannels = await repositoryFactory.WorkspaceChannelRepository.GetByWorkspaceIdAsync(workspaceId, cancellationToken);
        var itemsToRemove = workspaceChannels.Where(channel => channels.All(item => item.Id != channel.Id)).ToList();
        var updatedItems = workspaceChannels
            .Where(channel => channels.Any(item => item.Id == channel.Id))
            .Select(channel =>
            {
                var updatedWorkspaceChannel = mapper.MergeToEntity(channels.First(item => item.Id == channel.Id), channel, existingWorkspace);
                updatedWorkspaceChannel.DeletedAt = null;
                return repositoryFactory.WorkspaceChannelRepository.Update(updatedWorkspaceChannel);
            })
            .ToList();
        var addedItems = channels
            .Where(channel => workspaceChannels.All(item => item.Id != channel.Id))
            .Select(channel => repositoryFactory.WorkspaceChannelRepository.Add(mapper.MapToEntity(channel, existingWorkspace)))
            .ToList();

        repositoryFactory.WorkspaceChannelRepository.RemoveRange(itemsToRemove);
        existingWorkspace.Channels = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        repositoryFactory.WorkspaceRepository.Update(existingWorkspace);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
