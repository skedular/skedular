using Enterprise.Shared.Time;

namespace Enterprise.Shared.UnitTests.Time.DateTimeOffsetExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartOfWeekShould
{
    [Theory]
    [InlineData(DayOfWeek.Monday)]
    [InlineData(DayOfWeek.Sunday)]
    public void Return_start_of_week_for_given_start_day(DayOfWeek startDay)
    {
        // Use a known Wednesday: 2024-06-19
        var wednesday = new DateTimeOffset(2024, 6, 19, 14, 0, 0, TimeSpan.Zero);

        var result = wednesday.StartOfWeek(startDay);

        result.DayOfWeek.ShouldBe(startDay);
        result.Hour.ShouldBe(0);
    }

    [Fact]
    public void Default_start_is_monday()
    {
        var wednesday = new DateTimeOffset(2024, 6, 19, 14, 0, 0, TimeSpan.Zero);

        var result = wednesday.StartOfWeek();

        result.DayOfWeek.ShouldBe(DayOfWeek.Monday);
    }
}
