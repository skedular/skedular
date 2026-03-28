using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Enterprise.Shared.Sanitization;
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

    Task<ICollection<Organization>> GetByIdsOrCustomDomainsAsync(
        ICollection<string>? ids,
        ICollection<string>? customDomains,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken);

    Organization Update(Organization organization);
    Organization Remove(Organization organization);
}

internal static class OrganizationExtensions
{
    extension(IQueryable<Organization> originalQuery)
    {
        internal IIncludableQueryable<Organization, IEnumerable<Customer>> AddDependentObjects(
            bool isTracked,
            bool includeDeletedOrganizationMembers,
            bool includeDeletedOrganizationTags) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.OrganizationSsoSettings)
            .Include(query =>
                query.OrganizationMembers.Where(organizationMember =>
                    includeDeletedOrganizationMembers || !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.Tags.Where(tag => includeDeletedOrganizationTags || !tag.DeletedAt.HasValue))
            .Include(query => query.Locations)
            .Include(query => query.Teams)
            .Include(query => query.DefaultedByCustomers);
    }
}

public class OrganizationRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Organization>(dbContext, timeProvider), IOrganizationRepository
{
    public override async Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdOrCustomDomainAsync(id, null, true, true, cancellationToken))!;
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
            .AddDependentObjects(true, includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
            .ToListAsync(cancellationToken);

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
                .FirstOrDefaultAsync(query => query.CustomDomain != null && query.CustomDomain == customDomain, cancellationToken);
        }

        throw new InvalidOperationException("Either id or customDomain must be provided.");
    }

    public async Task<ICollection<Organization>> GetByIdsOrCustomDomainsAsync(
        ICollection<string>? ids,
        ICollection<string>? customDomains,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken)
    {
        if (ids is not null && ids.RemoveInvalidIds().Any() && customDomains is not null && customDomains.RemoveInvalidIds().Any())
        {
            ids = ids.RemoveInvalidIds().ToSafeCollection();
            customDomains = customDomains.RemoveInvalidIds().ToSafeCollection();

            return await DbContext.Organization
                .Where(query => ids.Contains(query.Id) && query.CustomDomain != null && customDomains.Contains(query.CustomDomain))
                .AddDependentObjects(true, includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
                .ToListAsync(cancellationToken);
        }

        if (ids is not null && ids.RemoveInvalidIds().Any())
        {
            ids = ids.RemoveInvalidIds().ToSafeCollection();

            return await DbContext.Organization
                .Where(query => ids.Contains(query.Id))
                .AddDependentObjects(true, includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
                .ToListAsync(cancellationToken);
        }

        if (customDomains is not null && customDomains.RemoveInvalidIds().Any())
        {
            customDomains = customDomains.RemoveInvalidIds().ToSafeCollection();

            return await DbContext.Organization
                .Where(query => query.CustomDomain != null && customDomains.Contains(query.CustomDomain))
                .AddDependentObjects(true, includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
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
