using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Enterprise.Shared.Sanitization;
using Marketplace.Shared.Database;
using Marketplace.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Marketplace.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);

    Task<Organization?> GetByIdOrCustomDomainAsync(
        string? id,
        string? customDomain,
        CancellationToken cancellationToken);

    Task<ICollection<Organization>> GetByIdsOrCustomDomainsAsync(
        ICollection<string>? ids,
        ICollection<string>? customDomains,
        CancellationToken cancellationToken);

    Organization Update(Organization organization);
    Organization Remove(Organization organization);
}

internal static class OrganizationExtensions
{
    extension(IQueryable<Organization> originalQuery)
    {
        internal IIncludableQueryable<Organization, IEnumerable<Identity>> AddDependentObjects() =>
            originalQuery
                .Include(query => query.OrganizationSsoSettings)
                .Include(query => query.Tags.Where(tag => !tag.DeletedAt.HasValue))
                .Include(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
                .ThenInclude(query => query.Customer)
                .ThenInclude(query => query.Identities);
    }
}

public class OrganizationRepository(MarketplaceDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MarketplaceDbContext, Organization>(dbContext, timeProvider), IOrganizationRepository
{
    public override async Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdOrCustomDomainAsync(id, null, cancellationToken))!;
    }

    public async Task<Organization?> GetByIdOrCustomDomainAsync(
        string? id,
        string? customDomain,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            return await DbContext.Organization
                .AddDependentObjects()
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(customDomain))
        {
            return await DbContext.Organization
                .AddDependentObjects()
                .FirstOrDefaultAsync(
                    query => query.CustomDomain != null && query.CustomDomain == customDomain,
                    cancellationToken);
        }

        throw new InvalidOperationException("Either id or customDomain must be provided.");
    }

    public async Task<ICollection<Organization>> GetByIdsOrCustomDomainsAsync(
        ICollection<string>? ids,
        ICollection<string>? customDomains,
        CancellationToken cancellationToken)
    {
        if (ids is not null && ids.RemoveInvalidIds().Any() && customDomains is not null && customDomains.RemoveInvalidIds().Any())
        {
            ids = ids.RemoveInvalidIds().ToSafeCollection();
            customDomains = customDomains.RemoveInvalidIds().ToSafeCollection();

            return await DbContext.Organization
                .Where(query => ids.Contains(query.Id) && query.CustomDomain != null &&
                                customDomains.Contains(query.CustomDomain))
                .AddDependentObjects()
                .ToListAsync(cancellationToken);
        }

        if (ids is not null && ids.RemoveInvalidIds().Any())
        {
            ids = ids.RemoveInvalidIds().ToSafeCollection();

            return await DbContext.Organization
                .Where(query => ids.Contains(query.Id))
                .AddDependentObjects()
                .ToListAsync(cancellationToken);
        }

        if (customDomains is not null && customDomains.RemoveInvalidIds().Any())
        {
            customDomains = customDomains.RemoveInvalidIds().ToSafeCollection();

            return await DbContext.Organization
                .Where(query => query.CustomDomain != null && customDomains.Contains(query.CustomDomain))
                .AddDependentObjects()
                .ToListAsync(cancellationToken);
        }

        throw new InvalidOperationException("Either ids or customDomains must be provided.");
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
