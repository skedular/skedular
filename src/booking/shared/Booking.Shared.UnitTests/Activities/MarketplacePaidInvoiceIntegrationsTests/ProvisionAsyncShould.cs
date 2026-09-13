using Booking.Shared.Activities;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Temporalio.Testing;

namespace Booking.Shared.UnitTests.Activities.MarketplacePaidInvoiceIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ProvisionAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Complete_WhenInvoiceIsProvisioned(
        [Frozen]
        IMarketplacePaidInvoiceProvisioner provisioner,
        MarketplacePaidInvoiceIntegrations sut,
        ProvisionMarketplacePaidInvoiceInput input)
    {
        input = input with
        {
            MarketplaceBookingSubscriptionId = null,
            EntitlementPurchaseId = "entitlement-1",
        };
        input = input with
        {
            StripeAccountId = "acct",
            InvoiceId = "invoice",
        };
        var activityEnvironment = new ActivityEnvironment();

        A.CallTo(() => provisioner.ProvisionAsync(
                input.MarketplaceBookingSubscriptionId,
                input.EntitlementPurchaseId,
                input.StripeAccountId!,
                input.InvoiceId!,
                input.InvoicePeriodStart,
                input.PaymentIntentId,
                activityEnvironment.CancellationTokenSource.Token))
            .Returns(true);

        await activityEnvironment.RunAsync(() => sut.ProvisionAsync(input));

        A.CallTo(() => provisioner.ProvisionAsync(
            input.MarketplaceBookingSubscriptionId,
            input.EntitlementPurchaseId,
            input.StripeAccountId!,
            input.InvoiceId!,
            input.InvoicePeriodStart,
            input.PaymentIntentId,
            activityEnvironment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task ThrowRetryableFailure_WhenInvoiceCycleIsNotReady(
        [Frozen]
        IMarketplacePaidInvoiceProvisioner provisioner,
        MarketplacePaidInvoiceIntegrations sut,
        ProvisionMarketplacePaidInvoiceInput input)
    {
        input = input with
        {
            StripeAccountId = "acct",
            InvoiceId = "invoice",
        };
        var activityEnvironment = new ActivityEnvironment();

        A.CallTo(() => provisioner.ProvisionAsync(
                A<string?>._,
                A<string?>._,
                A<string>._!,
                A<string>._!,
                A<DateTimeOffset>._,
                A<string?>._,
                activityEnvironment.CancellationTokenSource.Token))
            .Returns(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() => activityEnvironment.RunAsync(() => sut.ProvisionAsync(input)));

        A.CallTo(() => provisioner.ProvisionAsync(
                input.MarketplaceBookingSubscriptionId,
                input.EntitlementPurchaseId,
                input.StripeAccountId!,
                input.InvoiceId!,
                input.InvoicePeriodStart,
                input.PaymentIntentId,
                activityEnvironment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }
}
