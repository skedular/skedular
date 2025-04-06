using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);

    Task<ICollection<Organization>> GetByCustomerIdAsync(
        string customerId,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken);

    Task<Organization?> GetByIdAsync(
        string id,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken);

    Organization Add(Organization organization);
    Organization Update(Organization organization);
    Organization Remove(Organization organization);
}

public class OrganizationRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Organization>(dbContext, timeProvider), IOrganizationRepository
{
    public override async Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdAsync(id, true, true, cancellationToken))!;
    }

    public async Task<ICollection<Organization>> GetByCustomerIdAsync(
        string customerId,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query =>
                query.OrganizationMembers
                    .Where(item => includeDeletedOrganizationMembers || (!item.DeletedAt.HasValue && item.CustomerId == customerId))
                    .Select(item => item.Customer.Id).Contains(customerId))
            .Include(query => query.OrganizationSsoSettings)
            .Include(query => query.OrganizationMembers.Where(
                organizationMember => includeDeletedOrganizationMembers || !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.Tags.Where(tag => includeDeletedOrganizationTags || !tag.DeletedAt.HasValue))
            .Include(query => query.Locations)
            .Include(query => query.Teams)
            .Include(query => query.DefaultedByCustomers)
            .ToListAsync(cancellationToken);

    public async Task<Organization?> GetByIdAsync(
        string id,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Include(query => query.OrganizationSsoSettings)
            .Include(query => query.OrganizationMembers.Where(
                organizationMember => includeDeletedOrganizationMembers || !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.Tags.Where(tag => includeDeletedOrganizationTags || !tag.DeletedAt.HasValue))
            .Include(query => query.Locations)
            .Include(query => query.Teams)
            .Include(query => query.DefaultedByCustomers)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

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
