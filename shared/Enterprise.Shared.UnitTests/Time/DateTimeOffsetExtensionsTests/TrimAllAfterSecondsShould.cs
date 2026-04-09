using Enterprise.Shared.Time;

namespace Enterprise.Shared.UnitTests.Time.DateTimeOffsetExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class TrimAllAfterSecondsShould
{
    [Fact]
    public void Remove_milliseconds_and_below()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 10, 30, 45, 500, TimeSpan.Zero);

        var result = dto.TrimAllAfterSeconds();

        result.Second.ShouldBe(45);
        result.Millisecond.ShouldBe(0);
    }

    [Fact]
    public void Preserve_offset()
    {
        var offset = TimeSpan.FromHours(5);
        var dto = new DateTimeOffset(2024, 6, 15, 10, 30, 45, 123, offset);

        var result = dto.TrimAllAfterSeconds();

        result.Offset.ShouldBe(offset);
    }
}
