using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundAutomationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ProcessAfterRequestAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Complete_The_Stripe_Refund_Without_Creating_A_Xero_Refund(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IMarketplaceRefundTransitionService transitionService,
        [Frozen] IStripeHostRefundService stripeHostRefundService,
        [Frozen] IXeroRefundService xeroRefundService,
        [Frozen] IUnitOfWork unitOfWork,
        MarketplaceRefundAutomationService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund { Id = "refund-1", Status = MarketplaceRefundStatusConstants.Requested, RefundAmount = 50m };
        var completedRefund = new MarketplaceRefund
        {
            Id = refund.Id,
            Status = MarketplaceRefundStatusConstants.Completed,
            PaymentProvider = "STRIPE",
            PaymentRefundStatus = MarketplaceRefundStatusConstants.Completed
        };
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.Update(A<MarketplaceRefund>._)).ReturnsLazily(call => call.GetArgument<MarketplaceRefund>(0)!);
        A.CallTo(() => transitionService.TransitionAsync(A<MarketplaceRefund>._, A<string>._, A<string?>._, A<string?>._, A<string?>._,
                cancellationToken))
            .ReturnsLazily(call =>
            {
                var value = call.GetArgument<MarketplaceRefund>(0)!;
                value.Status = call.GetArgument<string>(1)!;
                value.LastError = call.GetArgument<string?>(2);
                return Task.FromResult(value);
            });
        A.CallTo(() => stripeHostRefundService.IsHostRefundAsync(refund, cancellationToken)).Returns(true);
        A.CallTo(() => stripeHostRefundService.CanProcessAsync(refund, cancellationToken)).Returns(true);
        A.CallTo(() => stripeHostRefundService.ProcessAsync(refund, cancellationToken)).Returns(completedRefund);
        var result = await sut.ProcessAfterRequestAsync(refund, "customer-1", cancellationToken);

        result.ShouldBe(completedRefund);
        A.CallTo(() => xeroRefundService.GetProcessingAvailabilityAsync(A<MarketplaceRefund>._, cancellationToken)).MustNotHaveHappened();
        A.CallTo(() => xeroRefundService.ProcessAsync(A<MarketplaceRefund>._, cancellationToken)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Require_Manual_Handling_When_A_Host_Card_Refund_Has_No_Stripe_Correlation(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IMarketplaceRefundTransitionService transitionService,
        [Frozen] IStripeHostRefundService stripeHostRefundService,
        [Frozen] IXeroRefundService xeroRefundService,
        [Frozen] IUnitOfWork unitOfWork,
        MarketplaceRefundAutomationService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund { Id = "refund-1", Status = MarketplaceRefundStatusConstants.Requested };
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => transitionService.TransitionAsync(A<MarketplaceRefund>._, A<string>._, A<string?>._, A<string?>._, A<string?>._,
                cancellationToken))
            .ReturnsLazily(call =>
            {
                var value = call.GetArgument<MarketplaceRefund>(0)!;
                value.Status = call.GetArgument<string>(1)!;
                value.LastError = call.GetArgument<string?>(2);
                return Task.FromResult(value);
            });
        A.CallTo(() => stripeHostRefundService.IsHostRefundAsync(refund, cancellationToken)).Returns(true);
        A.CallTo(() => stripeHostRefundService.CanProcessAsync(refund, cancellationToken)).Returns(false);

        var result = await sut.ProcessAfterRequestAsync(refund, null, cancellationToken);

        result.Status.ShouldBe(MarketplaceRefundStatusConstants.Failed);
        A.CallTo(() => xeroRefundService.GetProcessingAvailabilityAsync(A<MarketplaceRefund>._, cancellationToken)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Process_In_Xero_When_Availability_Is_Confirmed(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IMarketplaceRefundTransitionService transitionService,
        [Frozen] IStripeHostRefundService stripeHostRefundService,
        [Frozen] IXeroRefundService xeroRefundService,
        [Frozen] IUnitOfWork unitOfWork,
        MarketplaceRefundAutomationService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund { Id = "refund-1", Status = MarketplaceRefundStatusConstants.Requested, RefundAmount = 50m };
        var completedRefund = new MarketplaceRefund
        {
            Id = refund.Id,
            Status = MarketplaceRefundStatusConstants.Completed,
            LastProcessedAt = new DateTimeOffset(2026, 4, 8, 9, 0, 0, TimeSpan.Zero)
        };

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => transitionService.TransitionAsync(A<MarketplaceRefund>._, A<string>._, A<string?>._, A<string?>._, A<string?>._,
                cancellationToken))
            .ReturnsLazily(call =>
            {
                var value = call.GetArgument<MarketplaceRefund>(0)!;
                value.Status = call.GetArgument<string>(1)!;
                value.LastError = call.GetArgument<string?>(2);
                return Task.FromResult(value);
            });
        A.CallTo(() => stripeHostRefundService.IsHostRefundAsync(refund, cancellationToken)).Returns(false);
        A.CallTo(() => xeroRefundService.GetProcessingAvailabilityAsync(refund, cancellationToken))
            .Returns(new XeroRefundProcessingAvailability(true, null));
        A.CallTo(() => xeroRefundService.ProcessAsync(refund, cancellationToken)).Returns(completedRefund);

        var result = await sut.ProcessAfterRequestAsync(refund, "customer-1", cancellationToken);

        result.ShouldBe(completedRefund);
        refund.Status.ShouldBe(MarketplaceRefundStatusConstants.Processing);
        A.CallTo(() => transitionService.TransitionAsync(completedRefund, MarketplaceRefundStatusConstants.Completed, A<string?>._,
            "customer-1", A<string?>._, cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Fall_Back_To_Manual_Required_When_Xero_Is_Not_Available(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IMarketplaceRefundTransitionService transitionService,
        [Frozen] IXeroRefundService xeroRefundService,
        [Frozen] IUnitOfWork unitOfWork,
        MarketplaceRefundAutomationService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund { Id = "refund-1", Status = MarketplaceRefundStatusConstants.Requested, RefundAmount = 50m };

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => transitionService.TransitionAsync(A<MarketplaceRefund>._, A<string>._, A<string?>._, A<string?>._, A<string?>._,
                cancellationToken))
            .ReturnsLazily(call =>
            {
                var value = call.GetArgument<MarketplaceRefund>(0)!;
                value.Status = call.GetArgument<string>(1)!;
                value.LastError = call.GetArgument<string?>(2);
                return Task.FromResult(value);
            });
        A.CallTo(() => xeroRefundService.GetProcessingAvailabilityAsync(refund, cancellationToken))
            .Returns(new XeroRefundProcessingAvailability(false, "Invoice correlation is missing."));

        var result = await sut.ProcessAfterRequestAsync(refund, "customer-1", cancellationToken);

        result.ShouldBe(refund);
        refund.Status.ShouldBe(MarketplaceRefundStatusConstants.Failed);
        refund.LastError.ShouldBe("Invoice correlation is missing.");
        A.CallTo(() => transitionService.TransitionAsync(refund, MarketplaceRefundStatusConstants.Failed,
            "Invoice correlation is missing.", "customer-1", A<string?>._, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => xeroRefundService.ProcessAsync(A<MarketplaceRefund>._, cancellationToken)).MustNotHaveHappened();
    }
}
