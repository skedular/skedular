using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Marketplace.Shared.Database;
using Marketplace.Shared.Database.Entities;

namespace Marketplace.Shared.Repositories;

public interface IOrganizationSsoSettingRepository : IRepository<OrganizationSsoSetting>
{
    void Add(OrganizationSsoSetting organizationSsoSetting);
    void Update(OrganizationSsoSetting organizationSsoSetting);
    void Remove(OrganizationSsoSetting organizationSsoSetting);
}

public class OrganizationSsoSettingRepository(MarketplaceDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MarketplaceDbContext, OrganizationSsoSetting>(dbContext, timeProvider), IOrganizationSsoSettingRepository
{
    public void Add(OrganizationSsoSetting organizationSsoSetting)
    {
        var now = TimeProvider.GetUtcNow();
        organizationSsoSetting.CreatedAt = now;
        DbContext.OrganizationSsoSetting.Add(organizationSsoSetting);
    }

    public void Update(OrganizationSsoSetting organizationSsoSetting)
    {
        var now = TimeProvider.GetUtcNow();
        organizationSsoSetting.ModifiedAt = now;
        DbContext.OrganizationSsoSetting.Update(organizationSsoSetting);
    }

    public void Remove(OrganizationSsoSetting organizationSsoSetting) =>
        DbContext.OrganizationSsoSetting.Remove(organizationSsoSetting);
}
