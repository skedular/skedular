using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Database;

public interface IDbTransactionBuilder
{
    Task<IDbContextTransaction> BeginTransactionAsync(IUnitOfWork unit, CancellationToken cancellationToken);

    Task<IDbContextTransaction> BeginTransactionAsync(
        IUnitOfWork unit,
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken);
}

public class DbTransactionBuilder(ILogger<DbTransactionBuilder> logger) : IDbTransactionBuilder
{
    public async Task<IDbContextTransaction> BeginTransactionAsync(IUnitOfWork unit, CancellationToken cancellationToken)
        => await BeginTransactionAsync(unit, IsolationLevel.ReadCommitted, cancellationToken);

    public async Task<IDbContextTransaction> BeginTransactionAsync(
        IUnitOfWork unit,
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Beginning database transaction. IsolationLevel={IsolationLevel}, UnitType={UnitType}",
            isolationLevel,
            unit.GetType().Name);
        return await ((DbContext)unit).Database.BeginTransactionAsync(isolationLevel, cancellationToken);
    }
}
