using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Slack.Shared.Database;
using Slack.Shared.Database.Entities;

namespace Slack.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Organization?> GetByWorkspaceIdAsync(string workspaceId, CancellationToken cancellationToken);
    Organization Add(Organization organization);
    Organization Update(Organization organization);
    Organization Remove(Organization organization);
}

internal static class OrganizationExtensions
{
    internal static IIncludableQueryable<Organization, WorkspaceChannel?> AddDependentObjects(
        this IQueryable<Organization> originalQuery) =>
        originalQuery
            .Include(query => query.Workspaces)
            .Include(query => query.OrganizationMembers)
            .ThenInclude(query => query.Customer)
            .Include(query => query.DailyUpdateChannel);
}

public class OrganizationRepository(SlackDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<SlackDbContext, Organization>(dbContext, timeProvider), IOrganizationRepository
{
    public override async Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => query.Id == id)
            .AddDependentObjects()
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Organization?> GetByWorkspaceIdAsync(string workspaceId, CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => !query.DeletedAt.HasValue && query.Workspaces.Any(workspace => workspace.Id == workspaceId))
            .AddDependentObjects()
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public Organization Add(Organization organization)
    {
        var now = TimeProvider.GetUtcNow();
        organization.CreatedAt = now;
        return DbContext.Organization.Add(organization).Entity;
    }

    public Organization Update(Organization organization)
    {
        var now = TimeProvider.GetUtcNow();
        organization.ModifiedAt = now;
        return DbContext.Organization.Update(organization).Entity;
    }

    public Organization Remove(Organization organization)
    {
        var now = TimeProvider.GetUtcNow();
        organization.DeletedAt = now;
        return DbContext.Organization.Update(organization).Entity;
    }
}
