using Enterprise.Shared.Time;

namespace Enterprise.Shared.UnitTests.Time.DateTimeOffsetExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class FormattingShould
{
    private static readonly DateTimeOffset Sample = new(2024, 6, 15, 9, 5, 0, TimeSpan.Zero);

    [Fact]
    public void ToShortDate_formats_correctly() =>
        Sample.ToShortDate().ShouldBe("15 June 2024");

    [Fact]
    public void ToShortDateWithoutYear_omits_year() =>
        Sample.ToShortDateWithoutYear().ShouldBe("15 June");

    [Fact]
    public void ToShortTime_formats_correctly() =>
        Sample.ToShortTime().ShouldBe("09:05");
}
