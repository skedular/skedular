using Api.Shared.Services.Models;
using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Stripe;
using Stripe.Checkout;
using Temporalio.Testing;
using Constants = Api.Shared.Services.Models.PaymentStatusConstants;

namespace Booking.Shared.UnitTests.Activities.StripeIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public sealed class CreateEntitlementCheckoutSessionAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task UseThePurchasePricingAndCorrelationWithoutLoadingABooking(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementPurchaseRepository purchaseRepository,
        [Frozen]
        IProductVersionRepository productVersionRepository,
        [Frozen]
        IStripeProductPricingService stripeProductPricingService,
        [Frozen]
        ICreatable<Session, SessionCreateOptions> sessionCreateService,
        StripeIntegrations sut,
        string purchaseId,
        string productVersionId,
        string pricingId,
        string stripePriceId)
    {
        var activityEnvironment = new ActivityEnvironment();
        var pricing = ProductPricing.Empty(pricingId) with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 5,
            EntitlementValidityDays = 30,
        };
        var purchase = new EntitlementPurchase
        {
            Id = purchaseId,
            ProductVersionId = productVersionId,
            PaymentStatus = PaymentStatus.Pending.ToPaymentStatus(),
            ProductPricing = pricing,
            Amount = 100,
            Currency = "NZD",
        };
        var stripeProduct = new StripeProduct
        {
            ProductPricingId = pricingId,
            StripePrices =
            [
                new StripePrice
                {
                    StripePriceId = stripePriceId,
                },
            ],
        };
        var productVersion = new ProductVersion
        {
            Id = productVersionId,
            StripeProducts = [stripeProduct],
        };
        var session = new Session
        {
            Id = "cs_entitlement",
            Url = "https://checkout.stripe.test/session",
            PaymentStatus = "unpaid",
        };

        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).Returns(purchaseRepository);
        A.CallTo(() => purchaseRepository.GetByIdAsync(purchaseId, activityEnvironment.CancellationTokenSource.Token)).Returns(purchase);
        A.CallTo(() => productVersionRepository.GetByIdAsync(productVersionId, activityEnvironment.CancellationTokenSource.Token))
            .Returns(productVersion);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => stripeProductPricingService.GetOneTimePriceId(productVersion, pricing, "acct_1"))
            .Returns(stripePriceId);
        A.CallTo(() => sessionCreateService.CreateAsync(A<SessionCreateOptions>._, A<RequestOptions>._,
                activityEnvironment.CancellationTokenSource.Token))
            .Returns(session);

        var result = await activityEnvironment.RunAsync(() => sut.CreateEntitlementCheckoutSessionAsync(
            new CreateEntitlementCheckoutSessionAsyncInput(purchaseId, "acct_1", "cus_1")));

        result!.CheckoutUrl.ShouldBe(session.Url);
        result.PaymentStatus.ShouldBe(Constants.Pending);
        A.CallTo(() => sessionCreateService.CreateAsync(
                A<SessionCreateOptions>.That.Matches(options =>
                    options.ClientReferenceId == purchaseId &&
                    options.Metadata["purchase_id"] == purchaseId &&
                    options.Metadata["pricing_id"] == pricingId &&
                    options.Metadata["amount"] == "100" &&
                    options.Metadata["currency"] == "NZD" &&
                    options.LineItems.Count == 1 &&
                    options.LineItems[0].Price == stripePriceId &&
                    options.LineItems[0].Quantity == 1),
                A<RequestOptions>.That.Matches(options => options.IdempotencyKey == purchaseId),
                activityEnvironment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => repositoryFactory.BookingRepository).MustNotHaveHappened();
    }
}
