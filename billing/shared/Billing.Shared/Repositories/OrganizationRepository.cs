using Billing.Shared.Database;
using Billing.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Billing.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, bool includeAllOfferings, CancellationToken cancellationToken);
    Task<Organization?> GetByIdAsync(string id, bool includeDeletedOrganizationMembers, CancellationToken cancellationToken);
    Task<ICollection<Organization>> GetAllAsync(CancellationToken cancellationToken);
    Organization Update(Organization organization);
    Organization Remove(Organization organization);
}

internal static class OrganizationExtensions
{
    internal static
        IIncludableQueryable<Organization, ICollection<Identity>> AddDependentObjects(
            this IQueryable<Organization> originalQuery,
            bool includeDeletedOrganizationMembers,
            bool includeAllOfferings) =>
        includeAllOfferings
            ? originalQuery.Include(query => query.OrganizationOfferings.OrderByDescending(organizationOffering => organizationOffering.End))
                .Include(query =>
                    query.OrganizationMembers.Where(organizationMember =>
                        includeDeletedOrganizationMembers || !organizationMember.DeletedAt.HasValue))
                .ThenInclude(query => query.Customer)
                .ThenInclude(query => query.Identities)
            : originalQuery.Include(query => query.OrganizationOfferings
                    .Where(organizationOffering => !organizationOffering.DeletedAt.HasValue)
                    .OrderByDescending(organizationOffering => organizationOffering.End)
                    .Take(1))
                .Include(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
                .ThenInclude(query => query.Customer)
                .ThenInclude(query => query.Identities);
}

public class OrganizationRepository(BillingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BillingDbContext, Organization>(dbContext, timeProvider), IOrganizationRepository
{
    public async Task<Organization> UpsertNakedAsync(string id, bool includeAllOfferings, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdAsync(id, true, includeAllOfferings, cancellationToken))!;
    }

    public async Task<Organization?> GetByIdAsync(string id, bool includeDeletedOrganizationMembers, CancellationToken cancellationToken) =>
        await GetByIdAsync(id, includeDeletedOrganizationMembers, false, cancellationToken);

    public async Task<ICollection<Organization>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects(false, false)
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

    private async Task<Organization?> GetByIdAsync(
        string id,
        bool includeDeletedOrganizationMembers,
        bool includeAllOfferings,
        CancellationToken cancellationToken) =>
        await DbContext.Organization
            .AddDependentObjects(includeDeletedOrganizationMembers, includeAllOfferings)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
}
