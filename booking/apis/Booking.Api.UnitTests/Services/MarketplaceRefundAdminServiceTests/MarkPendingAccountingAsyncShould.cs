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
public class MarkPendingAccountingAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Update_Refund_Status_And_Raise_Booking_Graphql_Change(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
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
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "booking-1",
            Status = MarketplaceRefundStatusConstants.Requested,
            RefundAmount = 60m
        };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(refund.Id, cancellationToken)).Returns(refund);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        var result = await sut.MarkPendingAccountingAsync(refund.Id, 40m, "Queued in Xero", cancellationToken);

        result.ShouldBe(refund);
        refund.Status.ShouldBe(MarketplaceRefundStatusConstants.PendingAccounting);
        refund.RefundAmount.ShouldBe(40m);
        refund.Reason.ShouldBe("Queued in Xero");
        refund.LastProcessedAt.ShouldNotBeNull();
        A.CallTo(() => marketplaceRefundEventService.Add(refund, MarketplaceRefundEventTypeConstants.PendingAccounting, "customer-1",
                A<DateTimeOffset?>.That.Matches(value => value.HasValue)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.BookingTopicName,
                "booking-1",
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceRefundNotificationService.NotifyStatusChangedAsync(refund, cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_Customer_Cannot_Modify_Payment_Method(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMarketplaceRefundEventService marketplaceRefundEventService,
        MarketplaceRefundAdminService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "booking-1",
            Status = MarketplaceRefundStatusConstants.Requested,
            RefundAmount = 60m
        };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(refund.Id, cancellationToken)).Returns(refund);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "customer-1", cancellationToken)).Returns(false);

        await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            sut.MarkPendingAccountingAsync(refund.Id, null, null, cancellationToken));
        A.CallTo(() => marketplaceRefundEventService.Add(A<MarketplaceRefund>._, A<string>._, A<string?>._, A<DateTimeOffset?>._))
            .MustNotHaveHappened();
    }
}
