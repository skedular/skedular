using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public sealed class ValidateBookingDurationShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Accept_any_duration_within_inclusive_minimum_and_maximum(
        DateTimeOffset from)
    {
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            MinDurationMinutes = 30,
            MaxDurationMinutes = 120,
        };

        MarketplaceBookingService.ValidateBookingDuration(from, from.AddMinutes(30), pricing);
        MarketplaceBookingService.ValidateBookingDuration(from, from.AddMinutes(120), pricing);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Reject_duration_shorter_than_minimum(DateTimeOffset from)
    {
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            MinDurationMinutes = 30,
        };

        Should.Throw<MarketplaceBookingDurationMustBeAtLeastMinimum>(() =>
            MarketplaceBookingService.ValidateBookingDuration(from, from.AddMinutes(29), pricing));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Reject_duration_longer_than_maximum(DateTimeOffset from)
    {
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            MaxDurationMinutes = 120,
        };

        Should.Throw<MarketplaceBookingDurationMustNotExceedMaximum>(() =>
            MarketplaceBookingService.ValidateBookingDuration(from, from.AddMinutes(121), pricing));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Reject_equal_or_reversed_times(DateTimeOffset from)
    {
        var pricing = ProductPricing.Empty("pricing-1");

        Should.Throw<InvalidOperationException>(() =>
            MarketplaceBookingService.ValidateBookingDuration(from, from, pricing));
        Should.Throw<InvalidOperationException>(() =>
            MarketplaceBookingService.ValidateBookingDuration(from, from.AddMinutes(-1), pricing));
    }

    [Fact]
    public void Calculate_duration_from_absolute_offsets_across_daylight_saving_change()
    {
        var pricing = ProductPricing.Empty("pricing-1") with
        {
            MinDurationMinutes = 120,
            MaxDurationMinutes = 120,
        };
        var from = new DateTimeOffset(2026, 4, 5, 1, 30, 0, TimeSpan.FromHours(13));
        var until = new DateTimeOffset(2026, 4, 5, 2, 30, 0, TimeSpan.FromHours(12));

        MarketplaceBookingService.ValidateBookingDuration(from, until, pricing);
    }
}
