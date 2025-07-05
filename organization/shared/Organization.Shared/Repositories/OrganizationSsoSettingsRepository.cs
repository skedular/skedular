using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationSsoSettingsRepository : IRepository<OrganizationSsoSettings>
{
    OrganizationSsoSettings Add(OrganizationSsoSettings organizationSsoSettings);
    OrganizationSsoSettings Update(OrganizationSsoSettings organizationSsoSettings);
    OrganizationSsoSettings Remove(OrganizationSsoSettings organizationSsoSettings);
    Task<OrganizationSsoSettings?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<OrganizationSsoSettings?> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
}

public class OrganizationSsoSettingsRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationSsoSettings>(dbContext, timeProvider), IOrganizationSsoSettingsRepository
{
    public OrganizationSsoSettings Add(OrganizationSsoSettings organizationSsoSettings)
    {
        var now = TimeProvider.GetUtcNow();
        organizationSsoSettings.CreatedAt = now;
        return DbContext.OrganizationSsoSettings.Add(organizationSsoSettings).Entity;
    }

    public OrganizationSsoSettings Update(OrganizationSsoSettings organizationSsoSettings)
    {
        var now = TimeProvider.GetUtcNow();
        organizationSsoSettings.ModifiedAt = now;
        return DbContext.OrganizationSsoSettings.Update(organizationSsoSettings).Entity;
    }

    public OrganizationSsoSettings Remove(OrganizationSsoSettings organizationSsoSettings) =>
        DbContext.OrganizationSsoSettings.Remove(organizationSsoSettings).Entity;

    public async Task<OrganizationSsoSettings?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationSsoSettings
            .Include(query => query.Organization)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<OrganizationSsoSettings?> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken) =>
        await DbContext.OrganizationSsoSettings
            .Include(query => query.Organization)
            .FirstOrDefaultAsync(query => query.Organization.Id == organizationId, cancellationToken);
}
