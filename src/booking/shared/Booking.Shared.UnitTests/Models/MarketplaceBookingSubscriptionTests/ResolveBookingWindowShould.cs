using Booking.Shared.Models;

namespace Booking.Shared.UnitTests.Models.MarketplaceBookingSubscriptionTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public sealed class ResolveBookingWindowShould
{
    [Fact]
    public void Preserve_equal_or_reversed_times_for_duration_validation()
    {
        var termStart = new DateTimeOffset(2026, 9, 2, 0, 0, 0, TimeSpan.Zero);

        var (equalFrom, equalUntil) = MarketplaceBookingSubscription.ResolveBookingWindow(
            termStart,
            new TimeOnly(9, 0),
            new TimeOnly(9, 0));
        var (reversedFrom, reversedUntil) = MarketplaceBookingSubscription.ResolveBookingWindow(
            termStart,
            new TimeOnly(10, 0),
            new TimeOnly(9, 0));

        equalUntil.ShouldBe(equalFrom);
        reversedUntil.ShouldBeLessThan(reversedFrom);
    }
}
