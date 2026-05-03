using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Location.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);

    Task<Organization?> GetByIdOrCustomDomainAsync(
        string? id,
        string? customDomain,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken);

    Task<Organization?> GetByIdOrCustomDomainUntrackedAsync(
        string? id,
        string? customDomain,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken);

    Organization Update(Organization location);
    Organization Remove(Organization location);
}

public static class OrganizationExtensions
{
    extension(IQueryable<Organization> originalQuery)
    {
        public IIncludableQueryable<Organization, IEnumerable<OrganizationTag>> AddDependentObjects(bool isTracked,
            bool includeDeletedOrganizationMembers,
            bool includeDeletedOrganizationTags) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.OrganizationSsoSettings)
            .Include(query => query.OrganizationMembers.Where(organizationMember =>
                includeDeletedOrganizationMembers || !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.Tags.Where(tag => includeDeletedOrganizationTags || !tag.DeletedAt.HasValue));
    }
}

public class OrganizationRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Organization>(dbContext, timeProvider), IOrganizationRepository
{
    public override async Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdOrCustomDomainAsync(id, null, true, true, cancellationToken))!;
    }

    public async Task<Organization?> GetByIdOrCustomDomainAsync(
        string? id,
        string? customDomain,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            return await DbContext.Organization
                .AddDependentObjects(true, includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(customDomain))
        {
            return await DbContext.Organization
                .AddDependentObjects(true, includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
                .FirstOrDefaultAsync(
                    query => query.CustomDomain != null && query.CustomDomain == customDomain,
                    cancellationToken);
        }

        throw new InvalidOperationException("Either id or customDomain must be provided.");
    }

    public async Task<Organization?> GetByIdOrCustomDomainUntrackedAsync(
        string? id,
        string? customDomain,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            return await DbContext.Organization
                .AddDependentObjects(false, includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(customDomain))
        {
            return await DbContext.Organization
                .AddDependentObjects(false, includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
                .FirstOrDefaultAsync(
                    query => query.CustomDomain != null && query.CustomDomain == customDomain,
                    cancellationToken);
        }

        throw new InvalidOperationException("Either id or customDomain must be provided.");
    }

    public Organization Remove(Organization location)
    {
        var now = TimeProvider.GetUtcNow();
        location.DeletedAt = now;
        return DbContext.Organization.Update(location).Entity;
    }

    public Organization Update(Organization location)
    {
        var now = TimeProvider.GetUtcNow();
        location.ModifiedAt = now;
        return DbContext.Organization.Update(location).Entity;
    }
}
