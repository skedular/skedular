using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Enterprise.Shared.Database;

public interface IDbTransactionBuilder
{
    Task<IDbContextTransaction> BeginTransactionAsync(IUnitOfWork unit, CancellationToken cancellationToken);
}

public class DbTransactionBuilder : IDbTransactionBuilder
{
    public async Task<IDbContextTransaction> BeginTransactionAsync(IUnitOfWork unit, CancellationToken cancellationToken) =>
        await ((DbContext)unit).Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
}
