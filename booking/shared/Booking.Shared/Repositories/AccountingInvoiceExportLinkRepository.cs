using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IAccountingInvoiceExportLinkRepository : IRepository<AccountingInvoiceExportLink>
{
    AccountingInvoiceExportLink Add(AccountingInvoiceExportLink accountingInvoiceLink);
    AccountingInvoiceExportLink Update(AccountingInvoiceExportLink accountingInvoiceLink);

    Task<AccountingInvoiceExportLink?> GetByProviderAndLocalEntityAsync(
        string provider,
        string localEntityType,
        string localEntityId,
        CancellationToken cancellationToken);

    Task<ICollection<AccountingInvoiceExportLink>> GetByProviderAndExternalInvoiceIdsAsync(
        string provider,
        ICollection<string> externalInvoiceIds,
        CancellationToken cancellationToken);
}

public class AccountingInvoiceExportLinkRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, AccountingInvoiceExportLink>(dbContext, timeProvider), IAccountingInvoiceExportLinkRepository
{
    public AccountingInvoiceExportLink Add(AccountingInvoiceExportLink accountingInvoiceLink)
    {
        var now = TimeProvider.GetUtcNow();
        accountingInvoiceLink.CreatedAt = now;
        return DbContext.AccountingInvoiceExportLink.Add(accountingInvoiceLink).Entity;
    }

    public AccountingInvoiceExportLink Update(AccountingInvoiceExportLink accountingInvoiceLink)
    {
        var now = TimeProvider.GetUtcNow();
        accountingInvoiceLink.ModifiedAt = now;
        return DbContext.AccountingInvoiceExportLink.Update(accountingInvoiceLink).Entity;
    }

    public async Task<AccountingInvoiceExportLink?> GetByProviderAndLocalEntityAsync(
        string provider,
        string localEntityType,
        string localEntityId,
        CancellationToken cancellationToken) =>
        await DbContext.AccountingInvoiceExportLink.FirstOrDefaultAsync(
            query => query.Provider == provider && query.LocalEntityType == localEntityType && query.LocalEntityId == localEntityId,
            cancellationToken);

    public async Task<ICollection<AccountingInvoiceExportLink>> GetByProviderAndExternalInvoiceIdsAsync(
        string provider,
        ICollection<string> externalInvoiceIds,
        CancellationToken cancellationToken)
    {
        if (externalInvoiceIds.Count == 0)
        {
            return [];
        }

        return await DbContext.AccountingInvoiceExportLink
            .Where(query =>
                query.Provider == provider &&
                query.ExternalInvoiceId != null &&
                externalInvoiceIds.Contains(query.ExternalInvoiceId))
            .ToListAsync(cancellationToken);
    }
}
