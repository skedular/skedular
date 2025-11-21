using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Sanitization;
using Marketplace.Shared.Database;
using Marketplace.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Marketplace.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);

    Task<Organization?> GetByIdOrUniqueAlphanumericNameAsync(
        string? id,
        string? uniqueAlphanumericName,
        CancellationToken cancellationToken);

    Task<ICollection<Organization>> GetByIdsOrUniqueAlphanumericNamesAsync(
        ICollection<string>? ids,
        ICollection<string>? uniqueAlphanumericNames,
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

        return (await GetByIdOrUniqueAlphanumericNameAsync(id, null, cancellationToken))!;
    }

    public async Task<Organization?> GetByIdOrUniqueAlphanumericNameAsync(
        string? id,
        string? uniqueAlphanumericName,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            return await DbContext.Organization
                .AddDependentObjects()
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(uniqueAlphanumericName))
        {
            return await DbContext.Organization
                .AddDependentObjects()
                .FirstOrDefaultAsync(
                    query => query.UniqueAlphanumericName != null && query.UniqueAlphanumericName == uniqueAlphanumericName,
                    cancellationToken);
        }

        throw new InvalidOperationException("Either id or uniqueAlphanumericName must be provided.");
    }

    public async Task<ICollection<Organization>> GetByIdsOrUniqueAlphanumericNamesAsync(
        ICollection<string>? ids,
        ICollection<string>? uniqueAlphanumericNames,
        CancellationToken cancellationToken)
    {
        if (ids is not null && ids.RemoveInvalidIds()!.Any() &&
            uniqueAlphanumericNames is not null && uniqueAlphanumericNames.RemoveInvalidIds()!.Any())
        {
            ids = ids.RemoveInvalidIds().ToSafeCollection();
            uniqueAlphanumericNames = uniqueAlphanumericNames.RemoveInvalidIds().ToSafeCollection();

            return await DbContext.Organization
                .Where(query => ids.Contains(query.Id) && query.UniqueAlphanumericName != null &&
                                uniqueAlphanumericNames.Contains(query.UniqueAlphanumericName))
                .AddDependentObjects()
                .ToListAsync(cancellationToken);
        }

        if (ids is not null && ids.RemoveInvalidIds()!.Any())
        {
            ids = ids.RemoveInvalidIds().ToSafeCollection();

            return await DbContext.Organization
                .Where(query => ids.Contains(query.Id))
                .AddDependentObjects()
                .ToListAsync(cancellationToken);
        }

        if (uniqueAlphanumericNames is not null && uniqueAlphanumericNames.RemoveInvalidIds()!.Any())
        {
            uniqueAlphanumericNames = uniqueAlphanumericNames.RemoveInvalidIds().ToSafeCollection();

            return await DbContext.Organization
                .Where(query => query.UniqueAlphanumericName != null && uniqueAlphanumericNames.Contains(query.UniqueAlphanumericName))
                .AddDependentObjects()
                .ToListAsync(cancellationToken);
        }

        throw new InvalidOperationException("Either ids or uniqueAlphanumericNames must be provided.");
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
