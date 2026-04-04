using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IAccountingInvoiceLinkRepository : IRepository<AccountingInvoiceLink>
{
    AccountingInvoiceLink Add(AccountingInvoiceLink accountingInvoiceLink);
    AccountingInvoiceLink Update(AccountingInvoiceLink accountingInvoiceLink);

    Task<AccountingInvoiceLink?> GetByProviderAndLocalEntityAsync(
        string provider,
        string localEntityType,
        string localEntityId,
        CancellationToken cancellationToken);

    Task<AccountingInvoiceLink?> GetByProviderAndExternalInvoiceIdAsync(
        string provider,
        string externalInvoiceId,
        CancellationToken cancellationToken);

    Task<ICollection<AccountingInvoiceLink>> GetByProviderAndExternalInvoiceIdsAsync(
        string provider,
        ICollection<string> externalInvoiceIds,
        CancellationToken cancellationToken);

    Task<ICollection<AccountingInvoiceLink>> GetByOrganizationIdUntrackedAsync(
        string organizationId,
        CancellationToken cancellationToken);
}

public class AccountingInvoiceLinkRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, AccountingInvoiceLink>(dbContext, timeProvider), IAccountingInvoiceLinkRepository
{
    public AccountingInvoiceLink Add(AccountingInvoiceLink accountingInvoiceLink)
    {
        var now = TimeProvider.GetUtcNow();
        accountingInvoiceLink.CreatedAt = now;
        return DbContext.AccountingInvoiceLink.Add(accountingInvoiceLink).Entity;
    }

    public AccountingInvoiceLink Update(AccountingInvoiceLink accountingInvoiceLink)
    {
        var now = TimeProvider.GetUtcNow();
        accountingInvoiceLink.ModifiedAt = now;
        return DbContext.AccountingInvoiceLink.Update(accountingInvoiceLink).Entity;
    }

    public async Task<AccountingInvoiceLink?> GetByProviderAndLocalEntityAsync(
        string provider,
        string localEntityType,
        string localEntityId,
        CancellationToken cancellationToken) =>
        await DbContext.AccountingInvoiceLink.FirstOrDefaultAsync(
            query => query.Provider == provider && query.LocalEntityType == localEntityType && query.LocalEntityId == localEntityId,
            cancellationToken);

    public async Task<AccountingInvoiceLink?> GetByProviderAndExternalInvoiceIdAsync(
        string provider,
        string externalInvoiceId,
        CancellationToken cancellationToken) =>
        await DbContext.AccountingInvoiceLink.FirstOrDefaultAsync(
            query => query.Provider == provider && query.ExternalInvoiceId == externalInvoiceId,
            cancellationToken);

    public async Task<ICollection<AccountingInvoiceLink>> GetByProviderAndExternalInvoiceIdsAsync(
        string provider,
        ICollection<string> externalInvoiceIds,
        CancellationToken cancellationToken)
    {
        if (externalInvoiceIds.Count == 0)
        {
            return [];
        }

        return await DbContext.AccountingInvoiceLink
            .Where(query =>
                query.Provider == provider &&
                query.ExternalInvoiceId != null &&
                externalInvoiceIds.Contains(query.ExternalInvoiceId))
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<AccountingInvoiceLink>> GetByOrganizationIdUntrackedAsync(
        string organizationId,
        CancellationToken cancellationToken) =>
        await DbContext.AccountingInvoiceLink
            .AsNoTracking()
            .Where(query => query.OrganizationId == organizationId)
            .OrderBy(query => query.CreatedAt)
            .ToListAsync(cancellationToken);
}
