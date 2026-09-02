using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UtcCalendarWeekTests
{
    [Theory]
    [InlineData("2026-09-02T12:00:00Z", "2026-08-31T00:00:00Z")]
    [InlineData("2026-08-30T23:59:59Z", "2026-08-24T00:00:00Z")]
    public void Starts_On_Monday_At_UTC(string value, string expected) =>
        UtcCalendarWeek.Start(DateTimeOffset.Parse(value)).ShouldBe(DateTimeOffset.Parse(expected));

    [Fact]
    public void Exempts_A_Partial_Boundary_Week() =>
        UtcCalendarWeek.IsComplete(
                DateTimeOffset.Parse("2026-08-31T00:00:00Z"),
                DateTimeOffset.Parse("2026-09-02T00:00:00Z"),
                DateTimeOffset.Parse("2026-09-14T00:00:00Z"))
            .ShouldBeFalse();

    [Fact]
    public void Accepts_A_Fully_Covered_UTC_Week() =>
        UtcCalendarWeek.IsComplete(
                DateTimeOffset.Parse("2026-08-31T00:00:00Z"),
                DateTimeOffset.Parse("2026-08-31T00:00:00Z"),
                DateTimeOffset.Parse("2026-09-14T00:00:00Z"))
            .ShouldBeTrue();
}
