using Enterprise.Shared.Database;
using MsTeams.Shared.Database;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Repositories;

public interface ITenantRepository : IRepository<Tenant>
{
    Tenant Add(Tenant tenant);
    Tenant Update(Tenant tenant);
}

public class TenantRepository(MsTeamsDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MsTeamsDbContext, Tenant>(dbContext), ITenantRepository
{
    public Tenant Update(Tenant tenant)
    {
        var now = timeProvider.GetUtcNow();
        tenant.ModifiedAt = now;
        return DbContext.Tenant.Update(tenant).Entity;
    }

    public Tenant Add(Tenant tenant)
    {
        var now = timeProvider.GetUtcNow();
        tenant.CreatedAt = now;
        return DbContext.Tenant.Add(tenant).Entity;
    }
}
