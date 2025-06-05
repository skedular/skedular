using Enterprise.Shared.Database;
using Marketplace.Shared.Database;
using Marketplace.Shared.Database.Entities;

namespace Marketplace.Shared.Repositories;

public interface ICdnFileRepository : IRepository<CdnFile>
{
    CdnFile Add(CdnFile cdnFile);
}

public class CdnFileRepository(MarketplaceDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MarketplaceDbContext, CdnFile>(dbContext, timeProvider), ICdnFileRepository
{
    public CdnFile Add(CdnFile cdnFile)
    {
        var now = TimeProvider.GetUtcNow();
        cdnFile.CreatedAt = now;
        return DbContext.CdnFile.Add(cdnFile).Entity;
    }
}
