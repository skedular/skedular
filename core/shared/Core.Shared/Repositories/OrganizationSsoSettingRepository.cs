using Core.Shared.Database;
using Core.Shared.Database.Entities;
using Enterprise.Shared.Database;

namespace Core.Shared.Repositories;

public interface IOrganizationSsoSettingRepository : IRepository<OrganizationSsoSetting>
{
    void Add(OrganizationSsoSetting organizationSsoSetting);
    void Update(OrganizationSsoSetting organizationSsoSetting);
    void Remove(OrganizationSsoSetting organizationSsoSetting);
}

public class OrganizationSsoSettingRepository(CoreDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CoreDbContext, OrganizationSsoSetting>(dbContext, timeProvider), IOrganizationSsoSettingRepository
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

    public void Remove(OrganizationSsoSetting organizationSsoSetting) => DbContext.OrganizationSsoSetting.Remove(organizationSsoSetting);
}
