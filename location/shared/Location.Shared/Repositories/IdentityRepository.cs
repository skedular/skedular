using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Location.Shared.Database;
using Location.Shared.Database.Entities;

namespace Location.Shared.Repositories;

public interface IIdentityRepository : IRepository<Identity>
{
    Identity Add(Identity identity);
    void AddRange(IReadOnlyList<Identity> identities);
    Identity Update(Identity identity);
    void RemoveRange(IEnumerable<Identity> identities);
}

public class IdentityRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Identity>(dbContext, timeProvider), IIdentityRepository
{
    public Identity Add(Identity identity)
    {
        var now = TimeProvider.GetUtcNow();
        identity.CreatedAt = now;
        return DbContext.Identity.Add(identity).Entity;
    }

    public void AddRange(IReadOnlyList<Identity> identities)
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
