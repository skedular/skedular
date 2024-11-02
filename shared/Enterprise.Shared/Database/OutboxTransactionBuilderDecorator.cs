using Microsoft.EntityFrameworkCore.Storage;

namespace Enterprise.Shared.Database;

public class OutboxTransactionBuilderDecorator(IDbTransactionBuilder transactionBuilder) : IDbTransactionBuilder
{
    public IDbContextTransaction BeginTransaction(IUnitOfWork unit) =>
        new TransactionDecorator(transactionBuilder.BeginTransaction(unit));

    public async Task<IDbContextTransaction> BeginTransactionAsync(IUnitOfWork unit,
        CancellationToken cancellationToken) =>
        new TransactionDecorator(await transactionBuilder.BeginTransactionAsync(unit, cancellationToken));
}
