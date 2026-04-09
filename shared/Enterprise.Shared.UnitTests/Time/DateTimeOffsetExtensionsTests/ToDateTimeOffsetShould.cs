using Enterprise.Shared.Time;

namespace Enterprise.Shared.UnitTests.Time.DateTimeOffsetExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ToDateTimeOffsetShould
{
    [Fact]
    public void Convert_DateTime_to_DateTimeOffset_with_zero_offset()
    {
        var dt = new DateTime(2024, 6, 15, 10, 30, 0);

        var result = dt.ToDateTimeOffset();

        result.DateTime.ShouldBe(dt);
        result.Offset.ShouldBe(TimeSpan.Zero);
    }
}
