using Microsoft.EntityFrameworkCore.Storage;

namespace Enterprise.Shared.Database;

public class OutboxTransactionBuilderDecorator(IDbTransactionBuilder transactionBuilder) : IDbTransactionBuilder
{
    public IDbContextTransaction BeginTransaction(IUnitOfWork unit)
    {
        var dbContextTransaction = transactionBuilder.BeginTransaction(unit);

        return new TransactionDecorator(dbContextTransaction);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(IUnitOfWork unit,
        CancellationToken cancellationToken)
    {
        var dbContextTransaction = await transactionBuilder.BeginTransactionAsync(unit, cancellationToken);

        return new TransactionDecorator(dbContextTransaction);
    }
}
