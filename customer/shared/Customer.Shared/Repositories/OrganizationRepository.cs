using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Customer.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);

    Task<Organization?> GetByIdOrUniqueAlphanumericNameAsync(
        string? id,
        string? uniqueAlphanumericName,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken);

    Task<ICollection<Organization>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken);
    Organization Update(Organization organization);
    Organization Remove(Organization organization);
}

public class OrganizationRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, Organization>(dbContext, timeProvider), IOrganizationRepository
{
    public override async Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdOrUniqueAlphanumericNameAsync(id, null, true, true, cancellationToken))!;
    }

    public async Task<Organization?> GetByIdOrUniqueAlphanumericNameAsync(
        string? id,
        string? uniqueAlphanumericName,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            return await DbContext.Organization
                .Include(query => query.OrganizationSsoSettings)
                .Include(query =>
                    query.OrganizationMembers.Where(organizationMember =>
                        includeDeletedOrganizationMembers || !organizationMember.DeletedAt.HasValue))
                .ThenInclude(query => query.Customer)
                .ThenInclude(query => query.Identities)
                .Include(query => query.Tags.Where(tag => includeDeletedOrganizationTags || !tag.DeletedAt.HasValue))
                .Include(query => query.Locations)
                .Include(query => query.Teams)
                .Include(query => query.DefaultedByCustomers)
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(uniqueAlphanumericName))
        {
            return await DbContext.Organization
                .Include(query => query.OrganizationSsoSettings)
                .Include(query =>
                    query.OrganizationMembers.Where(organizationMember =>
                        includeDeletedOrganizationMembers || !organizationMember.DeletedAt.HasValue))
                .ThenInclude(query => query.Customer)
                .ThenInclude(query => query.Identities)
                .Include(query => query.Tags.Where(tag => includeDeletedOrganizationTags || !tag.DeletedAt.HasValue))
                .Include(query => query.Locations)
                .Include(query => query.Teams)
                .Include(query => query.DefaultedByCustomers)
                .FirstOrDefaultAsync(
                    query => query.UniqueAlphanumericName != null && query.UniqueAlphanumericName == uniqueAlphanumericName,
                    cancellationToken);
        }

        throw new InvalidOperationException("Either id or uniqueAlphanumericName must be provided.");
    }

    public async Task<ICollection<Organization>> GetByCustomerIdAsync(
        string customerId,
        CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => query.OrganizationMembers.Any(item => item.CustomerId == customerId))
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
