using Api.Shared.Services.Models;
using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IEntitlementPurchaseRepository : IRepository<EntitlementPurchase>
{
    EntitlementPurchase Add(EntitlementPurchase purchase);
    EntitlementPurchase Update(EntitlementPurchase purchase);

    Task<bool> UpdateCheckoutReturnUrlAsync(
        string purchaseId,
        string checkoutReturnUrl,
        CancellationToken cancellationToken);

    Task<bool> UpdateBankTransferInvoiceAsync(
        string purchaseId,
        string invoiceNumber,
        string paymentInstructions,
        CancellationToken cancellationToken);

    Task<bool> UpdateCardCheckoutAsync(
        string purchaseId,
        string checkoutSessionId,
        string checkoutUrl,
        string? paymentIntentId,
        string stripeAccountId,
        CancellationToken cancellationToken);

    Task<EntitlementPurchase?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<EntitlementPurchase>> GetForCustomerAsync(string customerId, CancellationToken cancellationToken);
    Task<IReadOnlyList<EntitlementPurchase>> GetForOrganizationAsync(string organizationId, CancellationToken cancellationToken);
    Task<IReadOnlyList<EntitlementPurchase>> GetExpiredPendingAsync(DateTimeOffset now, CancellationToken cancellationToken);
    Task<EntitlementPurchase?> GetByRenewalReferenceAsync(string renewalReference, CancellationToken cancellationToken);
}

public sealed class EntitlementPurchaseRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, EntitlementPurchase>(dbContext, timeProvider), IEntitlementPurchaseRepository
{
    public EntitlementPurchase Add(EntitlementPurchase purchase) => DbContext.EntitlementPurchase.Add(purchase).Entity;

    public EntitlementPurchase Update(EntitlementPurchase purchase) => DbContext.EntitlementPurchase.Update(purchase).Entity;

    public async Task<bool> UpdateCheckoutReturnUrlAsync(
        string purchaseId,
        string checkoutReturnUrl,
        CancellationToken cancellationToken)
    {
        var updated = await DbContext.EntitlementPurchase
            .Where(item => item.Id == purchaseId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(item => item.CheckoutReturnUrl, checkoutReturnUrl)
                    .SetProperty(item => item.ModifiedAt, TimeProvider.GetUtcNow()),
                cancellationToken);
        if (updated == 0)
        {
            return false;
        }

        // ExecuteUpdate bypasses EF tracking. Keep an already-tracked purchase
        // in this request consistent so the checkout service does not reuse the
        // original __PURCHASE_ID__ template when it builds the Stripe session.
        var trackedPurchase = DbContext.ChangeTracker.Entries<EntitlementPurchase>()
            .FirstOrDefault(entry => entry.Entity.Id == purchaseId)?.Entity;
        if (trackedPurchase is not null)
        {
            // ExecuteUpdate advances PostgreSQL's xmin. Reload the tracked row so
            // a later SaveChanges call in the same request does not use a stale
            // concurrency token.
            await DbContext.Entry(trackedPurchase).ReloadAsync(cancellationToken);
        }

        return true;
    }

    public async Task<bool> UpdateBankTransferInvoiceAsync(
        string purchaseId,
        string invoiceNumber,
        string paymentInstructions,
        CancellationToken cancellationToken)
    {
        var updated = await DbContext.EntitlementPurchase
            .Where(item => item.Id == purchaseId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(item => item.InvoiceNumber, invoiceNumber)
                    .SetProperty(item => item.PaymentInstructions, paymentInstructions)
                    .SetProperty(item => item.ModifiedAt, TimeProvider.GetUtcNow()),
                cancellationToken);
        if (updated == 0)
        {
            return false;
        }

        var trackedPurchase = DbContext.ChangeTracker.Entries<EntitlementPurchase>()
            .FirstOrDefault(entry => entry.Entity.Id == purchaseId)?.Entity;
        if (trackedPurchase is not null)
        {
            await DbContext.Entry(trackedPurchase).ReloadAsync(cancellationToken);
        }

        return true;
    }

    public async Task<bool> UpdateCardCheckoutAsync(
        string purchaseId,
        string checkoutSessionId,
        string checkoutUrl,
        string? paymentIntentId,
        string stripeAccountId,
        CancellationToken cancellationToken) =>
        await DbContext.EntitlementPurchase
            .Where(item => item.Id == purchaseId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(item => item.StripeCheckoutSessionId, checkoutSessionId)
                    .SetProperty(item => item.StripeCheckoutUrl, checkoutUrl)
                    .SetProperty(item => item.StripePaymentIntentId, paymentIntentId)
                    .SetProperty(item => item.StripeAccountId, stripeAccountId)
                    .SetProperty(item => item.ModifiedAt, TimeProvider.GetUtcNow()),
                cancellationToken) > 0;

    public Task<EntitlementPurchase?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        DbContext.EntitlementPurchase
            .Include(item => item.Customer)
            .Include(item => item.ProductVersion)
            .ThenInclude(item => item.Product)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

    public async Task<IReadOnlyList<EntitlementPurchase>> GetForCustomerAsync(string customerId, CancellationToken cancellationToken) =>
        await DbContext.EntitlementPurchase
            .Where(item => item.CustomerId == customerId)
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<EntitlementPurchase>> GetForOrganizationAsync(string organizationId, CancellationToken cancellationToken) =>
        await DbContext.EntitlementPurchase
            .AsNoTracking()
            .Where(item => item.OrganizationId == organizationId)
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<EntitlementPurchase>> GetExpiredPendingAsync(DateTimeOffset now, CancellationToken cancellationToken) =>
        await DbContext.EntitlementPurchase
            .Where(item => item.PaymentStatus == PaymentStatusConstants.Pending && item.PaymentExpiry <= now)
            .OrderBy(item => item.PaymentExpiry)
            .ToListAsync(cancellationToken);

    public Task<EntitlementPurchase?> GetByRenewalReferenceAsync(string renewalReference, CancellationToken cancellationToken) =>
        DbContext.EntitlementPurchase
            .Include(item => item.Customer)
            .Include(item => item.ProductVersion)
            .ThenInclude(item => item.Product)
            .SingleOrDefaultAsync(item => item.RenewalReference == renewalReference, cancellationToken);
}
