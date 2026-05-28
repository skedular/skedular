using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IAccountingContactLinkRepository : IRepository<AccountingContactLink>
{
    AccountingContactLink Add(AccountingContactLink accountingContactLink);
    AccountingContactLink Update(AccountingContactLink accountingContactLink);

    Task<AccountingContactLink?> GetByProviderAndLocalEntityAsync(
        string organizationId,
        string provider,
        string localEntityType,
        string localEntityId,
        CancellationToken cancellationToken);

    Task<AccountingContactLink?> GetByProviderAndExternalContactIdAsync(
        string organizationId,
        string provider,
        string externalContactId,
        CancellationToken cancellationToken);
}

public class AccountingContactLinkRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, AccountingContactLink>(dbContext, timeProvider), IAccountingContactLinkRepository
{
    public AccountingContactLink Add(AccountingContactLink accountingContactLink)
    {
        var now = TimeProvider.GetUtcNow();
        accountingContactLink.CreatedAt = now;
        return DbContext.AccountingContactLink.Add(accountingContactLink).Entity;
    }

    public AccountingContactLink Update(AccountingContactLink accountingContactLink)
    {
        var now = TimeProvider.GetUtcNow();
        accountingContactLink.ModifiedAt = now;
        return DbContext.AccountingContactLink.Update(accountingContactLink).Entity;
    }

    public async Task<AccountingContactLink?> GetByProviderAndLocalEntityAsync(
        string organizationId,
        string provider,
        string localEntityType,
        string localEntityId,
        CancellationToken cancellationToken) =>
        await DbContext.AccountingContactLink.FirstOrDefaultAsync(
            query =>
                query.OrganizationId == organizationId &&
                query.Provider == provider &&
                query.LocalEntityType == localEntityType &&
                query.LocalEntityId == localEntityId,
            cancellationToken);

    public async Task<AccountingContactLink?> GetByProviderAndExternalContactIdAsync(
        string organizationId,
        string provider,
        string externalContactId,
        CancellationToken cancellationToken) =>
        await DbContext.AccountingContactLink.FirstOrDefaultAsync(
            query =>
                query.OrganizationId == organizationId &&
                query.Provider == provider &&
                query.ExternalContactId == externalContactId,
            cancellationToken);
}
