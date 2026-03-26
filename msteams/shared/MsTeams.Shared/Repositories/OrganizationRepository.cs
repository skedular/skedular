using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MsTeams.Shared.Database;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);

    Task<Organization?> GetByIdOrCustomDomainAsync(
        string? id,
        string? customDomain,
        CancellationToken cancellationToken);

    Organization Update(Organization organization);
    Organization Remove(Organization organization);
}

internal static class OrganizationExtensions
{
    extension(IQueryable<Organization> originalQuery)
    {
        internal IIncludableQueryable<Organization, Customer?> AddDependentObjects() =>
            originalQuery
                .Include(query => query.OrganizationSsoSettings)
                .Include(query => query.OrganizationMembers)
                .ThenInclude(query => query.Customer);
    }
}

public class OrganizationRepository(MsTeamsDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MsTeamsDbContext, Organization>(dbContext, timeProvider), IOrganizationRepository
{
    public override async Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdOrCustomDomainAsync(id, null, cancellationToken))!;
    }

    public async Task<Organization?> GetByIdOrCustomDomainAsync(
        string? id,
        string? customDomain,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            return await DbContext.Organization
                .AddDependentObjects()
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(customDomain))
        {
            return await DbContext.Organization
                .AddDependentObjects()
                .FirstOrDefaultAsync(query => query.CustomDomain != null && query.CustomDomain == customDomain, cancellationToken);
        }

        throw new InvalidOperationException("Either id or customDomain must be provided.");
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
