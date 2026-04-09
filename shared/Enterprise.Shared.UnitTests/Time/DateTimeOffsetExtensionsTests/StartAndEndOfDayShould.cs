using Enterprise.Shared.Time;

namespace Enterprise.Shared.UnitTests.Time.DateTimeOffsetExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartAndEndOfDayShould
{
    private static readonly DateTimeOffset Sample = new(2024, 6, 15, 14, 30, 0, TimeSpan.FromHours(2));

    [Fact]
    public void StartOfDay_returns_midnight()
    {
        var result = Sample.StartOfDay();

        result.Hour.ShouldBe(0);
        result.Minute.ShouldBe(0);
        result.Second.ShouldBe(0);
        result.Day.ShouldBe(15);
        result.Offset.ShouldBe(TimeSpan.FromHours(2));
    }

    [Fact]
    public void EndOfDay_returns_start_of_next_day()
    {
        var result = Sample.EndOfDay();

        result.Day.ShouldBe(16);
        result.Hour.ShouldBe(0);
        result.Minute.ShouldBe(0);
        result.Second.ShouldBe(0);
    }

    [Fact]
    public void EndOfYesterday_returns_last_tick_of_previous_day()
    {
        var result = Sample.EndOfYesterday();

        result.Day.ShouldBe(14);
        result.Hour.ShouldBe(23);
        result.Minute.ShouldBe(59);
    }

    [Fact]
    public void ToDate_removes_time_component()
    {
        var result = Sample.ToDate();

        result.Hour.ShouldBe(0);
        result.Minute.ShouldBe(0);
        result.Second.ShouldBe(0);
        result.Day.ShouldBe(Sample.Day);
    }

    [Fact]
    public void ToDate_with_custom_offset()
    {
        var customOffset = TimeSpan.FromHours(-3);
        var result = Sample.ToDate(customOffset);

        result.Offset.ShouldBe(customOffset);
        result.Hour.ShouldBe(0);
    }

    [Fact]
    public void ToDateTime_returns_inner_DateTime()
    {
        var result = Sample.ToDateTime();

        result.ShouldBe(Sample.DateTime);
    }
}
