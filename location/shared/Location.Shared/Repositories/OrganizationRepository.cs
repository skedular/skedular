using Enterprise.Shared.Database;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Location.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);

    Task<Organization?> GetByIdOrUniqueAlphanumericNameAsync(
        string? id,
        string? uniqueAlphanumericName,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken);

    Task<Organization?> GetByIdOrUniqueAlphanumericNameUntrackedAsync(
        string? id,
        string? uniqueAlphanumericName,
        bool includeDeletedOrganizationMembers,
        bool includeDeletedOrganizationTags,
        CancellationToken cancellationToken);

    Organization Update(Organization location);
    Organization Remove(Organization location);
}

internal static class OrganizationExtensions
{
    internal static IIncludableQueryable<Organization, IEnumerable<OrganizationTag>> AddDependentObjects(
        this IQueryable<Organization> originalQuery,
        bool isTracked,
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

public class OrganizationRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Organization>(dbContext, timeProvider), IOrganizationRepository
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
                .AddDependentObjects(true, includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(uniqueAlphanumericName))
        {
            return await DbContext.Organization
                .AddDependentObjects(true, includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
                .FirstOrDefaultAsync(
                    query => query.UniqueAlphanumericName != null && query.UniqueAlphanumericName == uniqueAlphanumericName,
                    cancellationToken);
        }

        throw new InvalidOperationException("Either id or uniqueAlphanumericName must be provided.");
    }

    public async Task<Organization?> GetByIdOrUniqueAlphanumericNameUntrackedAsync(
        string? id,
        string? uniqueAlphanumericName,
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

        if (!string.IsNullOrWhiteSpace(uniqueAlphanumericName))
        {
            return await DbContext.Organization
                .AddDependentObjects(false, includeDeletedOrganizationMembers, includeDeletedOrganizationTags)
                .FirstOrDefaultAsync(
                    query => query.UniqueAlphanumericName != null && query.UniqueAlphanumericName == uniqueAlphanumericName,
                    cancellationToken);
        }

        throw new InvalidOperationException("Either id or uniqueAlphanumericName must be provided.");
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
