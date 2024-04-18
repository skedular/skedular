using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;

namespace Booking.Shared.Repositories;

public interface IIdentityRepository : IRepository<Identity>
{
    Identity Add(Identity identity);
    void AddRange(ICollection<Identity> identities);
    Identity Update(Identity identity);
    void RemoveRange(IEnumerable<Identity> identities);
}

public class IdentityRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Identity>(dbContext), IIdentityRepository
{
    public Identity Add(Identity identity)
    {
        var now = timeProvider.GetUtcNow();
        identity.CreatedAt = now;
        return DbContext.Identity.Add(identity).Entity;
    }

    public void AddRange(ICollection<Identity> identities)
    {
        var now = timeProvider.GetUtcNow();
        identities.ForEach(identity => identity.CreatedAt = now);
        DbContext.Identity.AddRange(identities);
    }

    public Identity Update(Identity identity)
    {
        var now = timeProvider.GetUtcNow();
        identity.ModifiedAt = now;
        return DbContext.Identity.Update(identity).Entity;
    }

    public void RemoveRange(IEnumerable<Identity> identities) => DbContext.Identity.RemoveRange(identities);
}
