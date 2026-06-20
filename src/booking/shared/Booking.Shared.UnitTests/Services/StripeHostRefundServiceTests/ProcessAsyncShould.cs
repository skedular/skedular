using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Stripe;
using Stripe.Checkout;
using Product = Booking.Shared.Database.Entities.Product;
using RefundEntityType = Booking.Shared.Models.MarketplaceRefundEntityTypeConstants;
using RefundStatus = Booking.Shared.Models.MarketplaceRefundStatusConstants;

namespace Booking.Shared.UnitTests.Services.StripeHostRefundServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ProcessAsyncShould
{
    [Theory]
    [InlineAutoFakeItEasyData(new Type[] { }, "pending", RefundStatus.PendingAccounting)]
    [InlineAutoFakeItEasyData(new Type[] { }, "succeeded", RefundStatus.Completed)]
    [InlineAutoFakeItEasyData(new Type[] { }, "failed", RefundStatus.Failed)]
    public async Task Reverse_The_Application_Fee_And_Map_Stripe_Status(
        string stripeStatus,
        string expectedStatus,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IStripeHostRefundClient stripeClient,
        StripeHostRefundService sut,
        CancellationToken cancellationToken)
    {
        var marketplaceBooking = CreateHostBooking();
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            LocalEntityType = RefundEntityType.MarketplaceBooking,
            LocalEntityId = marketplaceBooking.Id,
            RefundAmount = 12.34m,
            Currency = "nzd"
        };
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceBookingRepository.GetByIdAsync(marketplaceBooking.Id, cancellationToken)).Returns(marketplaceBooking);
        A.CallTo(() => stripeClient.GetSessionAsync("cs_1", cancellationToken))
            .Returns(new Session { PaymentIntentId = "pi_1" });
        A.CallTo(() => stripeClient.CreateRefundAsync(A<RefundCreateOptions>._, refund.Id, cancellationToken))
            .Returns(new Refund { Id = "re_1", Status = stripeStatus });
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        var result = await sut.ProcessAsync(refund, cancellationToken);

        result.Status.ShouldBe(expectedStatus);
        result.ExternalPaymentRefundId.ShouldBe("re_1");
        result.PaymentRefundStatus.ShouldBe(expectedStatus);
        A.CallTo(() => stripeClient.CreateRefundAsync(
                A<RefundCreateOptions>.That.Matches(options =>
                    options.PaymentIntent == "pi_1" &&
                    options.Amount == 1234 &&
                    options.RefundApplicationFee == true &&
                    options.ReverseTransfer == true),
                refund.Id,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    private static MarketplaceBooking CreateHostBooking()
    {
        var organization = new Organization { Id = "org-1", Type = OrganizationTypeConstants.Host };
        var product = new Product { Id = "product-1", Organization = organization };
        var productVersion = new ProductVersion { Id = "version-1", Product = product };
        var marketplaceBooking = new MarketplaceBooking { Id = "marketplace-booking-1", ProductVersion = productVersion };
        marketplaceBooking.StripeCheckoutSession = new StripeCheckoutSession
        {
            Id = "checkout-1",
            StripeCheckoutSessionId = "cs_1",
            CheckoutUrl = "https://example.test",
            MarketplaceBooking = marketplaceBooking,
            StripeCustomer = new StripeCustomer { Id = "customer-1", StripeCustomerId = "cus_1", StripeAccountId = "acct_1" }
        };
        return marketplaceBooking;
    }
}
