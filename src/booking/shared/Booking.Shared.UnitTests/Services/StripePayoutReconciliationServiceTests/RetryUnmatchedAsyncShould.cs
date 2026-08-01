using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Stripe;

namespace Booking.Shared.UnitTests.Services.StripePayoutReconciliationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RetryUnmatchedAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Reprocess_Open_Payouts_And_Mark_A_Matched_Payout_Resolved(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IStripeCheckoutSessionRepository stripeCheckoutSessionRepository,
        [Frozen] IStripeHostRefundClient stripeHostRefundClient,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] TimeProvider timeProvider,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var reconciliation = new MarketplaceExternalRefundReconciliation
        {
            Id = "reconciliation-1",
            Provider = "STRIPE_PAYOUT",
            ExternalRefundId = "po_1",
            StripeAccountId = "acct_1",
            Status = "Open"
        };
        var now = new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        var checkout = new StripeCheckoutSession { PaymentIntentId = "pi_1" };
        var payout = new Payout { Id = "po_1", Status = "paid", Metadata = new Dictionary<string, string> { ["payment_intent_id"] = "pi_1" } };

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(stripeCheckoutSessionRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetOpenStripePayoutReconciliationsAsync(
                timeProvider.GetUtcNow(), 100, cancellationToken))
            .Returns([reconciliation]);
        A.CallTo(() => stripeHostRefundClient.GetPayoutAsync("po_1", "acct_1", cancellationToken))
            .Returns(payout);
        A.CallTo(() => stripeCheckoutSessionRepository.GetByPaymentIntentIdAsync("pi_1", cancellationToken))
            .Returns(checkout);
        A.CallTo(() => marketplaceRefundRepository.GetExternalReconciliationAsync(
                "STRIPE_PAYOUT", "po_1", null, cancellationToken))
            .Returns(reconciliation);
        A.CallTo(() => marketplaceRefundRepository.UpdateExternalReconciliation(reconciliation))
            .Returns(reconciliation);

        await sut.RetryUnmatchedAsync(cancellationToken);

        checkout.PayoutId.ShouldBe("po_1");
        reconciliation.Status.ShouldBe("Resolved");
        reconciliation.RetryCount.ShouldBe(0);
        A.CallTo(() => stripeHostRefundClient.GetPayoutAsync("po_1", "acct_1", cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceRefundRepository.UpdateExternalReconciliation(reconciliation))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Stop_After_Third_Failed_Retry_And_Require_Manual_Review(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IStripeHostRefundClient stripeHostRefundClient,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] TimeProvider timeProvider,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var now = new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero);
        var reconciliation = new MarketplaceExternalRefundReconciliation
        {
            Id = "reconciliation-1",
            Provider = "STRIPE_PAYOUT",
            ExternalRefundId = "po_1",
            StripeAccountId = "acct_1",
            Status = "Open",
            RetryCount = 2
        };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetOpenStripePayoutReconciliationsAsync(now, 100, cancellationToken))
            .Returns([reconciliation]);
        A.CallTo(() => marketplaceRefundRepository.GetExternalReconciliationAsync(
                "STRIPE_PAYOUT", "po_1", null, cancellationToken))
            .Returns(reconciliation);
        A.CallTo(() => stripeHostRefundClient.GetPayoutAsync("po_1", "acct_1", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("provider unavailable"));
        A.CallTo(() => marketplaceRefundRepository.UpdateExternalReconciliation(reconciliation))
            .Returns(reconciliation);

        await sut.RetryUnmatchedAsync(cancellationToken);

        reconciliation.RetryCount.ShouldBe(3);
        reconciliation.NextRetryAt.ShouldBeNull();
        reconciliation.ResolutionReason.ShouldNotBeNull();
        reconciliation.ResolutionReason!.ShouldContain("Manual review is required");
        A.CallTo(() => marketplaceRefundRepository.UpdateExternalReconciliation(reconciliation))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineAutoFakeItEasyData(new Type[] { }, "failed")]
    [InlineAutoFakeItEasyData(new Type[] { }, "canceled")]
    public async Task Process_NonPaid_Retrieved_Payouts_Through_State_Handling(
        string payoutStatus,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IStripeCheckoutSessionRepository stripeCheckoutSessionRepository,
        [Frozen] IStripeHostRefundClient stripeHostRefundClient,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] TimeProvider timeProvider,
        StripePayoutReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var now = new DateTimeOffset(2026, 7, 28, 10, 0, 0, TimeSpan.Zero);
        var reconciliation = new MarketplaceExternalRefundReconciliation
        {
            Id = "reconciliation-1",
            Provider = "STRIPE_PAYOUT",
            ExternalRefundId = "po_1",
            StripeAccountId = "acct_1",
            Status = "Open"
        };
        var checkout = new StripeCheckoutSession { PayoutId = "po_1", PayoutStatus = "paid", PayoutDisbursedAt = now };
        var payout = new Payout { Id = "po_1", Status = payoutStatus };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.StripeCheckoutSessionRepository).Returns(stripeCheckoutSessionRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetOpenStripePayoutReconciliationsAsync(now, 100, cancellationToken))
            .Returns([reconciliation]);
        A.CallTo(() => stripeHostRefundClient.GetPayoutAsync("po_1", "acct_1", cancellationToken)).Returns(payout);
        A.CallTo(() => stripeCheckoutSessionRepository.GetByPayoutIdAsync("po_1", cancellationToken)).Returns(checkout);
        A.CallTo(() => marketplaceRefundRepository.GetByStripePaymentContextAsync(
                A<string?>._, A<string?>._, A<string?>._, cancellationToken))
            .Returns([]);
        A.CallTo(() => marketplaceRefundRepository.GetExternalReconciliationAsync(
                "STRIPE_PAYOUT", "po_1", null, cancellationToken))
            .Returns(reconciliation);
        A.CallTo(() => marketplaceRefundRepository.UpdateExternalReconciliation(reconciliation))
            .Returns(reconciliation);

        await sut.RetryUnmatchedAsync(cancellationToken);

        checkout.PayoutStatus.ShouldBe(payoutStatus);
        checkout.PayoutDisbursedAt.ShouldBeNull();
        reconciliation.Status.ShouldBe("Open");
        A.CallTo(() => stripeHostRefundClient.GetPayoutAsync("po_1", "acct_1", cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceRefundRepository.UpdateExternalReconciliation(reconciliation))
            .MustHaveHappenedOnceExactly();
    }
}
