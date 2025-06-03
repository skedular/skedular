using Core.Shared.Database;
using Core.Shared.Database.Entities;
using Enterprise.Shared.Database;

namespace Core.Shared.Repositories;

public interface IIdentityRepository : IRepository<Identity>
{
    Identity Add(Identity identity);
    Identity Update(Identity identity);
    void RemoveRange(IEnumerable<Identity> identities);
}

public class IdentityRepository(CoreDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CoreDbContext, Identity>(dbContext, timeProvider), IIdentityRepository
{
    public Identity Add(Identity identity)
    {
        var now = TimeProvider.GetUtcNow();
        identity.CreatedAt = now;
        return DbContext.Identity.Add(identity).Entity;
    }

    public Identity Update(Identity identity)
    {
        var now = TimeProvider.GetUtcNow();
        identity.ModifiedAt = now;
        return DbContext.Identity.Update(identity).Entity;
    }

    public void RemoveRange(IEnumerable<Identity> identities) => DbContext.Identity.RemoveRange(identities);
}
