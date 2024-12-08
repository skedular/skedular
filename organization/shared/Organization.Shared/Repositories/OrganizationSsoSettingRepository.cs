using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IOrganizationSsoSettingRepository : IRepository<OrganizationSsoSetting>
{
    Task<OrganizationSsoSetting?> GetByOrganizationIdAsync(
        string organizationId,
        CancellationToken cancellationToken);
}
public class OrganizationSsoSettingRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationSsoSetting>(dbContext, timeProvider),
        IOrganizationSsoSettingRepository
{
    public async Task<OrganizationSsoSetting?> GetByOrganizationIdAsync(
        string organizationId,
        CancellationToken cancellationToken) =>
        await DbContext.OrganizationSsoSetting
            .Include(x=>x.Organization)
            .FirstOrDefaultAsync(query => query.Organization.Id == organizationId, cancellationToken);
}
