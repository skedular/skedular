using Enterprise.Shared.Time;

namespace Enterprise.Shared.UnitTests.Time.DateTimeOffsetExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ToTimezoneInfoShould
{
    [Fact]
    public void Return_utc_when_null() =>
        ((string?)null).ToTimezoneInfo().ShouldBe(TimeZoneInfo.Utc);

    [Fact]
    public void Return_utc_when_whitespace() =>
        "   ".ToTimezoneInfo().ShouldBe(TimeZoneInfo.Utc);

    [Fact]
    public void Return_utc_when_invalid_timezone() =>
        "NotARealTimezone/Invalid".ToTimezoneInfo().ShouldBe(TimeZoneInfo.Utc);

    [Fact]
    public void Return_correct_timezone_for_utc() =>
        "UTC".ToTimezoneInfo().ShouldBe(TimeZoneInfo.Utc);
}
