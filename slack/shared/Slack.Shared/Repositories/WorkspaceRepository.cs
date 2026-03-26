using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Slack.Shared.Database;
using Slack.Shared.Database.Entities;

namespace Slack.Shared.Repositories;

public interface IWorkspaceRepository : IRepository<Workspace>
{
    Task<Workspace?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Workspace?> GetByWorkspaceMemberIdAsync(string workspaceMemberId, CancellationToken cancellationToken);
    Task<Workspace?> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
    Task<ICollection<Workspace>> GetAllAsync(CancellationToken cancellationToken);
    Workspace Add(Workspace workspace);
    Workspace Update(Workspace workspace);
}

internal static class WorkspaceExtensions
{
    extension(IQueryable<Workspace> originalQuery)
    {
        internal IIncludableQueryable<Workspace, ICollection<WorkspaceMember>> AddDependentObjects() =>
            originalQuery
                .Include(query => query.Organization)
                .ThenInclude(query => query.OrganizationMembers)
                .ThenInclude(query => query.Customer)
                .Include(query => query.Channels)
                .Include(query => query.WorkspaceMembers);
    }
}

public class WorkspaceRepository(SlackDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<SlackDbContext, Workspace>(dbContext, timeProvider), IWorkspaceRepository
{
    public async Task<Workspace?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Workspace
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<Workspace?> GetByWorkspaceMemberIdAsync(string workspaceMemberId, CancellationToken cancellationToken)
    {
        var workspaceMember = await DbContext.WorkspaceMember
            .Include(query => query.Workspace)
            .FirstOrDefaultAsync(query => query.Id == workspaceMemberId, cancellationToken);
        if (workspaceMember is null)
        {
            return null;
        }

        return await DbContext.Workspace
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == workspaceMember.Workspace.Id, cancellationToken);
    }

    public async Task<Workspace?> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken) =>
        await DbContext.Workspace
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Organization.Id == organizationId, cancellationToken);

    public async Task<ICollection<Workspace>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Workspace
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

    public Workspace Add(Workspace workspace)
    {
        var now = TimeProvider.GetUtcNow();
        workspace.CreatedAt = now;
        return DbContext.Workspace.Add(workspace).Entity;
    }

    public Workspace Update(Workspace workspace)
    {
        var now = TimeProvider.GetUtcNow();
        workspace.ModifiedAt = now;
        return DbContext.Workspace.Update(workspace).Entity;
    }
}
