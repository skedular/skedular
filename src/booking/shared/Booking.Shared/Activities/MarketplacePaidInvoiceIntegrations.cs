using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.GraphQL;
using Temporalio.Activities;
using Constants = Booking.Shared.GraphQL.Constants;

namespace Booking.Shared.Activities;

public sealed class MarketplacePaidInvoiceIntegrations(
    IMarketplacePaidInvoiceProvisioner provisioner,
    IRepositoryFactory repositoryFactory,
    IGraphQlTopicEventSender graphQlTopicEventSender)
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
                subscription.StripeCurrentPeriodEndsAt = input.InvoicePeriodEnd;
                repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(subscription);
                var recurringBooking = await repositoryFactory.RecurringBookingRepository
                    .GetByMarketplaceBookingSubscriptionIdAsync(subscription.Id, cancellationToken);
                var paidBooking = recurringBooking.SingleOrDefault(item =>
                    item.StartDate.UtcDateTime.Date <= input.InvoicePeriodStart.UtcDateTime.Date &&
                    item.EndDate.HasValue &&
                    item.EndDate.Value.UtcDateTime.Date >= input.InvoicePeriodStart.UtcDateTime.Date)?.MarketplaceBooking;
                if (paidBooking is not null)
                {
                    paidBooking.StripeInvoiceId = input.InvoiceId;
                    paidBooking.StripePaymentIntentId = input.PaymentIntentId;
                    repositoryFactory.MarketplaceBookingRepository.Update(paidBooking);
                }

                await repositoryFactory.MarketplacePurchaseHistoryRepository.AppendEventAsync(
                    new MarketplacePurchaseHistoryEventModel(
                        $"stripe:invoice:{input.StripeAccountId}:{input.InvoiceId}",
                        subscription.Id,
                        MarketplacePurchaseHistoryEligibleSourceType.Subscription,
                        MarketplacePurchaseHistoryEventType.PaymentStateChanged,
                        input.InvoicePeriodStart,
                        DateTimeOffset.UtcNow,
                        null,
                        PaymentStatus.Confirmed,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        input.InvoicePeriodEnd,
                        $"Stripe invoice {input.InvoiceId} was paid.",
                        AutoRenew: true,
                        CorrelationId: $"invoice:{input.StripeAccountId}:{input.InvoiceId}"),
                    $"invoice:{input.StripeAccountId}:{input.InvoiceId}",
                    cancellationToken);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                    Constants.MarketplaceBookingSubscriptionTopicName, subscription.Id, cancellationToken);
            }
        }
        else if (input.EntitlementPurchaseId is not null)
        {
            var purchase = await repositoryFactory.EntitlementPurchaseRepository
                .GetByIdAsync(input.EntitlementPurchaseId, cancellationToken);
            if (purchase is not null)
            {
                purchase.StripeSubscriptionStatus = "active";
                purchase.StripeCurrentPeriodEndsAt = input.InvoicePeriodEnd;
                repositoryFactory.EntitlementPurchaseRepository.Update(purchase);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                    Constants.EntitlementPurchaseTopicName, purchase.Id, cancellationToken);
            }
        }
    }
}
