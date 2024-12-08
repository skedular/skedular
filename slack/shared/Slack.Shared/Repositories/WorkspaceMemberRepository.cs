using Enterprise.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Slack.Shared.Database;
using Slack.Shared.Database.Entities;

namespace Slack.Shared.Repositories;

public interface IWorkspaceMemberRepository : IRepository<WorkspaceMember>
{
    Task<WorkspaceMember?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<WorkspaceMember?> GetByAnyMatchingIdAsync(ICollection<string> ids, CancellationToken cancellationToken);
    WorkspaceMember Add(WorkspaceMember workspaceMember);
    WorkspaceMember Update(WorkspaceMember workspaceMember);
    WorkspaceMember Remove(WorkspaceMember workspaceMember);
    void RemoveRange(ICollection<WorkspaceMember> workspaceMembers);
}

internal static class WorkspaceMemberExtensions
{
    internal static IIncludableQueryable<WorkspaceMember, Organization> AddDependentObjects(
        this IQueryable<WorkspaceMember> originalQuery) =>
        originalQuery
            .Include(query => query.Workspace)
            .ThenInclude(query => query.Organization);
}

public class WorkspaceMemberRepository(SlackDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<SlackDbContext, WorkspaceMember>(dbContext, timeProvider), IWorkspaceMemberRepository
{
    public async Task<WorkspaceMember?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.WorkspaceMember
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<WorkspaceMember?> GetByAnyMatchingIdAsync(ICollection<string> ids,
        CancellationToken cancellationToken) =>
        await DbContext.WorkspaceMember
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => ids.Contains(query.Id), cancellationToken);

    public WorkspaceMember Add(WorkspaceMember workspaceMember)
    {
        var now = TimeProvider.GetUtcNow();
        workspaceMember.CreatedAt = now;
        return DbContext.WorkspaceMember.Add(workspaceMember).Entity;
    }

    public WorkspaceMember Update(WorkspaceMember workspaceMember)
    {
        var now = TimeProvider.GetUtcNow();
        workspaceMember.ModifiedAt = now;
        return DbContext.WorkspaceMember.Update(workspaceMember).Entity;
    }

    public WorkspaceMember Remove(WorkspaceMember workspaceMember)
    {
        var now = TimeProvider.GetUtcNow();
        workspaceMember.DeletedAt = now;
        return DbContext.WorkspaceMember.Update(workspaceMember).Entity;
    }

    public void RemoveRange(ICollection<WorkspaceMember> workspaceMembers)
    {
        var now = TimeProvider.GetUtcNow();
        workspaceMembers.ForEach(workspaceMember => workspaceMember.DeletedAt = now);
        DbContext.WorkspaceMember.UpdateRange(workspaceMembers);
    }
}
