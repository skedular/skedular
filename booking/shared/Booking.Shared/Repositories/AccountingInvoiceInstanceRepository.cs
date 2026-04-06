using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IAccountingInvoiceInstanceRepository : IRepository<AccountingInvoiceInstance>
{
    AccountingInvoiceInstance Add(AccountingInvoiceInstance accountingInvoiceInstance);
    AccountingInvoiceInstance Update(AccountingInvoiceInstance accountingInvoiceInstance);

    Task<ICollection<AccountingInvoiceInstance>> GetByProviderAndExternalInvoiceIdsAsync(
        string provider,
        ICollection<string> externalInvoiceIds,
        CancellationToken cancellationToken);

    Task<AccountingInvoiceInstance?> GetByProviderAndExternalInvoiceIdAsync(
        string provider,
        string externalInvoiceId,
        CancellationToken cancellationToken);

    Task<AccountingInvoiceInstance?> GetLatestByAccountingInvoiceExportLinkIdAsync(
        string accountingInvoiceExportLinkId,
        CancellationToken cancellationToken);

    Task<ICollection<AccountingInvoiceInstance>> GetByAccountingInvoiceExportLinkIdAsync(
        string accountingInvoiceExportLinkId,
        CancellationToken cancellationToken);
}

public class AccountingInvoiceInstanceRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, AccountingInvoiceInstance>(dbContext, timeProvider), IAccountingInvoiceInstanceRepository
{
    public AccountingInvoiceInstance Add(AccountingInvoiceInstance accountingInvoiceInstance)
    {
        var now = TimeProvider.GetUtcNow();
        accountingInvoiceInstance.CreatedAt = now;
        return DbContext.AccountingInvoiceInstance.Add(accountingInvoiceInstance).Entity;
    }

    public AccountingInvoiceInstance Update(AccountingInvoiceInstance accountingInvoiceInstance)
    {
        var now = TimeProvider.GetUtcNow();
        accountingInvoiceInstance.ModifiedAt = now;
        return DbContext.AccountingInvoiceInstance.Update(accountingInvoiceInstance).Entity;
    }

    public async Task<ICollection<AccountingInvoiceInstance>> GetByProviderAndExternalInvoiceIdsAsync(
        string provider,
        ICollection<string> externalInvoiceIds,
        CancellationToken cancellationToken)
    {
        if (externalInvoiceIds.Count == 0)
        {
            return [];
        }

        return await DbContext.AccountingInvoiceInstance
            .Include(query => query.AccountingInvoiceExportLink)
            .Where(query =>
                query.Provider == provider &&
                externalInvoiceIds.Contains(query.ExternalInvoiceId))
            .ToListAsync(cancellationToken);
    }

    public async Task<AccountingInvoiceInstance?> GetByProviderAndExternalInvoiceIdAsync(
        string provider,
        string externalInvoiceId,
        CancellationToken cancellationToken) =>
        await DbContext.AccountingInvoiceInstance
            .Include(query => query.AccountingInvoiceExportLink)
            .FirstOrDefaultAsync(
                query => query.Provider == provider && query.ExternalInvoiceId == externalInvoiceId,
                cancellationToken);

    public async Task<AccountingInvoiceInstance?> GetLatestByAccountingInvoiceExportLinkIdAsync(
        string accountingInvoiceExportLinkId,
        CancellationToken cancellationToken) =>
        await DbContext.AccountingInvoiceInstance
            .Where(query => query.AccountingInvoiceExportLinkId == accountingInvoiceExportLinkId)
            .OrderByDescending(query => query.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<ICollection<AccountingInvoiceInstance>> GetByAccountingInvoiceExportLinkIdAsync(
        string accountingInvoiceExportLinkId,
        CancellationToken cancellationToken) =>
        await DbContext.AccountingInvoiceInstance
            .Where(query => query.AccountingInvoiceExportLinkId == accountingInvoiceExportLinkId)
            .OrderByDescending(query => query.CreatedAt)
            .ToListAsync(cancellationToken);
}
