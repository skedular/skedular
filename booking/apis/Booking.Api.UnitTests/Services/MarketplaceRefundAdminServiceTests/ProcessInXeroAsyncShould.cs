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
public class ProcessInXeroAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Process_Refund_In_Xero_And_Raise_Subscription_Graphql_Change(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMarketplaceRefundService marketplaceRefundService,
        [Frozen] IGraphQlTopicEventSender graphQlTopicEventSender,
        [Frozen] IMarketplaceRefundEventService marketplaceRefundEventService,
        [Frozen] IMarketplaceRefundNotificationService marketplaceRefundNotificationService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IXeroRefundService xeroRefundService,
        MarketplaceRefundAdminService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            LocalEntityId = "subscription-1",
            Status = MarketplaceRefundStatusConstants.PendingAccounting
        };
        var processedRefund = new MarketplaceRefund
        {
            Id = refund.Id,
            OrganizationId = refund.OrganizationId,
            LocalEntityType = refund.LocalEntityType,
            LocalEntityId = refund.LocalEntityId,
            Status = MarketplaceRefundStatusConstants.Completed
        };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(refund.Id, cancellationToken)).Returns(refund);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundService.HasConfirmedPaymentAsync(refund, cancellationToken)).Returns(true);
        A.CallTo(() => xeroRefundService.ProcessAsync(refund, cancellationToken)).Returns(processedRefund);

        var result = await sut.ProcessInXeroAsync(refund.Id, cancellationToken);

        result.ShouldBe(processedRefund);
        A.CallTo(() => marketplaceRefundEventService.Add(refund, MarketplaceRefundEventTypeConstants.SentToXero, "customer-1",
                A<DateTimeOffset?>.That.Matches(value => value.HasValue)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceRefundEventService.Add(processedRefund, MarketplaceRefundEventTypeConstants.Completed, "customer-1",
                A<DateTimeOffset?>.That.Matches(value => value.HasValue)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.MarketplaceBookingSubscriptionTopicName,
                "subscription-1",
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceRefundNotificationService.NotifyStatusChangedAsync(processedRefund, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_Payment_Is_Not_Confirmed(
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
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            LocalEntityId = "subscription-1",
            Status = MarketplaceRefundStatusConstants.PendingAccounting
        };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(refund.Id, cancellationToken)).Returns(refund);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundService.HasConfirmedPaymentAsync(refund, cancellationToken)).Returns(false);

        await Should.ThrowAsync<InvalidOperationException>(() => sut.ProcessInXeroAsync(refund.Id, cancellationToken));
    }
}
