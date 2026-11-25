using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;

namespace Booking.Api.UnitTests.Services.MarketplaceRefundReadServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetByMarketplaceBookingSubscriptionIdsAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Only_Existing_Refunds_From_One_Batch_Read(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IMarketplaceRefundEventRepository marketplaceRefundEventRepository,
        [Frozen]
        IXeroRefundService xeroRefundService,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        MarketplaceRefundReadService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            LocalEntityId = "subscription-1",
            Status = MarketplaceRefundStatusConstants.Requested,
        };
        string[] subscriptionIds = ["subscription-1", "subscription-2"];

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundEventRepository).Returns(marketplaceRefundEventRepository);
        A.CallTo(() => marketplaceRefundRepository.GetLatestByLocalEntitiesAsync(
                MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
                subscriptionIds,
                cancellationToken))
            .Returns([refund]);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodByOrganizationIdsAsync(
                A<IReadOnlyCollection<string>>._,
                "customer-1",
                cancellationToken))
            .Returns(new Dictionary<string, bool>
            {
                ["org-1"] = true,
            });
        A.CallTo(() => marketplaceRefundEventRepository.GetByMarketplaceRefundIdsAsync(
                A<IReadOnlyList<string>>._,
                cancellationToken))
            .Returns([]);
        A.CallTo(() => marketplaceRefundRepository.GetCustomerNamesByRefundsAsync(
                A<IReadOnlyList<MarketplaceRefund>>._,
                cancellationToken))
            .Returns(new Dictionary<string, string>());
        A.CallTo(() => xeroRefundService.GetProcessingAvailabilitiesAsync(
                A<IReadOnlyList<MarketplaceRefund>>._,
                cancellationToken))
            .Returns(new Dictionary<string, XeroRefundProcessingAvailability>
            {
                [refund.Id] = new(false, "Unavailable"),
            });

        var result = await sut.GetByMarketplaceBookingSubscriptionIdsAsync(subscriptionIds, cancellationToken);

        result.Keys.ShouldBe(["subscription-1"]);
        result["subscription-1"].Id.ShouldBe("refund-1");
        A.CallTo(() => marketplaceRefundRepository.GetLatestByLocalEntitiesAsync(
                MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
                subscriptionIds,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceRefundRepository.GetLatestByLocalEntityAsync(
                A<string>._,
                A<string>._,
                cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => xeroRefundService.GetProcessingAvailabilitiesAsync(
                A<IReadOnlyList<MarketplaceRefund>>._,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Redact_Refunds_From_Unauthorized_Organizations(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IMarketplaceRefundEventRepository marketplaceRefundEventRepository,
        [Frozen]
        IXeroRefundService xeroRefundService,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        MarketplaceRefundReadService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-unauthorized",
            OrganizationId = "org-hidden",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            LocalEntityId = "subscription-hidden",
            Status = MarketplaceRefundStatusConstants.Completed,
            LastError = "sensitive-error",
            RequestedByCustomerId = "actor-1",
        };

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundEventRepository).Returns(marketplaceRefundEventRepository);
        A.CallTo(() => marketplaceRefundRepository.GetLatestByLocalEntitiesAsync(
                MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns([refund]);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodByOrganizationIdsAsync(
                A<IReadOnlyCollection<string>>._,
                "customer-1",
                cancellationToken))
            .Returns(new Dictionary<string, bool>
            {
                ["org-hidden"] = false,
            });

        var result = await sut.GetByMarketplaceBookingSubscriptionIdsAsync([refund.LocalEntityId], cancellationToken);

        result[refund.LocalEntityId].LastError.ShouldBeNull();
        result[refund.LocalEntityId].RequestedByCustomerName.ShouldBeNull();
        result[refund.LocalEntityId].Events.ShouldBeEmpty();
        result[refund.LocalEntityId].CanProcessInXero.ShouldBeFalse();
        A.CallTo(() => marketplaceRefundEventRepository.GetByMarketplaceRefundIdsAsync(
                A<IReadOnlyList<string>>._,
                cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => xeroRefundService.GetProcessingAvailabilitiesAsync(
                A<IReadOnlyList<MarketplaceRefund>>._,
                cancellationToken))
            .MustNotHaveHappened();
    }
}
