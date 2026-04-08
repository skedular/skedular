using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.GraphQL;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;

namespace Booking.Api.UnitTests.Services.MarketplaceRefundAdminServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarkManualCompletedAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Update_Refund_Status_To_Manual_Completed_And_Raise_Subscription_Graphql_Change(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMarketplaceRefundService marketplaceRefundService,
        [Frozen] IGraphQlTopicEventSender graphQlTopicEventSender,
        [Frozen] IMarketplaceRefundEventService marketplaceRefundEventService,
        [Frozen] IMarketplaceRefundNotificationService marketplaceRefundNotificationService,
        [Frozen] IUnitOfWork unitOfWork,
        MarketplaceRefundAdminService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            LocalEntityId = "subscription-1",
            Status = MarketplaceRefundStatusConstants.ManualRequired
        };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(refund.Id, cancellationToken)).Returns(refund);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundService.HasConfirmedPaymentAsync(refund, cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        var result = await sut.MarkManualCompletedAsync(refund.Id, "Manual bank transfer confirmed", cancellationToken);

        result.ShouldBe(refund);
        refund.Status.ShouldBe(MarketplaceRefundStatusConstants.ManualCompleted);
        refund.Reason.ShouldBe("Manual bank transfer confirmed");
        refund.LastProcessedAt.ShouldNotBeNull();
        A.CallTo(() => marketplaceRefundEventService.Add(refund, MarketplaceRefundEventTypeConstants.ManualCompleted, "customer-1",
                A<DateTimeOffset?>.That.Matches(value => value.HasValue)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.MarketplaceBookingSubscriptionTopicName, "subscription-1",
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceRefundNotificationService.NotifyStatusChangedAsync(refund, cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
