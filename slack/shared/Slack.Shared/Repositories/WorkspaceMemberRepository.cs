using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Slack.Shared.Database;
using Slack.Shared.Database.Entities;

namespace Slack.Shared.Repositories;

public interface IWorkspaceMemberRepository : IRepository<WorkspaceMember>
{
    Task<WorkspaceMember?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<IReadOnlyList<WorkspaceMember>> GetForAutomaticProfileStatusUpdateAsync(
        DateTimeOffset now,
        CancellationToken cancellationToken);

    WorkspaceMember Add(WorkspaceMember workspaceMember);
    WorkspaceMember Update(WorkspaceMember workspaceMember);
    void RemoveRange(IEnumerable<WorkspaceMember> workspaceMembers);
    Task<IReadOnlyList<WorkspaceMember>> GetByWorkspaceIdAsync(string workspaceId, CancellationToken cancellationToken);
}

public static class WorkspaceMemberExtensions
{
    extension(IQueryable<WorkspaceMember> originalQuery)
    {
        public IIncludableQueryable<WorkspaceMember, Organization> AddDependentObjects() =>
            originalQuery
                .Include(query => query.Workspace)
                .ThenInclude(query => query.Organization);
    }
}

public class WorkspaceMemberRepository(SlackDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<SlackDbContext, WorkspaceMember>(dbContext, timeProvider), IWorkspaceMemberRepository
{
    public async Task<WorkspaceMember?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.WorkspaceMember
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    /// <summary>
    ///     Returns workspace members that are eligible for automatic Slack profile status updates.
    /// </summary>
    /// <param name="now">The current timestamp used to evaluate the update cadence.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The workspace members whose profile status should be refreshed automatically.</returns>
    /// <remarks>
    ///     This query replaced the shared specification used by the profile-status job and keeps the Slack capability checks close to the data they filter.
    /// </remarks>
    public async Task<IReadOnlyList<WorkspaceMember>> GetForAutomaticProfileStatusUpdateAsync(
        DateTimeOffset now,
        CancellationToken cancellationToken) =>
        await DbContext.WorkspaceMember
            .Where(query =>
                !query.DeletedAt.HasValue &&
                !query.Workspace.DeletedAt.HasValue &&
                query.AutomaticallyUpdateProfileStatus.HasValue &&
                query.AutomaticallyUpdateProfileStatus.Value &&
                (!query.LastProfileStatusUpdatedAt.HasValue ||
                 (now - query.LastProfileStatusUpdatedAt.Value).TotalHours >= 24) &&
                EF.Functions.ILike(query.Workspace.AuthedUserScope, "%users.profile:read%") &&
                EF.Functions.ILike(query.Workspace.AuthedUserScope, "%users.profile:write%"))
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

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

    public void RemoveRange(IEnumerable<WorkspaceMember> workspaceMembers)
    {
        var now = TimeProvider.GetUtcNow();
        workspaceMembers.ForEach(workspaceMember => workspaceMember.DeletedAt = now);
        DbContext.WorkspaceMember.UpdateRange(workspaceMembers);
    }

    public async Task<IReadOnlyList<WorkspaceMember>> GetByWorkspaceIdAsync(string workspaceId, CancellationToken cancellationToken) =>
        await DbContext.WorkspaceMember
            .Where(query => query.Workspace.Id == workspaceId)
            .ToListAsync(cancellationToken);
}
