using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
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
    void RemoveRange(IEnumerable<WorkspaceChannel> workspaceChannels);
    Task<IReadOnlyList<WorkspaceChannel>> GetByWorkspaceIdAsync(string workspaceId, CancellationToken cancellationToken);
}

public class WorkspaceChannelRepository(SlackDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<SlackDbContext, WorkspaceChannel>(dbContext, timeProvider), IWorkspaceChannelRepository
{
    public async Task<WorkspaceChannel?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.WorkspaceChannel
            .Include(query => query.Workspace)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public WorkspaceChannel Add(WorkspaceChannel workspaceChannel)
    {
        var now = TimeProvider.GetUtcNow();
        workspaceChannel.CreatedAt = now;
        return DbContext.WorkspaceChannel.Add(workspaceChannel).Entity;
    }

    public WorkspaceChannel Update(WorkspaceChannel workspaceChannel)
    {
        var now = TimeProvider.GetUtcNow();
        workspaceChannel.ModifiedAt = now;
        return DbContext.WorkspaceChannel.Update(workspaceChannel).Entity;
    }

    public WorkspaceChannel Remove(WorkspaceChannel workspaceChannel)
    {
        var now = TimeProvider.GetUtcNow();
        workspaceChannel.DeletedAt = now;
        return DbContext.WorkspaceChannel.Update(workspaceChannel).Entity;
    }

    public void RemoveRange(IEnumerable<WorkspaceChannel> workspaceChannels)
    {
        var now = TimeProvider.GetUtcNow();
        DbContext.WorkspaceChannel.UpdateRange(workspaceChannels.Select(item =>
        {
            item.DeletedAt = now;
            return item;
        }));
    }

    public async Task<IReadOnlyList<WorkspaceChannel>> GetByWorkspaceIdAsync(string workspaceId, CancellationToken cancellationToken) =>
        await DbContext.WorkspaceChannel
            .Where(query => query.Workspace.Id == workspaceId)
            .ToListAsync(cancellationToken);
}
