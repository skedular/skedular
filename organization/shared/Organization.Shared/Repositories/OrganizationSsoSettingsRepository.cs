using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationSsoSettingsRepository : IRepository<OrganizationSsoSettings>
{
    void Add(OrganizationSsoSettings organizationSsoSettings);
    void Update(OrganizationSsoSettings organizationSsoSettings);

    Task<OrganizationSsoSettings?> GetByOrganizationCustomDomainAsync(
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken);
}

public class OrganizationSsoSettingsRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationSsoSettings>(dbContext, timeProvider), IOrganizationSsoSettingsRepository
{
    public void Add(OrganizationSsoSettings organizationSsoSettings)
    {
        var now = TimeProvider.GetUtcNow();
        organizationSsoSettings.CreatedAt = now;
        DbContext.OrganizationSsoSettings.Update(organizationSsoSettings);
    }

    public void Update(OrganizationSsoSettings organizationSsoSettings)
    {
        var now = TimeProvider.GetUtcNow();
        organizationSsoSettings.ModifiedAt = now;
        DbContext.OrganizationSsoSettings.Update(organizationSsoSettings);
    }

    public async Task<OrganizationSsoSettings?> GetByOrganizationCustomDomainAsync(
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(organizationId))
        {
            return await DbContext.OrganizationSsoSettings
                .Include(query => query.Organization)
                .FirstOrDefaultAsync(query => query.Organization.Id == organizationId, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(organizationCustomDomain))
        {
            return await DbContext.OrganizationSsoSettings
                .Include(query => query.Organization)
                .FirstOrDefaultAsync(
                    query => query.Organization.CustomDomain != null &&
                             query.Organization.CustomDomain == organizationCustomDomain,
                    cancellationToken);
        }

        throw new InvalidOperationException("Either id or customDomain must be provided.");
    }
}
