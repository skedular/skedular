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
public class GetByMarketplaceBookingIdAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Mapped_Refund_When_Marketplace_Booking_Refund_Exists(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IMarketplaceRefundEventRepository marketplaceRefundEventRepository,
        [Frozen] IGraphQlMapper graphQlMapper,
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
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "marketplace-booking-1"
        };
        var mappedRefund = new MarketplaceRefundDetails { Id = "refund-1" };

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundEventRepository).Returns(marketplaceRefundEventRepository);
        A.CallTo(() => marketplaceRefundRepository.GetLatestByLocalEntityAsync(
                MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
                "marketplace-booking-1",
                cancellationToken))
            .Returns(refund);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundEventRepository.GetByMarketplaceRefundIdAsync("refund-1", cancellationToken)).Returns([]);
        A.CallTo(() => graphQlMapper.MapTo(refund)).Returns(mappedRefund);
        A.CallTo(() => xeroRefundService.GetProcessingAvailabilityAsync(refund, cancellationToken))
            .Returns(new XeroRefundProcessingAvailability(true, null));

        var result = await sut.GetByMarketplaceBookingIdAsync("marketplace-booking-1", cancellationToken);

        result!.Id.ShouldBe(mappedRefund.Id);
        result!.CanProcessInXero.ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Redacted_Refund_When_Customer_Cannot_Modify_Payment_Method(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IGraphQlMapper graphQlMapper,
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
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "marketplace-booking-1"
        };
        var mappedRefund = new MarketplaceRefundDetails
        {
            Id = "refund-1",
            LastError = "provider error",
            RequestedByCustomerName = "Alice",
            Events = [new MarketplaceRefundEventDetails { Id = "event-1" }],
            CanProcessInXero = true,
            XeroProcessingBlockedReason = "blocked"
        };

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetLatestByLocalEntityAsync(
                MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
                "marketplace-booking-1",
                cancellationToken))
            .Returns(refund);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "customer-1", cancellationToken)).Returns(false);
        A.CallTo(() => graphQlMapper.MapTo(refund)).Returns(mappedRefund);

        var result = await sut.GetByMarketplaceBookingIdAsync("marketplace-booking-1", cancellationToken);

        result!.Id.ShouldBe(mappedRefund.Id);
        result!.Events.ShouldBeEmpty();
        result.RequestedByCustomerName.ShouldBeNull();
        result.LastError.ShouldBeNull();
        result.CanProcessInXero.ShouldBeFalse();
        result.XeroProcessingBlockedReason.ShouldBeNull();
        A.CallTo(() => xeroRefundService.GetProcessingAvailabilityAsync(A<MarketplaceRefund>._, cancellationToken)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Null_When_Marketplace_Booking_Refund_Does_Not_Exist(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IMarketplaceRefundEventRepository marketplaceRefundEventRepository,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IXeroRefundService xeroRefundService,
        MarketplaceRefundReadService sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundEventRepository).Returns(marketplaceRefundEventRepository);
        A.CallTo(() => marketplaceRefundRepository.GetLatestByLocalEntityAsync(
                MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
                "marketplace-booking-1",
                cancellationToken))
            .Returns((MarketplaceRefund?)null);

        var result = await sut.GetByMarketplaceBookingIdAsync("marketplace-booking-1", cancellationToken);

        result.ShouldBeNull();
        A.CallTo(() => graphQlMapper.MapTo(A<MarketplaceRefund>._)).MustNotHaveHappened();
        A.CallTo(() => xeroRefundService.GetProcessingAvailabilityAsync(A<MarketplaceRefund>._, cancellationToken)).MustNotHaveHappened();
    }
}
