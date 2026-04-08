using Api.Shared.Services.Models;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using ProductEntity = Booking.Shared.Database.Entities.Product;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;

namespace Booking.Api.UnitTests.Services.MarketplaceRefundPreviewServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetByMarketplaceBookingSubscriptionIdAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Mapped_Preview_For_Subscription(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen] IMarketplaceRefundService marketplaceRefundService,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        MarketplaceRefundPreviewService sut,
        CancellationToken cancellationToken)
    {
        var subscription = CreateSubscription();
        var preview = new MarketplaceRefundPreview(
            "org-1",
            MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            "subscription-1",
            new DateTimeOffset(2026, 4, 7, 9, 0, 0, TimeSpan.Zero),
            subscription.NextRenewalAt!.Value,
            true,
            75,
            1440,
            80m,
            60m,
            CurrencyConstants.Nzd);

        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync("subscription-1", cancellationToken)).Returns(subscription);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundService.GetImmediateSubscriptionCancellationPreviewAsync(subscription, cancellationToken)).Returns(preview);

        var result = await sut.GetByMarketplaceBookingSubscriptionIdAsync("subscription-1", cancellationToken);

        result.LocalEntityType.ShouldBe(MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription);
        result.LocalEntityId.ShouldBe("subscription-1");
        result.RefundAmount.ShouldBe(60m);
    }

    private static MarketplaceBookingSubscriptionEntity CreateSubscription() =>
        new()
        {
            Id = "subscription-1",
            NextRenewalAt = new DateTimeOffset(2026, 4, 9, 9, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                Id = "marketplace-booking-1",
                ProductVersion = new ProductVersionEntity
                {
                    Product = new ProductEntity { Organization = new OrganizationEntity { Id = "org-1" } }
                }
            }
        };
}
