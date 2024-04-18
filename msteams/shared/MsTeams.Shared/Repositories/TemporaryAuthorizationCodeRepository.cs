using Enterprise.Shared.Database;
using MsTeams.Shared.Database;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Repositories;

public interface ITemporaryAuthorizationCodeRepository : IRepository<TemporaryAuthorizationCode>
{
    TemporaryAuthorizationCode Add(TemporaryAuthorizationCode temporaryAuthorizationCode);
    TemporaryAuthorizationCode Update(TemporaryAuthorizationCode temporaryAuthorizationCode);
}

public class TemporaryAuthorizationCodeRepository(MsTeamsDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MsTeamsDbContext, TemporaryAuthorizationCode>(dbContext),
        ITemporaryAuthorizationCodeRepository
{
    public TemporaryAuthorizationCode Add(TemporaryAuthorizationCode temporaryAuthorizationCode)
    {
        var now = timeProvider.GetUtcNow();
        temporaryAuthorizationCode.CreatedAt = now;
        return DbContext.TemporaryAuthorizationCode.Add(temporaryAuthorizationCode).Entity;
    }

    public TemporaryAuthorizationCode Update(TemporaryAuthorizationCode temporaryAuthorizationCode)
    {
        var now = timeProvider.GetUtcNow();
        temporaryAuthorizationCode.ModifiedAt = now;
        return DbContext.TemporaryAuthorizationCode.Update(temporaryAuthorizationCode).Entity;
    }
}
