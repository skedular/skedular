using Api.Shared.Services.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Entitlements;

namespace Booking.Shared.Services;

public interface IMarketplacePaidInvoiceProvisioner
{
    Task<bool> ProvisionAsync(
        string? marketplaceBookingSubscriptionId,
        string? entitlementPurchaseId,
        string stripeAccountId,
        string invoiceId,
        DateTimeOffset invoicePeriodStart,
        string? paymentIntentId,
        CancellationToken cancellationToken);
}

public sealed class MarketplacePaidInvoiceProvisioner(
    IRepositoryFactory repositoryFactory,
    IEntitlementPurchaseService entitlementPurchaseService)
    : IMarketplacePaidInvoiceProvisioner
{
    public async Task<bool> ProvisionAsync(
        string? marketplaceBookingSubscriptionId,
        string? entitlementPurchaseId,
        string stripeAccountId,
        string invoiceId,
        DateTimeOffset invoicePeriodStart,
        string? paymentIntentId,
        CancellationToken cancellationToken)
    {
        if (marketplaceBookingSubscriptionId is not null)
        {
            var recurringBooking = (await repositoryFactory.RecurringBookingRepository
                    .GetByMarketplaceBookingSubscriptionIdAsync(marketplaceBookingSubscriptionId, cancellationToken))
                .SingleOrDefault(item =>
                    item.StartDate.UtcDateTime.Date <= invoicePeriodStart.UtcDateTime.Date &&
                    item.EndDate.HasValue &&
                    item.EndDate.Value.UtcDateTime.Date >= invoicePeriodStart.UtcDateTime.Date &&
                    item.MarketplaceBooking?.PaymentStatus is PaymentStatusConstants.Pending or PaymentStatusConstants.Confirmed);
            if (recurringBooking?.MarketplaceBooking is null)
            {
                return false;
            }

            recurringBooking.MarketplaceBooking.StripeInvoiceId = invoiceId;
            recurringBooking.MarketplaceBooking.StripePaymentIntentId = paymentIntentId;
            if (recurringBooking.MarketplaceBooking.PaymentStatus == PaymentStatusConstants.Confirmed)
            {
                repositoryFactory.MarketplaceBookingRepository.Update(recurringBooking.MarketplaceBooking);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                return true;
            }

            recurringBooking.MarketplaceBooking.PaymentStatus = PaymentStatusConstants.Confirmed;
            repositoryFactory.MarketplaceBookingRepository.Update(recurringBooking.MarketplaceBooking);
            await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForMarketplaceBookingAsync(
                recurringBooking.MarketplaceBooking.Id,
                cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        if (entitlementPurchaseId is not null)
        {
            await entitlementPurchaseService.ConfirmStripeSubscriptionInvoiceAsync(
                entitlementPurchaseId,
                stripeAccountId,
                invoiceId,
                invoicePeriodStart,
                paymentIntentId,
                cancellationToken);
            return true;
        }

        return false;
    }
}
