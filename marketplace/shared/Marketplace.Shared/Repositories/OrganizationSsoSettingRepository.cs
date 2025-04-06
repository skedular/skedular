using Enterprise.Shared.Database;
using Marketplace.Shared.Database;
using Marketplace.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Shared.Repositories;

public interface IOrganizationSsoSettingRepository : IRepository<OrganizationSsoSetting>
{
    OrganizationSsoSetting Add(OrganizationSsoSetting organizationSsoSetting);
    OrganizationSsoSetting Update(OrganizationSsoSetting organizationSsoSetting);
    OrganizationSsoSetting Remove(OrganizationSsoSetting organizationSsoSetting);
    Task<OrganizationSsoSetting?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<OrganizationSsoSetting?> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
}

public class OrganizationSsoSettingRepository(MarketplaceDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MarketplaceDbContext, OrganizationSsoSetting>(dbContext, timeProvider), IOrganizationSsoSettingRepository
{
    public OrganizationSsoSetting Add(OrganizationSsoSetting organizationSsoSetting)
    {
        var now = TimeProvider.GetUtcNow();
        organizationSsoSetting.CreatedAt = now;
        return DbContext.OrganizationSsoSetting.Add(organizationSsoSetting).Entity;
    }

    public OrganizationSsoSetting Update(OrganizationSsoSetting organizationSsoSetting)
    {
        var now = TimeProvider.GetUtcNow();
        organizationSsoSetting.ModifiedAt = now;
        return DbContext.OrganizationSsoSetting.Update(organizationSsoSetting).Entity;
    }

    public OrganizationSsoSetting Remove(OrganizationSsoSetting organizationSsoSetting) =>
        DbContext.OrganizationSsoSetting.Remove(organizationSsoSetting).Entity;

    public async Task<OrganizationSsoSetting?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationSsoSetting
            .Include(query => query.Organization)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<OrganizationSsoSetting?> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken) =>
        await DbContext.OrganizationSsoSetting
            .Include(query => query.Organization)
            .FirstOrDefaultAsync(query => query.Organization.Id == organizationId, cancellationToken);
}
