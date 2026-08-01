using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Database;

namespace Booking.Api.UnitTests.Services.MarketplaceRefundAdminServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RetryAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Move_A_Failed_Refund_To_Processing_And_Preserve_Idempotency(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMarketplaceRefundService marketplaceRefundService,
        [Frozen] IMarketplaceRefundTransitionService refundTransitionService,
        [Frozen] IUnitOfWork unitOfWork,
        MarketplaceRefundAdminService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "booking-1",
            Status = MarketplaceRefundStatusConstants.Failed,
            RetryCount = 2,
            IdempotencyKey = "cancellation:MarketplaceBooking:booking-1"
        };
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("operator-1");
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(refund.Id, cancellationToken)).Returns(refund);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "operator-1", cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundService.HasConfirmedPaymentAsync(refund, cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);
        A.CallTo(() => refundTransitionService.TransitionAsync(refund, A<string>._, A<string?>._, A<string?>._, A<string?>._, cancellationToken))
            .ReturnsLazily(call =>
            {
                var value = call.GetArgument<MarketplaceRefund>(0)!;
                value.Status = call.GetArgument<string>(1)!;
                return Task.FromResult(value);
            });

        var result = await sut.RetryAsync(refund.Id, cancellationToken);

        result.Status.ShouldBe(MarketplaceRefundStatusConstants.Processing.ToMarketplaceRefundStatus());
        result.RetryCount.ShouldBe(3);
        result.IdempotencyKey.ShouldBe("cancellation:MarketplaceBooking:booking-1");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Allow_Manual_Retry_When_Automatic_Retry_Count_Is_Reached(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMarketplaceRefundService marketplaceRefundService,
        [Frozen] IMarketplaceRefundTransitionService refundTransitionService,
        MarketplaceRefundAdminService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "booking-1",
            Status = MarketplaceRefundStatusConstants.Failed,
            RetryCount = 3
        };
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("operator-1");
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(refund.Id, cancellationToken)).Returns(refund);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "operator-1", cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundService.HasConfirmedPaymentAsync(refund, cancellationToken)).Returns(true);
        A.CallTo(() => refundTransitionService.TransitionAsync(refund, A<string>._, A<string?>._, A<string?>._, A<string?>._, cancellationToken))
            .ReturnsLazily(call =>
            {
                var value = call.GetArgument<MarketplaceRefund>(0)!;
                value.Status = call.GetArgument<string>(1)!;
                return Task.FromResult(value);
            });

        var result = await sut.RetryAsync(refund.Id, cancellationToken);

        result.Status.ShouldBe(MarketplaceRefundStatusConstants.Processing.ToMarketplaceRefundStatus());
        result.RetryCount.ShouldBe(4);
    }
}
