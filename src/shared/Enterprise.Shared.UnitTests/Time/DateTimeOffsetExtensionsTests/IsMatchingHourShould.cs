using Enterprise.Shared.Time;

namespace Enterprise.Shared.UnitTests.Time.DateTimeOffsetExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class IsMatchingHourShould
{
    [Fact]
    public void Return_true_when_hour_matches_in_timezone()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 10, 0, 0, TimeSpan.Zero);

        dto.IsMatchingHour("UTC", 10).ShouldBeTrue();
    }

    [Fact]
    public void Return_false_when_hour_does_not_match()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 10, 0, 0, TimeSpan.Zero);

        dto.IsMatchingHour("UTC", 11).ShouldBeFalse();
    }

    [Fact]
    public void Use_utc_when_timezone_is_null()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 10, 0, 0, TimeSpan.Zero);

        dto.IsMatchingHour((string?)null, 10).ShouldBeTrue();
    }

    [Fact]
    public void Use_utc_when_timezone_is_invalid()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 10, 0, 0, TimeSpan.Zero);

        dto.IsMatchingHour("NotATimezone/Invalid", 10).ShouldBeTrue();
    }

    [Fact]
    public void Return_true_when_hour_matches_via_TimeZoneInfo()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 10, 0, 0, TimeSpan.Zero);

        dto.IsMatchingHour(TimeZoneInfo.Utc, 10).ShouldBeTrue();
    }
}
