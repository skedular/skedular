using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Stripe;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundReconciliationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ReconcileAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Request_A_Bounded_Reconciliation_Batch(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] TimeProvider timeProvider,
        MarketplaceRefundReconciliationService sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(TimeProvider.System.GetUtcNow());
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetRefundsForReconciliationAsync(
                A<DateTimeOffset>._, 100, cancellationToken))
            .Returns([]);

        await sut.ReconcileAsync(cancellationToken);

        A.CallTo(() => marketplaceRefundRepository.GetRefundsForReconciliationAsync(
                A<DateTimeOffset>._, 100, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reconcile_A_Completed_Xero_Refund_When_Its_Last_Check_Is_Older_Than_The_Threshold(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IXeroRefundService xeroRefundService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] TimeProvider timeProvider,
        MarketplaceRefundReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.Completed,
            AccountingProvider = AccountingProviderConstants.Xero,
            ExternalRefundId = Guid.NewGuid().ToString(),
            LastReconciledAt = TimeProvider.System.GetUtcNow().AddDays(-1),
            RefundAmount = 25m
        };
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(TimeProvider.System.GetUtcNow());
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetRefundsForReconciliationAsync(A<DateTimeOffset>._, 100, cancellationToken))
            .Returns([refund]);
        A.CallTo(() => marketplaceRefundRepository.TryClaimReconciliationAsync(
                refund.Id, A<string>._, A<DateTimeOffset>._, A<TimeSpan>._, cancellationToken))
            .Returns(true);
        A.CallTo(() => xeroRefundService.ReconcileAsync(refund, A<DateTimeOffset>._, cancellationToken))
            .ReturnsLazily(() =>
            {
                refund.ReconciliationStatus = "Matched";
                return true;
            });
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        await sut.ReconcileAsync(cancellationToken);

        A.CallTo(() => xeroRefundService.ReconcileAsync(refund, A<DateTimeOffset>._, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => xeroRefundService.ReconcileAsync(
                refund,
                A<DateTimeOffset>.That.Matches(value => value == refund.LastReconciledAt),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceRefundRepository.ReleaseReconciliationLeaseAsync(
                refund.Id, A<string>._, cancellationToken))
            .MustHaveHappenedOnceExactly();
        refund.ReconciliationStatus.ShouldBe("Matched");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_A_Completed_Xero_Refund_When_The_Credit_Note_Is_Not_Yet_Settled(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IXeroRefundService xeroRefundService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] TimeProvider timeProvider,
        MarketplaceRefundReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.Completed,
            AccountingProvider = AccountingProviderConstants.Xero,
            ExternalRefundId = Guid.NewGuid().ToString(),
            LastReconciledAt = TimeProvider.System.GetUtcNow().AddDays(-1),
            RefundAmount = 25m
        };
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(TimeProvider.System.GetUtcNow());
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetRefundsForReconciliationAsync(A<DateTimeOffset>._, 100, cancellationToken))
            .Returns([refund]);
        A.CallTo(() => marketplaceRefundRepository.TryClaimReconciliationAsync(
                refund.Id, A<string>._, A<DateTimeOffset>._, A<TimeSpan>._, cancellationToken))
            .Returns(true);
        A.CallTo(() => xeroRefundService.ReconcileAsync(refund, A<DateTimeOffset>._, cancellationToken)).Returns(false);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        await sut.ReconcileAsync(cancellationToken);

        refund.Status.ShouldBe(MarketplaceRefundStatusConstants.Completed);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceRefundRepository.ReleaseReconciliationLeaseAsync(
                refund.Id, A<string>._, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Prefer_Xero_Reconciliation_When_A_Stripe_Refund_Has_A_Xero_Projection(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IXeroRefundService xeroRefundService,
        [Frozen] IStripeHostRefundService stripeHostRefundService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] TimeProvider timeProvider,
        MarketplaceRefundReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.Completed,
            PaymentProvider = "STRIPE",
            ExternalPaymentRefundId = "re_1",
            AccountingProvider = AccountingProviderConstants.Xero,
            ExternalRefundId = Guid.NewGuid().ToString(),
            LastReconciledAt = TimeProvider.System.GetUtcNow().AddDays(-1),
            RefundAmount = 25m
        };
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(TimeProvider.System.GetUtcNow());
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetRefundsForReconciliationAsync(A<DateTimeOffset>._, 100, cancellationToken))
            .Returns([refund]);
        A.CallTo(() => marketplaceRefundRepository.TryClaimReconciliationAsync(
                refund.Id, A<string>._, A<DateTimeOffset>._, A<TimeSpan>._, cancellationToken))
            .Returns(true);
        A.CallTo(() => xeroRefundService.ReconcileAsync(refund, A<DateTimeOffset>._, cancellationToken))
            .Returns(true);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        await sut.ReconcileAsync(cancellationToken);

        A.CallTo(() => xeroRefundService.ReconcileAsync(refund, A<DateTimeOffset>._, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => stripeHostRefundService.GetProviderRefundAsync(A<string>._, null, cancellationToken))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Keep_An_Unknown_Stripe_Path_Out_Of_Matched_Reconciliation(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IStripeHostRefundService stripeHostRefundService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] TimeProvider timeProvider,
        MarketplaceRefundReconciliationService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.ReconciliationRequired,
            PaymentProvider = "STRIPE",
            ExternalPaymentRefundId = "re_1",
            ReconciliationStatus = "UnknownStripeRefundPath",
            RefundAmount = 25m
        };
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(TimeProvider.System.GetUtcNow());
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetRefundsForReconciliationAsync(A<DateTimeOffset>._, 100, cancellationToken))
            .Returns([refund]);
        A.CallTo(() => marketplaceRefundRepository.TryClaimReconciliationAsync(
                refund.Id, A<string>._, A<DateTimeOffset>._, A<TimeSpan>._, cancellationToken))
            .Returns(true);
        A.CallTo(() => stripeHostRefundService.GetProviderRefundAsync("re_1", null, cancellationToken))
            .Returns(new Refund { Id = "re_1", Status = "succeeded" });
        A.CallTo(() => stripeHostRefundService.ReconcileAsync(
                A<Refund>._, cancellationToken, A<string?>._))
            .Returns(refund);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        await sut.ReconcileAsync(cancellationToken);

        refund.ReconciliationStatus.ShouldBe("UnknownStripeRefundPath");
        A.CallTo(() => stripeHostRefundService.ReconcileAsync(A<Refund>._, cancellationToken, A<string?>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).MustHaveHappenedOnceExactly();
    }
}
