using Billing.Shared.Database;
using Billing.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Billing.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<Organization?> GetByIdAsync(
        string id,
        bool includeAllOfferings,
        CancellationToken cancellationToken);

    Task<ICollection<Organization>> GetAllAsync(CancellationToken cancellationToken);
    Organization Add(Organization organization);
    Organization Update(Organization organization);
    Organization Remove(Organization organization);
}

internal static class OrganizationExtensions
{
    internal static
        IIncludableQueryable<Organization, ICollection<Identity>> AddDependentObjects(
            this IQueryable<Organization> originalQuery,
            bool includeAllOfferings) =>
        includeAllOfferings
            ? originalQuery.Include(query => query.OrganizationOfferings
                    .OrderByDescending(organizationOffering => organizationOffering.End))
                .Include(query =>
                    query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
                .ThenInclude(query => query.Customer)
                .ThenInclude(query => query.Identities)
            : originalQuery.Include(query => query.OrganizationOfferings
                    .Where(organizationOffering => !organizationOffering.DeletedAt.HasValue)
                    .OrderByDescending(organizationOffering => organizationOffering.End)
                    .Take(1))
                .Include(query =>
                    query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
                .ThenInclude(query => query.Customer)
                .ThenInclude(query => query.Identities);
}

public class OrganizationRepository(BillingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BillingDbContext, Organization>(dbContext), IOrganizationRepository
{
    public async Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var now = timeProvider.GetUtcNow();
        return DbContext.Organization.Add(new Organization { Id = id, CreatedAt = now }).Entity;
    }

    public async Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await GetByIdAsync(id, false, cancellationToken);

    public async Task<Organization?> GetByIdAsync(
        string id,
        bool includeAllOfferings,
        CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => query.Id == id)
            .AddDependentObjects(includeAllOfferings)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<ICollection<Organization>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects(false)
            .ToListAsync(cancellationToken);

    public Organization Add(Organization organization)
    {
        var now = timeProvider.GetUtcNow();
        organization.CreatedAt = now;
        return DbContext.Organization.Add(organization).Entity;
    }

    public Organization Update(Organization organization)
    {
        var now = timeProvider.GetUtcNow();
        organization.ModifiedAt = now;
        return DbContext.Organization.Update(organization).Entity;
    }

    public Organization Remove(Organization organization)
    {
        var now = timeProvider.GetUtcNow();
        organization.DeletedAt = now;
        return DbContext.Organization.Update(organization).Entity;
    }
}
