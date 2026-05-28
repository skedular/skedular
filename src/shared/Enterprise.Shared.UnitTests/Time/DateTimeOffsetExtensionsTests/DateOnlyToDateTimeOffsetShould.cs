using Enterprise.Shared.Time;

namespace Enterprise.Shared.UnitTests.Time.DateTimeOffsetExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DateOnlyToDateTimeOffsetShould
{
    [Fact]
    public void Convert_date_and_timespan_to_DateTimeOffset()
    {
        var date = new DateOnly(2024, 6, 15);
        var time = new TimeSpan(0, 9, 30, 45, 500);

        var result = date.ToDateTimeOffset(time);

        result.Year.ShouldBe(2024);
        result.Month.ShouldBe(6);
        result.Day.ShouldBe(15);
        result.Hour.ShouldBe(9);
        result.Minute.ShouldBe(30);
        result.Second.ShouldBe(45);
        result.Offset.ShouldBe(TimeSpan.Zero);
    }
}
