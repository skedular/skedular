using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Slack.Shared.Database;
using Slack.Shared.Database.Entities;

namespace Slack.Shared.Repositories;

public interface IWorkspaceRepository : IRepository<Workspace>
{
    Task<Workspace?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Workspace?> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
    Workspace Add(Workspace workspace);
    Workspace Update(Workspace workspace);
    Workspace Remove(Workspace workspace);
}

internal static class WorkspaceExtensions
{
    internal static IIncludableQueryable<Workspace, ICollection<WorkspaceMember>> AddDependentObjects(
        this IQueryable<Workspace> originalQuery) =>
        originalQuery
            .Include(query => query.Organization)
            .ThenInclude(query => query.OrganizationMembers)
            .ThenInclude(query => query.Customer)
            .Include(query => query.Channels)
            .Include(query => query.WorkspaceMembers);
}

public class WorkspaceRepository(SlackDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<SlackDbContext, Workspace>(dbContext), IWorkspaceRepository
{
    public async Task<Workspace?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Workspace
            .Where(query => query.Id == id)
            .AddDependentObjects()
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Workspace?>
        GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken) =>
        await DbContext.Workspace
            .Where(query => query.Organization.Id == organizationId)
            .AddDependentObjects()
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public Workspace Add(Workspace workspace)
    {
        var now = timeProvider.GetUtcNow();
        workspace.CreatedAt = now;
        return DbContext.Workspace.Add(workspace).Entity;
    }

    public Workspace Update(Workspace workspace)
    {
        var now = timeProvider.GetUtcNow();
        workspace.ModifiedAt = now;
        return DbContext.Workspace.Update(workspace).Entity;
    }

    public Workspace Remove(Workspace workspace)
    {
        var now = timeProvider.GetUtcNow();
        workspace.DeletedAt = now;
        return DbContext.Workspace.Update(workspace).Entity;
    }
}
