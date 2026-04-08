using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;

namespace Booking.Api.UnitTests.Services.MarketplaceRefundReadServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetByMarketplaceBookingSubscriptionIdAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Mapped_Refund_When_Subscription_Refund_Exists(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IMarketplaceRefundEventRepository marketplaceRefundEventRepository,
        [Frozen] IMapper mapper,
        [Frozen] IXeroRefundService xeroRefundService,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        MarketplaceRefundReadService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            LocalEntityId = "subscription-1"
        };
        var mappedRefund = new MarketplaceRefundDetails { Id = "refund-1" };

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundEventRepository).Returns(marketplaceRefundEventRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByLocalEntityAsync(
                MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
                "subscription-1",
                cancellationToken))
            .Returns(refund);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundEventRepository.GetByMarketplaceRefundIdAsync("refund-1", cancellationToken)).Returns([]);
        A.CallTo(() => mapper.MapTo(refund)).Returns(mappedRefund);
        A.CallTo(() => xeroRefundService.GetProcessingAvailabilityAsync(refund, cancellationToken))
            .Returns(new XeroRefundProcessingAvailability(false, "Concrete invoice instance has not been correlated yet."));

        var result = await sut.GetByMarketplaceBookingSubscriptionIdAsync("subscription-1", cancellationToken);

        result.ShouldBe(mappedRefund);
        result!.CanProcessInXero.ShouldBeFalse();
        result.XeroProcessingBlockedReason.ShouldBe("Concrete invoice instance has not been correlated yet.");
    }
}
