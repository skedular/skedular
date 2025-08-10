using Enterprise.Shared.Database;
using Marketplace.Shared.Database;
using Marketplace.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Marketplace.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<Organization>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Organization Update(Organization organization);
    Organization Remove(Organization organization);
}

internal static class OrganizationExtensions
{
    internal static IIncludableQueryable<Organization, IEnumerable<Identity>> AddDependentObjects(
        this IQueryable<Organization> originalQuery) =>
        originalQuery
            .Include(query => query.OrganizationSsoSettings)
            .Include(query => query.Tags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities);
}

public class OrganizationRepository(MarketplaceDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MarketplaceDbContext, Organization>(dbContext, timeProvider), IOrganizationRepository
{
    public override async Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Organization
            .AddDependentObjects()
            .FirstOrDefaultAsync(
                query => query.Id == id || (query.UniqueAlphanumericName != null && query.UniqueAlphanumericName == id),
                cancellationToken);

    public async Task<ICollection<Organization>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => ids.Contains(query.Id) || (query.UniqueAlphanumericName != null && ids.Contains(query.UniqueAlphanumericName)))
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

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
