using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Stripe;
using Product = Booking.Shared.Database.Entities.Product;
using RefundEntityType = Booking.Shared.Models.MarketplaceRefundEntityTypeConstants;
using RefundStatus = Booking.Shared.Models.MarketplaceRefundStatusConstants;

namespace Booking.Shared.UnitTests.Services.StripeHostRefundServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ProcessAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Fall_Back_To_Platform_Funds_When_Transfer_Reversal_Is_Unavailable(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IStripeHostRefundClient stripeClient,
        StripeHostRefundService sut,
        CancellationToken cancellationToken)
    {
        var marketplaceBooking = CreateHostBooking();
        var refund = CreateRefund(marketplaceBooking);
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceBookingRepository.GetByIdAsync(marketplaceBooking.Id, cancellationToken)).Returns(marketplaceBooking);
        A.CallTo(() => stripeClient.CreateRefundAsync(
                A<RefundCreateOptions>.That.Matches(options => options.ReverseTransfer == true), refund.Id, cancellationToken, A<string?>._))
            .ThrowsAsync(new StripeException("provider rejected transfer reversal")
            {
                StripeError = new StripeError
                {
                    Code = "transfer_reversal_not_allowed",
                },
            });
        A.CallTo(() => stripeClient.CreateRefundAsync(
                A<RefundCreateOptions>.That.Matches(options => options.ReverseTransfer == false),
                $"{refund.Id}:stripe-refund:platform:fee",
                cancellationToken,
                A<string?>._))
            .Returns(new Refund
            {
                Id = "re_platform",
                Status = "succeeded",
            });
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        var result = await sut.ProcessAsync(refund, cancellationToken);

        result.Status.ShouldBe(RefundStatus.Completed);
        result.StripeRefundPath.ShouldBe("PlatformFunded");
        result.ExternalPaymentRefundId.ShouldBe("re_platform");
        result.PostPayoutRefund.ShouldBeTrue();
        result.StripeChargeType.ShouldBe("Destination");
        result.StripeTransferId.ShouldBe("tr_1");
        A.CallTo(() => stripeClient.CreateRefundAsync(
                A<RefundCreateOptions>.That.Matches(options => options.ReverseTransfer == true), refund.Id, cancellationToken, A<string?>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => stripeClient.CreateRefundAsync(
                A<RefundCreateOptions>.That.Matches(options => options.ReverseTransfer == false),
                $"{refund.Id}:stripe-refund:platform:fee",
                cancellationToken,
                A<string?>._))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Keep_An_Ambiguous_Provider_Submission_Pending_For_Reconciliation(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IStripeHostRefundClient stripeClient,
        StripeHostRefundService sut,
        CancellationToken cancellationToken)
    {
        var marketplaceBooking = CreateHostBooking();
        var refund = CreateRefund(marketplaceBooking);
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceBookingRepository.GetByIdAsync(marketplaceBooking.Id, cancellationToken)).Returns(marketplaceBooking);
        A.CallTo(() => stripeClient.CreateRefundAsync(A<RefundCreateOptions>._, refund.Id, cancellationToken, A<string?>._))
            .ThrowsAsync(new StripeException("Stripe request timed out."));
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        var result = await sut.ProcessAsync(refund, cancellationToken);

        result.Status.ShouldBe(RefundStatus.ProviderPending);
        result.PaymentRefundStatus.ShouldBe(RefundStatus.ProviderPending);
        result.StripeRefundPath.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Retry_Without_Application_Fee_Reversal_When_The_Charge_Has_No_Application_Fee(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IStripeHostRefundClient stripeClient,
        StripeHostRefundService sut,
        CancellationToken cancellationToken)
    {
        var marketplaceBooking = CreateHostBooking();
        var refund = CreateRefund(marketplaceBooking);
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceBookingRepository.GetByIdAsync(marketplaceBooking.Id, cancellationToken)).Returns(marketplaceBooking);
        A.CallTo(() => stripeClient.CreateRefundAsync(
                A<RefundCreateOptions>.That.Matches(options => options.RefundApplicationFee == true),
                refund.Id,
                cancellationToken,
                A<string?>._))
            .ThrowsAsync(new StripeException("Attempting to refund_application_fee on ch_1, but it has no application fee"));
        A.CallTo(() => stripeClient.CreateRefundAsync(
                A<RefundCreateOptions>.That.Matches(options => options.RefundApplicationFee == false),
                $"{refund.Id}:stripe-refund:reverse:no-fee",
                cancellationToken,
                A<string?>._))
            .Returns(new Refund
            {
                Id = "re_without_fee",
                Status = "succeeded",
            });
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        var result = await sut.ProcessAsync(refund, cancellationToken);

        result.Status.ShouldBe(RefundStatus.Completed);
        result.ExternalPaymentRefundId.ShouldBe("re_without_fee");
        A.CallTo(() => stripeClient.CreateRefundAsync(
                A<RefundCreateOptions>.That.Matches(options => options.RefundApplicationFee == true),
                refund.Id,
                cancellationToken,
                A<string?>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => stripeClient.CreateRefundAsync(
                A<RefundCreateOptions>.That.Matches(options => options.RefundApplicationFee == false),
                $"{refund.Id}:stripe-refund:reverse:no-fee",
                cancellationToken,
                A<string?>._))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Process_Using_Persisted_Stripe_Context_When_The_Checkout_Is_Unavailable(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IStripeHostRefundClient stripeClient,
        StripeHostRefundService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-persisted-context",
            LocalEntityType = RefundEntityType.MarketplaceBooking,
            LocalEntityId = "booking-no-longer-loadable",
            RefundAmount = 12.34m,
            Currency = "nzd",
            StripePaymentIntentId = "pi_persisted",
            StripeAccountId = "acct_persisted",
            StripeChargeType = "Direct",
            StripeChargeId = "ch_persisted",
            StripeTransferId = "tr_persisted",
        };
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => stripeClient.CreateRefundAsync(
                A<RefundCreateOptions>.That.Matches(options =>
                    options.PaymentIntent == "pi_persisted" && options.ReverseTransfer == false),
                refund.Id,
                cancellationToken,
                "acct_persisted"))
            .Returns(new Refund
            {
                Id = "re_persisted",
                Status = "succeeded",
            });
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        (await sut.IsHostRefundAsync(refund, cancellationToken)).ShouldBeTrue();
        (await sut.CanProcessAsync(refund, cancellationToken)).ShouldBeTrue();
        var result = await sut.ProcessAsync(refund, cancellationToken);

        result.Status.ShouldBe(RefundStatus.Completed);
        result.ExternalPaymentRefundId.ShouldBe("re_persisted");
        A.CallTo(() => marketplaceBookingRepository.GetByIdAsync(refund.LocalEntityId, cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => stripeClient.CreateRefundAsync(
                A<RefundCreateOptions>._, refund.Id, cancellationToken, "acct_persisted"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Route_An_Unknown_Stripe_Charge_Type_To_Reconciliation(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IStripeHostRefundClient stripeClient,
        StripeHostRefundService sut,
        CancellationToken cancellationToken)
    {
        var marketplaceBooking = CreateHostBooking();
        marketplaceBooking.StripeCheckoutSession!.ChargeType = "UnknownChargeType";
        var refund = CreateRefund(marketplaceBooking);
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceBookingRepository.GetByIdAsync(marketplaceBooking.Id, cancellationToken)).Returns(marketplaceBooking);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        var result = await sut.ProcessAsync(refund, cancellationToken);

        result.Status.ShouldBe(RefundStatus.ReconciliationRequired);
        result.LastError.ShouldNotBeNull();
        result.LastError.ShouldContain("charge type is unknown");
        result.StripeChargeType.ShouldBe("UnknownChargeType");
        result.StripeChargeId.ShouldBe("ch_1");
        result.StripePaymentIntentId.ShouldBe("pi_1");
        result.StripeRefundPath.ShouldBeNull();
        A.CallTo(() => stripeClient.CreateRefundAsync(A<RefundCreateOptions>._, A<string>._, A<CancellationToken>._, A<string?>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [InlineAutoFakeItEasyData(new Type[] { }, "pending", RefundStatus.ProviderPending)]
    [InlineAutoFakeItEasyData(new Type[] { }, "succeeded", RefundStatus.Completed)]
    [InlineAutoFakeItEasyData(new Type[] { }, "failed", RefundStatus.Failed)]
    public async Task Reverse_The_Application_Fee_And_Map_Stripe_Status(
        string stripeStatus,
        string expectedStatus,
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IStripeHostRefundClient stripeClient,
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
            Currency = "nzd",
        };
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceBookingRepository.GetByIdAsync(marketplaceBooking.Id, cancellationToken)).Returns(marketplaceBooking);
        A.CallTo(() => stripeClient.CreateRefundAsync(A<RefundCreateOptions>._, refund.Id, cancellationToken, A<string?>._))
            .Returns(new Refund
            {
                Id = "re_1",
                Status = stripeStatus,
            });
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        var result = await sut.ProcessAsync(refund, cancellationToken);

        result.Status.ShouldBe(expectedStatus);
        result.ExternalPaymentRefundId.ShouldBe("re_1");
        result.PaymentRefundStatus.ShouldBe(expectedStatus);
        result.StripeRefundPath.ShouldBe("TransferReversal");
        A.CallTo(() => stripeClient.CreateRefundAsync(
                A<RefundCreateOptions>.That.Matches(options =>
                    options.PaymentIntent == "pi_1" &&
                    options.Amount == 1234 &&
                    options.RefundApplicationFee == true &&
                    options.ReverseTransfer == true),
                refund.Id,
                cancellationToken,
                A<string?>._))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Resolve_A_Subscription_Stripe_Refund_From_Its_Source_Payment_Allocation(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IStripeCheckoutSessionRepository stripeCheckoutSessionRepository,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IStripeHostRefundClient stripeClient,
        StripeHostRefundService sut,
        CancellationToken cancellationToken)
    {
        var checkout = CreateHostBooking().StripeCheckoutSession!;
        var refund = new MarketplaceRefund
        {
            Id = "subscription-refund-1",
            LocalEntityType = RefundEntityType.MarketplaceBookingSubscription,
            LocalEntityId = "subscription-1",
            RefundAmount = 12.34m,
            Currency = "nzd",
            PaymentAllocations =
            [
                new MarketplaceRefundPaymentAllocation
                {
                    IsSourcePayment = true,
                    SourcePaymentProvider = "STRIPE",
                    SourcePaymentReference = checkout.StripeCheckoutSessionId,
                },
            ],
        };
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(stripeCheckoutSessionRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => stripeCheckoutSessionRepository.GetByStripeCheckoutSessionIdAsync(
                checkout.StripeCheckoutSessionId,
                cancellationToken))
            .Returns(checkout);
        A.CallTo(() => stripeClient.CreateRefundAsync(
                A<RefundCreateOptions>.That.Matches(options => options.PaymentIntent == checkout.PaymentIntentId),
                refund.Id,
                cancellationToken,
                A<string?>._))
            .Returns(new Refund
            {
                Id = "re_subscription",
                Status = "succeeded",
            });
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        (await sut.IsHostRefundAsync(refund, cancellationToken)).ShouldBeTrue();
        var result = await sut.ProcessAsync(refund, cancellationToken);

        result.Status.ShouldBe(RefundStatus.Completed);
        result.ExternalPaymentRefundId.ShouldBe("re_subscription");
        A.CallTo(() => stripeClient.CreateRefundAsync(
                A<RefundCreateOptions>._,
                refund.Id,
                cancellationToken,
                A<string?>._))
            .MustHaveHappenedOnceExactly();
    }

    private static MarketplaceBooking CreateHostBooking()
    {
        var organization = new Organization
        {
            Id = "org-1",
            Type = OrganizationTypeConstants.Host,
        };
        var product = new Product
        {
            Id = "product-1",
            Organization = organization,
        };
        var productVersion = new ProductVersion
        {
            Id = "version-1",
            Product = product,
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            Id = "marketplace-booking-1",
            ProductVersion = productVersion,
        };
        marketplaceBooking.StripeCheckoutSession = new StripeCheckoutSession
        {
            Id = "checkout-1",
            StripeCheckoutSessionId = "cs_1",
            CheckoutUrl = "https://example.test",
            PaymentIntentId = "pi_1",
            ChargeId = "ch_1",
            ChargeType = "Destination",
            TransferId = "tr_1",
            PayoutDisbursedAt = TimeProvider.System.GetUtcNow(),
            DestinationAccountId = "acct_1",
            MarketplaceBooking = marketplaceBooking,
            StripeCustomer = new StripeCustomer
            {
                Id = "customer-1",
                StripeCustomerId = "cus_1",
                StripeAccountId = "acct_1",
            },
        };
        return marketplaceBooking;
    }

    private static MarketplaceRefund CreateRefund(MarketplaceBooking marketplaceBooking) => new()
    {
        Id = "refund-1",
        LocalEntityType = RefundEntityType.MarketplaceBooking,
        LocalEntityId = marketplaceBooking.Id,
        RefundAmount = 12.34m,
        Currency = "nzd",
    };
}
