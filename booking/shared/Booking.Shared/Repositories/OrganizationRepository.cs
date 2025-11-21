using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
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

    Task<Organization?> GetByIdOrUniqueAlphanumericNameAsync(
        string? id,
        string? uniqueAlphanumericName,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken);

    Task<ICollection<Organization>> GetByIdsOrUniqueAlphanumericNamesAsync(
        ICollection<string>? ids,
        ICollection<string>? uniqueAlphanumericNames,
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
            bool includeDeletedOrganizationMembers,
            bool includeDeletedOrganizationTags) =>
            originalQuery
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

        return (await GetByIdOrUniqueAlphanumericNameAsync(id, null, true, true, cancellationToken))!;
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
                .AddDependentObjects(includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(uniqueAlphanumericName))
        {
            return await DbContext.Organization
                .AddDependentObjects(includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
                .FirstOrDefaultAsync(
                    query => query.UniqueAlphanumericName != null && query.UniqueAlphanumericName == uniqueAlphanumericName,
                    cancellationToken);
        }

        throw new InvalidOperationException("Either id or uniqueAlphanumericName must be provided.");
    }

    public async Task<ICollection<Organization>> GetByIdsOrUniqueAlphanumericNamesAsync(
        ICollection<string>? ids,
        ICollection<string>? uniqueAlphanumericNames,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
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
                .AddDependentObjects(includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
                .ToListAsync(cancellationToken);
        }

        if (ids is not null && ids.RemoveInvalidIds()!.Any())
        {
            ids = ids.RemoveInvalidIds().ToSafeCollection();

            return await DbContext.Organization
                .Where(query => ids.Contains(query.Id))
                .AddDependentObjects(includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
                .ToListAsync(cancellationToken);
        }

        if (uniqueAlphanumericNames is not null && uniqueAlphanumericNames.RemoveInvalidIds()!.Any())
        {
            uniqueAlphanumericNames = uniqueAlphanumericNames.RemoveInvalidIds().ToSafeCollection();

            return await DbContext.Organization
                .Where(query => query.UniqueAlphanumericName != null && uniqueAlphanumericNames.Contains(query.UniqueAlphanumericName))
                .AddDependentObjects(includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
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

    public async Task<Organization?> GetByUniqueAlphanumericNameAsync(
        string uniqueAlphanumericName,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken) =>
        await DbContext.Organization
            .AddDependentObjects(includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
            .FirstOrDefaultAsync(
                query => query.UniqueAlphanumericName != null && query.UniqueAlphanumericName == uniqueAlphanumericName,
                cancellationToken);
}
