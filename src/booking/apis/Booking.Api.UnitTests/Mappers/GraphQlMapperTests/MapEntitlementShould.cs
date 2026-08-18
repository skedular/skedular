using Api.Shared.Services.Models;
using Booking.Api.Mappers;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Models.Entitlements;
using EntitlementPurchase = Booking.Shared.Database.Entities.EntitlementPurchase;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;
using Organization = Booking.Shared.Database.Entities.Organization;

namespace Booking.Api.UnitTests.Mappers.GraphQlMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapEntitlementShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void PreservePricingAndDistinctLinkedBookingIds(IEntityMapper entityMapper, string entitlementId, string pricingId, string firstBookingId,
        string secondBookingId)
    {
        var sut = new GraphQlMapper(entityMapper);
        var source = new Entitlement
        {
            Id = entitlementId,
            Organization = new Organization
            {
                CustomDomain = "test",
            },
            PricingId = pricingId,
            MarketplaceBookings =
            [
                new MarketplaceBooking
                {
                    BookingId = firstBookingId,
                },
                new MarketplaceBooking
                {
                    BookingId = firstBookingId,
                },
                new MarketplaceBooking
                {
                    BookingId = null,
                },
                new MarketplaceBooking
                {
                    BookingId = secondBookingId,
                },
            ],
        };

        var result = sut.MapTo(source);

        result.PricingId.ShouldBe(pricingId);
        result.LinkedBookingIds.ShouldBe([firstBookingId, secondBookingId]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void MapPurchasePricingRestrictions(IEntityMapper entityMapper, string productVersionId, string pricingId)
    {
        var sut = new GraphQlMapper(entityMapper);
        var source = new Entitlement
        {
            Organization = new Organization
            {
                CustomDomain = "test",
            },
            EntitlementPurchase = new EntitlementPurchase
            {
                ProductVersionId = productVersionId,
                ProductPricing = ProductPricing.Empty(pricingId) with
                {
                    AvailableDays = [DayOfWeek.Monday, DayOfWeek.Friday],
                    MinDurationMinutes = 30,
                    MaxDurationMinutes = 120,
                    NumberOfResourcesToBook = 2,
                },
            },
        };

        var result = sut.MapTo(source);

        result.Restrictions.ShouldNotBeNull();
        result.Restrictions.ProductVersionId.ShouldBe(productVersionId);
        result.Restrictions.AvailableDays.ShouldBe([DayOfWeek.Monday, DayOfWeek.Friday]);
        result.Restrictions.MinDurationMinutes.ShouldBe(30);
        result.Restrictions.MaxDurationMinutes.ShouldBe(120);
        result.Restrictions.NumberOfResourcesToBook.ShouldBe(2);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void PreserveRefundAndLinkedBookingsFromReadModel(IEntityMapper entityMapper, string entitlementId, string refundId,
        string bookingId)
    {
        var sut = new GraphQlMapper(entityMapper);
        var source = new EntitlementModel
        {
            Id = entitlementId,
            Refund = new EntitlementRefundModel
            {
                Id = refundId,
                Amount = 12.50m,
                UnusedCreditQuantity = 2,
                Status = MarketplaceRefundStatus.Completed,
                PaymentRefundStatus = "completed",
            },
            LinkedBookingIds = [bookingId],
        };

        var result = sut.MapTo(source);

        result.Refund.ShouldNotBeNull();
        result.Refund.Id.ShouldBe(refundId);
        result.Refund.Amount.ShouldBe(12.50m);
        result.LinkedBookingIds.ShouldBe([bookingId]);
    }
}
