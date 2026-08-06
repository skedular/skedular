using Api.Shared.Services.Models;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using ProductEntity = Booking.Shared.Database.Entities.Product;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;

namespace Booking.Api.UnitTests.Services.MarketplaceRefundPreviewServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetByBookingIdAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Mapped_Preview_When_Customer_Can_Modify_Payment_Method(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IMarketplaceRefundService marketplaceRefundService,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        MarketplaceRefundPreviewService sut,
        CancellationToken cancellationToken)
    {
        var booking = CreateBooking();
        var preview = new MarketplaceRefundPreview(
            "org-1",
            MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            "marketplace-booking-1",
            new DateTimeOffset(2026, 4, 7, 9, 0, 0, TimeSpan.Zero),
            booking.From,
            true,
            50,
            180,
            120m,
            60m,
            CurrencyConstants.Nzd);

        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => bookingRepository.GetByIdAsync("booking-1", cancellationToken)).Returns(booking);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => marketplaceRefundService.GetBookingCancellationPreviewAsync(booking, cancellationToken)).Returns(preview);

        var result = await sut.GetByBookingIdAsync("booking-1", cancellationToken);

        result.LocalEntityType.ShouldBe(MarketplaceRefundEntityType.MarketplaceBooking);
        result.LocalEntityId.ShouldBe("marketplace-booking-1");
        result.IsRefundable.ShouldBeTrue();
        result.RefundAmount.ShouldBe(60m);
        result.Currency.ShouldBe(Currency.Nzd);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_Customer_Cannot_Modify_Payment_Method(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        MarketplaceRefundPreviewService sut,
        CancellationToken cancellationToken)
    {
        var booking = CreateBooking();

        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => bookingRepository.GetByIdAsync("booking-1", cancellationToken)).Returns(booking);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "customer-1", cancellationToken)).Returns(false);

        await Should.ThrowAsync<UnauthorizedAccessException>(() => sut.GetByBookingIdAsync("booking-1", cancellationToken));
    }

    private static Shared.Database.Entities.Booking CreateBooking() =>
        new()
        {
            Id = "booking-1",
            From = new DateTimeOffset(2026, 4, 7, 15, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                Id = "marketplace-booking-1",
                ProductVersion = new ProductVersionEntity
                {
                    Product = new ProductEntity
                    {
                        Organization = new OrganizationEntity
                        {
                            Id = "org-1",
                        },
                    },
                },
            },
        };
}
