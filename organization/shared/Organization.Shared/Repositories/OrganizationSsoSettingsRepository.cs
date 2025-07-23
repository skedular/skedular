using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationSsoSettingsRepository : IRepository<OrganizationSsoSettings>
{
    void Add(OrganizationSsoSettings organizationSsoSettings);
    void Update(OrganizationSsoSettings organizationSsoSettings);
    Task<OrganizationSsoSettings?> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
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

    public async Task<OrganizationSsoSettings?> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken) =>
        await DbContext.OrganizationSsoSettings
            .Include(query => query.Organization)
            .FirstOrDefaultAsync(query => query.Organization.Id == organizationId, cancellationToken);
}
