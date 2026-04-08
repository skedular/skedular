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
public class GetByIdAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Mapped_Refund_When_Id_Exists(
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
        var refund = new MarketplaceRefund { Id = "refund-1", OrganizationId = "org-1" };
        var refundEvent = new MarketplaceRefundEvent { Id = "refund-event-1", MarketplaceRefundId = "refund-1" };
        var mappedRefund = new MarketplaceRefundDetails { Id = "refund-1" };
        var mappedRefundEvent = new MarketplaceRefundEventDetails { Id = "refund-event-1" };

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundEventRepository).Returns(marketplaceRefundEventRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync("refund-1", cancellationToken)).Returns(refund);
        A.CallTo(() => marketplaceRefundEventRepository.GetByMarketplaceRefundIdAsync("refund-1", cancellationToken)).Returns([refundEvent]);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => mapper.MapTo(refund)).Returns(mappedRefund);
        A.CallTo(() => mapper.MapTo(refundEvent)).Returns(mappedRefundEvent);
        A.CallTo(() => xeroRefundService.GetProcessingAvailabilityAsync(refund, cancellationToken))
            .Returns(new XeroRefundProcessingAvailability(true, null));

        var result = await sut.GetByIdAsync("refund-1", cancellationToken);

        result.ShouldBe(mappedRefund);
        result!.CanProcessInXero.ShouldBeTrue();
        result.XeroProcessingBlockedReason.ShouldBeNull();
        result.Events.ShouldHaveSingleItem();
        result.Events.Single().ShouldBe(mappedRefundEvent);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_Customer_Cannot_Modify_Payment_Method(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        MarketplaceRefundReadService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund { Id = "refund-1", OrganizationId = "org-1" };

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync("refund-1", cancellationToken)).Returns(refund);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "customer-1", cancellationToken)).Returns(false);

        await Should.ThrowAsync<UnauthorizedAccessException>(() => sut.GetByIdAsync("refund-1", cancellationToken));
    }
}
