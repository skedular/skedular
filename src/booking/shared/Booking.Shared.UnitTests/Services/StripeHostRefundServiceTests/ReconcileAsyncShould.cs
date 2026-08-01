using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Stripe;

namespace Booking.Shared.UnitTests.Services.StripeHostRefundServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ReconcileAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Require_Reconciliation_When_A_Succeeded_Refund_Has_No_Persisted_Path(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IMarketplaceRefundTransitionService refundTransitionService,
        StripeHostRefundService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund { Id = "refund-1", Status = MarketplaceRefundStatusConstants.Processing };
        var stripeRefund = new Refund { Id = "re_1", Status = "succeeded", Metadata = [] };
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByExternalPaymentRefundIdAsync(stripeRefund.Id, cancellationToken))
            .Returns(refund);
        A.CallTo(() => refundTransitionService.TransitionAsync(
                refund,
                MarketplaceRefundStatusConstants.ReconciliationRequired,
                A<string?>._,
                null,
                stripeRefund.Id,
                cancellationToken))
            .Returns(refund);

        var result = await sut.ReconcileAsync(stripeRefund, cancellationToken);

        result.ShouldBe(refund);
        refund.StripeRefundPath.ShouldBeNull();
        refund.StripeRefundPathSelectedAt.ShouldBeNull();
        refund.ReconciliationStatus.ShouldBe("UnknownStripeRefundPath");
        refund.LastError.ShouldNotBeNull();
        refund.LastError.ShouldContain("path context");
        A.CallTo(() => refundTransitionService.TransitionAsync(
                refund,
                MarketplaceRefundStatusConstants.ReconciliationRequired,
                A<string?>._,
                null,
                "re_1",
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Scope_An_Unmatched_Stripe_Refund_To_Its_Organization(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IStripeCustomerRepository stripeCustomerRepository,
        [Frozen] IUnitOfWork unitOfWork,
        StripeHostRefundService sut,
        CancellationToken cancellationToken)
    {
        var stripeRefund = new Refund { Id = "re_1", Amount = 1250, Currency = "nzd", Metadata = [] };
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.StripeCustomerRepository).Returns(stripeCustomerRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetByExternalPaymentRefundIdAsync(stripeRefund.Id, cancellationToken))
            .Returns((MarketplaceRefund?)null);
        A.CallTo(() => marketplaceRefundRepository.GetExternalReconciliationAsync(
                "STRIPE", stripeRefund.Id, null, cancellationToken))
            .Returns((MarketplaceExternalRefundReconciliation?)null);
        A.CallTo(() => stripeCustomerRepository.GetOrganizationIdByStripeAccountIdAsync("acct_1", cancellationToken))
            .Returns("org-1");

        await sut.ReconcileAsync(stripeRefund, cancellationToken, "acct_1");

        A.CallTo(() => marketplaceRefundRepository.AddExternalReconciliation(
                A<MarketplaceExternalRefundReconciliation>.That.Matches(item =>
                    item.Provider == "STRIPE" &&
                    item.ExternalRefundId == "re_1" &&
                    item.OrganizationId == "org-1" &&
                    item.StripeAccountId == "acct_1")))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Correlate_By_Metadata_And_Persist_A_Status_Change(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IMarketplaceRefundTransitionService refundTransitionService,
        StripeHostRefundService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1", Status = MarketplaceRefundStatusConstants.Processing, StripeRefundPath = "TransferReversal"
        };
        var stripeRefund = new Refund
        {
            Id = "re_1", Status = "succeeded", Metadata = new Dictionary<string, string> { ["marketplace_refund_id"] = refund.Id }
        };
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByExternalPaymentRefundIdAsync(stripeRefund.Id, cancellationToken))
            .Returns((MarketplaceRefund?)null);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(refund.Id, cancellationToken)).Returns(refund);
        A.CallTo(() => refundTransitionService.TransitionAsync(
                refund,
                MarketplaceRefundStatusConstants.Completed,
                A<string?>._,
                null,
                stripeRefund.Id,
                cancellationToken))
            .Returns(refund);

        var result = await sut.ReconcileAsync(stripeRefund, cancellationToken);

        result.ShouldBe(refund);
        A.CallTo(() => refundTransitionService.TransitionAsync(
                refund,
                MarketplaceRefundStatusConstants.Completed,
                A<string?>._,
                null,
                "re_1",
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Ignore_A_Duplicate_Status_Event(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IMarketplaceRefundEventService marketplaceRefundEventService,
        [Frozen] IUnitOfWork unitOfWork,
        StripeHostRefundService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.Failed,
            PaymentRefundStatus = MarketplaceRefundStatusConstants.Completed,
            StripeRefundPath = "TransferReversal"
        };
        var stripeRefund = new Refund { Id = "re_1", Status = "succeeded", Metadata = [] };
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetByExternalPaymentRefundIdAsync(stripeRefund.Id, cancellationToken)).Returns(refund);

        var result = await sut.ReconcileAsync(stripeRefund, cancellationToken);

        result.ShouldBeNull();
        A.CallTo(() => marketplaceRefundEventService.Add(A<MarketplaceRefund>._, A<string>._, A<string?>._, A<DateTimeOffset?>._))
            .MustNotHaveHappened();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Keep_An_Unknown_Path_In_Reconciliation_When_Status_Is_Unchanged(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IUnitOfWork unitOfWork,
        StripeHostRefundService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.ReconciliationRequired,
            PaymentRefundStatus = MarketplaceRefundStatusConstants.ReconciliationRequired
        };
        var stripeRefund = new Refund { Id = "re_1", Status = "succeeded", Metadata = [] };
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetByExternalPaymentRefundIdAsync(stripeRefund.Id, cancellationToken))
            .Returns(refund);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        var result = await sut.ReconcileAsync(stripeRefund, cancellationToken);

        result.ShouldBe(refund);
        refund.ReconciliationStatus.ShouldBe("UnknownStripeRefundPath");
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
