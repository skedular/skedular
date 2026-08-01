using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.Extensions.Logging;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundAutomationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class LoggingShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Information_When_Stripe_Refund_Completes_Successfully(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IStripeHostRefundService stripeHostRefundService,
        [Frozen] IXeroRefundService xeroRefundService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] ILogger<MarketplaceRefundAutomationService> logger,
        MarketplaceRefundAutomationService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1", Status = MarketplaceRefundStatusConstants.Requested, RefundAmount = 50m, RefundKind = "Cancellation"
        };
        var completedRefund = new MarketplaceRefund
        {
            Id = refund.Id, Status = MarketplaceRefundStatusConstants.Completed, ExternalPaymentRefundId = "re_stripe1"
        };

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.Update(A<MarketplaceRefund>._)).ReturnsLazily(call => call.GetArgument<MarketplaceRefund>(0)!);
        A.CallTo(() => stripeHostRefundService.IsHostRefundAsync(refund, cancellationToken)).Returns(true);
        A.CallTo(() => stripeHostRefundService.CanProcessAsync(refund, cancellationToken)).Returns(true);
        A.CallTo(() => stripeHostRefundService.ProcessAsync(refund, cancellationToken)).Returns(completedRefund);
        A.CallTo(() => xeroRefundService.GetProcessingAvailabilityAsync(completedRefund, cancellationToken))
            .Returns(new XeroRefundProcessingAvailability(false, "No Xero connection"));

        await sut.ProcessAfterRequestAsync(refund, null, cancellationToken);

        A.CallTo(logger)
            .Where(call => call.Method.Name == nameof(ILogger.Log) && call.GetArgument<LogLevel>(0) == LogLevel.Information)
            .MustHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Warning_When_Stripe_Checkout_Session_Is_Missing_For_Host_Refund(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IStripeHostRefundService stripeHostRefundService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] ILogger<MarketplaceRefundAutomationService> logger,
        MarketplaceRefundAutomationService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1", Status = MarketplaceRefundStatusConstants.Requested, RefundAmount = 50m, RefundKind = "Cancellation"
        };

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.Update(A<MarketplaceRefund>._)).ReturnsLazily(call => call.GetArgument<MarketplaceRefund>(0)!);
        A.CallTo(() => stripeHostRefundService.IsHostRefundAsync(refund, cancellationToken)).Returns(true);
        A.CallTo(() => stripeHostRefundService.CanProcessAsync(refund, cancellationToken)).Returns(false);

        await sut.ProcessAfterRequestAsync(refund, null, cancellationToken);

        A.CallTo(logger)
            .Where(call => call.Method.Name == nameof(ILogger.Log) && call.GetArgument<LogLevel>(0) == LogLevel.Warning)
            .MustHaveHappenedOnceOrMore();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Warning_When_Xero_Processing_Is_Blocked(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IStripeHostRefundService stripeHostRefundService,
        [Frozen] IXeroRefundService xeroRefundService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] ILogger<MarketplaceRefundAutomationService> logger,
        MarketplaceRefundAutomationService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1", Status = MarketplaceRefundStatusConstants.Requested, RefundAmount = 50m, RefundKind = "Cancellation"
        };

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.Update(A<MarketplaceRefund>._)).ReturnsLazily(call => call.GetArgument<MarketplaceRefund>(0)!);
        A.CallTo(() => stripeHostRefundService.IsHostRefundAsync(refund, cancellationToken)).Returns(false);
        A.CallTo(() => xeroRefundService.GetProcessingAvailabilityAsync(A<MarketplaceRefund>._, cancellationToken))
            .Returns(new XeroRefundProcessingAvailability(false, "Invoice not found in Xero"));

        await sut.ProcessAfterRequestAsync(refund, null, cancellationToken);

        A.CallTo(logger)
            .Where(call => call.Method.Name == nameof(ILogger.Log) && call.GetArgument<LogLevel>(0) == LogLevel.Warning)
            .MustHaveHappenedOnceOrMore();
    }
}
