using Enterprise.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Slack.Shared.Database;
using Slack.Shared.Database.Entities;

namespace Slack.Shared.Repositories;

public interface IWorkspaceChannelRepository : IRepository<WorkspaceChannel>
{
    Task<WorkspaceChannel?> GetByIdAsync(string id, CancellationToken cancellationToken);
    WorkspaceChannel Add(WorkspaceChannel workspaceChannel);
    WorkspaceChannel Update(WorkspaceChannel workspaceChannel);
    WorkspaceChannel Remove(WorkspaceChannel workspaceChannel);
    void RemoveRange(ICollection<WorkspaceChannel> workspaceChannels);
}

public class WorkspaceChannelRepository(SlackDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<SlackDbContext, WorkspaceChannel>(dbContext), IWorkspaceChannelRepository
{
    public async Task<WorkspaceChannel?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.WorkspaceChannel
            .Where(query => query.Id == id)
            .Include(query => query.Workspace)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public WorkspaceChannel Add(WorkspaceChannel workspaceChannel)
    {
        var now = timeProvider.GetUtcNow();
        workspaceChannel.CreatedAt = now;
        return DbContext.WorkspaceChannel.Add(workspaceChannel).Entity;
    }

    public WorkspaceChannel Update(WorkspaceChannel workspaceChannel)
    {
        var now = timeProvider.GetUtcNow();
        workspaceChannel.ModifiedAt = now;
        return DbContext.WorkspaceChannel.Update(workspaceChannel).Entity;
    }

    public WorkspaceChannel Remove(WorkspaceChannel workspaceChannel)
    {
        var now = timeProvider.GetUtcNow();
        workspaceChannel.DeletedAt = now;
        return DbContext.WorkspaceChannel.Update(workspaceChannel).Entity;
    }

    public void RemoveRange(ICollection<WorkspaceChannel> workspaceChannels)
    {
        var now = timeProvider.GetUtcNow();
        workspaceChannels.ForEach(workspaceChannel => workspaceChannel.DeletedAt = now);
        DbContext.WorkspaceChannel.UpdateRange(workspaceChannels);
    }
}
