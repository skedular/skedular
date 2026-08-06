using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;

namespace Booking.Api.UnitTests.Services.MarketplaceRefundAdminServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ApproveAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Approve_An_Existing_Refund_Without_Rechecking_Mutable_Payment_Status(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        IMarketplaceRefundTransitionService refundTransitionService,
        [Frozen]
        ITemporalOutboxService temporalOutboxService,
        [Frozen]
        IUnitOfWork unitOfWork,
        MarketplaceRefundAdminService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "booking-1",
            Status = MarketplaceRefundStatusConstants.UnderReview,
        };
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("operator-1");
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(refund.Id, cancellationToken)).Returns(refund);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "operator-1", cancellationToken)).Returns(true);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => refundTransitionService.TransitionAsync(refund, MarketplaceRefundStatusConstants.Approved, null, "operator-1", A<string?>._,
                cancellationToken))
            .ReturnsLazily(() =>
            {
                refund.Status = MarketplaceRefundStatusConstants.Approved;
                return Task.FromResult(refund);
            });

        var result = await sut.ApproveAsync(refund.Id, cancellationToken);

        result.Status.ShouldBe(MarketplaceRefundStatusConstants.Approved.ToMarketplaceRefundStatus());
        A.CallTo(() => temporalOutboxService.StartWorkflowProcessMarketplaceRefund(
                new ProcessMarketplaceRefundInput(refund.Id, null), unitOfWork))
            .MustHaveHappenedOnceExactly();
    }
}
