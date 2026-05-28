using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Team.Shared.Database;
using Team.Shared.Database.Entities;

namespace Team.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);

    Task<Organization?> GetByIdOrCustomDomainAsync(
        string? id,
        string? customDomain,
        bool includeDeletedOrganizationMembers,
        CancellationToken cancellationToken);

    Task<Organization?> GetByIdOrCustomDomainUntrackedAsync(
        string? id,
        string? customDomain,
        bool includeDeletedOrganizationMembers,
        CancellationToken cancellationToken);

    Organization Update(Organization team);
    Organization Remove(Organization team);
}

public static class OrganizationExtensions
{
    extension(IQueryable<Organization> originalQuery)
    {
        public IIncludableQueryable<Organization, IEnumerable<Database.Entities.Team>> AddDependentObjects(
            bool isTracked,
            bool includeDeletedOrganizationMembers) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.OrganizationSsoSettings)
            .Include(query => query.OrganizationMembers.Where(organizationMember =>
                includeDeletedOrganizationMembers || !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.Teams.Where(location => !location.DeletedAt.HasValue));
    }
}

public class OrganizationRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, Organization>(dbContext, timeProvider), IOrganizationRepository
{
    public override async Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdOrCustomDomainAsync(id, null, true, cancellationToken))!;
    }

    public async Task<Organization?> GetByIdOrCustomDomainAsync(
        string? id,
        string? customDomain,
        bool includeDeletedOrganizationMembers,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            return await DbContext.Organization
                .AddDependentObjects(true, includeDeletedOrganizationMembers)
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(customDomain))
        {
            return await DbContext.Organization
                .AddDependentObjects(true, includeDeletedOrganizationMembers)
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
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            return await DbContext.Organization
                .AddDependentObjects(false, includeDeletedOrganizationMembers)
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(customDomain))
        {
            return await DbContext.Organization
                .AddDependentObjects(false, includeDeletedOrganizationMembers)
                .FirstOrDefaultAsync(
                    query => query.CustomDomain != null && query.CustomDomain == customDomain,
                    cancellationToken);
        }

        throw new InvalidOperationException("Either id or customDomain must be provided.");
    }

    public Organization Remove(Organization team)
    {
        var now = TimeProvider.GetUtcNow();
        team.DeletedAt = now;
        return DbContext.Organization.Update(team).Entity;
    }

    public Organization Update(Organization team)
    {
        var now = TimeProvider.GetUtcNow();
        team.ModifiedAt = now;
        return DbContext.Organization.Update(team).Entity;
    }
}
