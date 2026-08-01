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
public class CancelAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_Cancellation_For_An_Unauthorised_Organization_User(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        MarketplaceRefundAdminService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund { Id = "refund-1", OrganizationId = "org-1", Status = MarketplaceRefundStatusConstants.UnderReview };
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("ordinary-user");
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(refund.Id, cancellationToken)).Returns(refund);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "ordinary-user", cancellationToken)).Returns(false);

        await Should.ThrowAsync<UnauthorizedAccessException>(() => sut.CancelAsync(refund.Id, "not allowed", cancellationToken));
        refund.Status.ShouldBe(MarketplaceRefundStatusConstants.UnderReview);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Cancel_Authorised_Unsubmitted_Refund_And_Publish_The_Change(
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
            Status = MarketplaceRefundStatusConstants.UnderReview
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

        var result = await sut.CancelAsync(refund.Id, "Customer asked to withdraw the request", cancellationToken);

        result.Id.ShouldBe(refund.Id);
        refund.Status.ShouldBe(MarketplaceRefundStatusConstants.Cancelled);
        refund.CancellationReason.ShouldBe("Customer asked to withdraw the request");
        refund.CancelledAt.ShouldNotBeNull();
        A.CallTo(() => refundTransitionService.TransitionAsync(refund, MarketplaceRefundStatusConstants.Cancelled,
                "Customer asked to withdraw the request", "operator-1", A<string?>._, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_Cancellation_After_Provider_Submission(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMarketplaceRefundService marketplaceRefundService,
        MarketplaceRefundAdminService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "booking-1",
            Status = MarketplaceRefundStatusConstants.ProviderPending,
            ExternalPaymentRefundId = "re_1"
        };
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("operator-1");
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(refund.Id, cancellationToken)).Returns(refund);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "operator-1", cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundService.HasConfirmedPaymentAsync(refund, cancellationToken)).Returns(true);

        await Should.ThrowAsync<InvalidOperationException>(() => sut.CancelAsync(refund.Id, "Too late", cancellationToken));
        refund.Status.ShouldBe(MarketplaceRefundStatusConstants.ProviderPending);
    }
}
