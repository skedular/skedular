using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Database;

public interface IDbTransactionBuilder
{
    Task<IDbContextTransaction> BeginTransactionAsync(IUnitOfWork unit, CancellationToken cancellationToken);
}

public class DbTransactionBuilder(ILogger<DbTransactionBuilder> logger) : IDbTransactionBuilder
{
    public async Task<IDbContextTransaction> BeginTransactionAsync(IUnitOfWork unit, CancellationToken cancellationToken)
    {
        logger.LogDebug("Beginning database transaction. IsolationLevel={IsolationLevel}, UnitType={UnitType}",
            IsolationLevel.ReadCommitted,
            unit.GetType().Name);
        return await ((DbContext)unit).Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
    }
}
