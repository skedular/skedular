using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Enterprise.Shared.Database;

public interface IDbTransactionBuilder
{
    IDbContextTransaction BeginTransaction(IUnitOfWork unit);
    Task<IDbContextTransaction> BeginTransactionAsync(IUnitOfWork unit, CancellationToken cancellationToken);
}

public class DbTransactionBuilder : IDbTransactionBuilder
{
    public IDbContextTransaction BeginTransaction(IUnitOfWork unit) => ((DbContext)unit).Database.BeginTransaction(IsolationLevel.ReadCommitted);

    public async Task<IDbContextTransaction> BeginTransactionAsync(IUnitOfWork unit, CancellationToken cancellationToken) =>
        await ((DbContext)unit).Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
}
