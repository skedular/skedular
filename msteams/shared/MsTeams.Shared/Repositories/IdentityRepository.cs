using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using MsTeams.Shared.Database;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Repositories;

public interface IIdentityRepository : IRepository<Identity>
{
    Identity Add(Identity identity);
    void AddRange(ICollection<Identity> identities);
    Identity Update(Identity identity);
    void RemoveRange(IEnumerable<Identity> identities);
}

public class IdentityRepository(MsTeamsDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MsTeamsDbContext, Identity>(dbContext, timeProvider), IIdentityRepository
{
    public Identity Add(Identity identity)
    {
        var now = TimeProvider.GetUtcNow();
        identity.CreatedAt = now;
        return DbContext.Identity.Add(identity).Entity;
    }

    public void AddRange(ICollection<Identity> identities)
    {
        var now = TimeProvider.GetUtcNow();
        identities.ForEach(identity => identity.CreatedAt = now);
        DbContext.Identity.AddRange(identities);
    }

    public Identity Update(Identity identity)
    {
        var now = TimeProvider.GetUtcNow();
        identity.ModifiedAt = now;
        return DbContext.Identity.Update(identity).Entity;
    }

    public void RemoveRange(IEnumerable<Identity> identities) => DbContext.Identity.RemoveRange(identities);
}
