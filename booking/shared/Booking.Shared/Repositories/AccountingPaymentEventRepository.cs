using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IAccountingPaymentEventRepository : IRepository<AccountingPaymentEvent>
{
    AccountingPaymentEvent Add(AccountingPaymentEvent accountingPaymentEvent);
    AccountingPaymentEvent Update(AccountingPaymentEvent accountingPaymentEvent);

    Task<AccountingPaymentEvent?> GetByProviderAndExternalPaymentIdAsync(
        string organizationId,
        string provider,
        string externalPaymentId,
        CancellationToken cancellationToken);

    Task<ICollection<AccountingPaymentEvent>> GetUnprocessedByProviderAndExternalInvoiceIdAsync(
        string organizationId,
        string provider,
        string externalInvoiceId,
        CancellationToken cancellationToken);
}

public class AccountingPaymentEventRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, AccountingPaymentEvent>(dbContext, timeProvider), IAccountingPaymentEventRepository
{
    public AccountingPaymentEvent Add(AccountingPaymentEvent accountingPaymentEvent)
    {
        var now = TimeProvider.GetUtcNow();
        accountingPaymentEvent.CreatedAt = now;
        return DbContext.AccountingPaymentEvent.Add(accountingPaymentEvent).Entity;
    }

    public AccountingPaymentEvent Update(AccountingPaymentEvent accountingPaymentEvent)
    {
        var now = TimeProvider.GetUtcNow();
        accountingPaymentEvent.ModifiedAt = now;
        return DbContext.AccountingPaymentEvent.Update(accountingPaymentEvent).Entity;
    }

    public async Task<AccountingPaymentEvent?> GetByProviderAndExternalPaymentIdAsync(
        string organizationId,
        string provider,
        string externalPaymentId,
        CancellationToken cancellationToken) =>
        await DbContext.AccountingPaymentEvent.FirstOrDefaultAsync(
            query =>
                query.OrganizationId == organizationId &&
                query.Provider == provider &&
                query.ExternalPaymentId == externalPaymentId,
            cancellationToken);

    public async Task<ICollection<AccountingPaymentEvent>> GetUnprocessedByProviderAndExternalInvoiceIdAsync(
        string organizationId,
        string provider,
        string externalInvoiceId,
        CancellationToken cancellationToken) =>
        await DbContext.AccountingPaymentEvent
            .Where(query =>
                query.OrganizationId == organizationId &&
                query.Provider == provider &&
                query.ExternalInvoiceId == externalInvoiceId &&
                query.ProcessedAt == null)
            .ToListAsync(cancellationToken);
}
