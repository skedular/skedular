using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

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

    Task<ICollection<Organization>> GetByIdsAsync(
        ICollection<string> ids,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken);

    Organization Update(Organization organization);
    Organization Remove(Organization organization);
}

internal static class OrganizationExtensions
{
    internal static IIncludableQueryable<Organization, IEnumerable<Customer>> AddDependentObjects(
        this IQueryable<Organization> originalQuery,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags) =>
        originalQuery
            .Include(query => query.OrganizationSsoSettings)
            .Include(query =>
                query.OrganizationMembers.Where(organizationMember => includeDeletedOrganizationMembers || !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.Tags.Where(tag => includeDeletedOrganizationTags || !tag.DeletedAt.HasValue))
            .Include(query => query.Locations)
            .Include(query => query.Teams)
            .Include(query => query.DefaultedByCustomers);
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
            .AddDependentObjects(includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
            .ToListAsync(cancellationToken);

    public async Task<Organization?> GetByIdAsync(
        string id,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken) =>
        await DbContext.Organization
            .AddDependentObjects(includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
            .FirstOrDefaultAsync(
                query => query.Id == id || (query.UniqueAlphanumericName != null && query.UniqueAlphanumericName == id),
                cancellationToken);

    public async Task<ICollection<Organization>> GetByIdsAsync(
        ICollection<string> ids,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => ids.Contains(query.Id) || (query.UniqueAlphanumericName != null && ids.Contains(query.UniqueAlphanumericName)))
            .AddDependentObjects(includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
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
