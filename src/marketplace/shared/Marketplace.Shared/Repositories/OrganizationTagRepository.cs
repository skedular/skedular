using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Marketplace.Shared.Database;
using Marketplace.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Shared.Repositories;

public interface IOrganizationTagRepository : IRepository<OrganizationTag>
{
    Task<OrganizationTag> UpsertNakedAsync(string id, Organization organization, CancellationToken cancellationToken);
    Task<OrganizationTag?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<IReadOnlyList<OrganizationTag>> GetActiveByIdsForOrganizationAsync(
        IReadOnlyList<string> ids,
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken);

    OrganizationTag Add(OrganizationTag organizationTag);
    OrganizationTag Update(OrganizationTag organizationTag);
    void RemoveRange(IEnumerable<OrganizationTag> organizationTags);
}

public class OrganizationTagRepository(MarketplaceDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MarketplaceDbContext, OrganizationTag>(dbContext, timeProvider), IOrganizationTagRepository
{
    public async Task<OrganizationTag> UpsertNakedAsync(string id, Organization organization, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<OrganizationTag?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationTag
            .AsSingleQuery()
            .Include(query => query.Organization)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    /// <summary>
    ///     Returns the active organization tags that match the supplied identifiers and belong to the specified organization scope.
    /// </summary>
    /// <param name="ids">The candidate organization tag identifiers requested by the caller.</param>
    /// <param name="organizationId">The owning organization identifier when the caller already knows it.</param>
    /// <param name="organizationCustomDomain">The owning organization custom domain when the caller is scoped by domain rather than identifier.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The matching active organization tags for the requested organization scope.</returns>
    /// <remarks>
    ///     This repository-owned query replaces specification composition for marketplace product tag resolution and keeps organization ownership
    ///     enforcement inside the repository.
    /// </remarks>
    public async Task<IReadOnlyList<OrganizationTag>> GetActiveByIdsForOrganizationAsync(
        IReadOnlyList<string> ids,
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken)
    {
        var hasOrganizationId = !string.IsNullOrWhiteSpace(organizationId);
        var hasOrganizationCustomDomain = !string.IsNullOrWhiteSpace(organizationCustomDomain);

        if (ids.Count == 0 || (!hasOrganizationId && !hasOrganizationCustomDomain))
        {
            return [];
        }

        var query = DbContext.OrganizationTag
            .Where(item =>
                !item.DeletedAt.HasValue &&
                ids.Contains(item.Id) &&
                !item.Organization.DeletedAt.HasValue);

        if (hasOrganizationId && hasOrganizationCustomDomain)
        {
            query = query.Where(item => item.Organization.Id == organizationId || item.Organization.CustomDomain == organizationCustomDomain);
        }
        else if (hasOrganizationId)
        {
            query = query.Where(item => item.Organization.Id == organizationId);
        }
        else if (hasOrganizationCustomDomain)
        {
            query = query.Where(item => item.Organization.CustomDomain == organizationCustomDomain);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public OrganizationTag Add(OrganizationTag organizationTag)
    {
        var now = TimeProvider.GetUtcNow();
        organizationTag.CreatedAt = now;
        return DbContext.OrganizationTag.Add(organizationTag).Entity;
    }

    public void RemoveRange(IEnumerable<OrganizationTag> organizationTags)
    {
        var now = TimeProvider.GetUtcNow();
        DbContext.OrganizationTag.UpdateRange(organizationTags.Select(item =>
        {
            item.DeletedAt = now;
            return item;
        }));
    }

    public OrganizationTag Update(OrganizationTag organizationTag)
    {
        var now = TimeProvider.GetUtcNow();
        organizationTag.ModifiedAt = now;
        return DbContext.OrganizationTag.Update(organizationTag).Entity;
    }
}
