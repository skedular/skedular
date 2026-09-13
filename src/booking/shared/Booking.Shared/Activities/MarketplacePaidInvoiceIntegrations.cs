using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public sealed class MarketplacePaidInvoiceIntegrations(IMarketplacePaidInvoiceProvisioner provisioner, IRepositoryFactory repositoryFactory)
{
    [Activity]
    public async Task ProvisionAsync(ProvisionMarketplacePaidInvoiceInput input)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var provisioned = await provisioner.ProvisionAsync(
            input.MarketplaceBookingSubscriptionId,
            input.EntitlementPurchaseId,
            input.StripeAccountId,
            input.InvoiceId,
            input.InvoicePeriodStart,
            input.PaymentIntentId,
            cancellationToken);
        if (!provisioned)
        {
            throw new InvalidOperationException("The paid Stripe invoice is waiting for its marketplace cycle to be created.");
        }

        if (input.MarketplaceBookingSubscriptionId is not null)
        {
            var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository
                .GetByIdAsync(input.MarketplaceBookingSubscriptionId, cancellationToken);
            if (subscription is not null)
            {
                subscription.StripeSubscriptionStatus = "active";
                repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(subscription);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
