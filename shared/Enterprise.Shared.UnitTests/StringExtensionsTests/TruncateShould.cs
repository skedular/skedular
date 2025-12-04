using Shouldly;

namespace Enterprise.Shared.UnitTests.StringExtensionsTests;

public class TruncateShould
{
    [Fact]
    public void TruncateAllWhenZeroSpecified() => "12345".Truncate(0).ShouldBe(string.Empty);

    [Fact]
    public void TruncateCorrectlyWithGivenParameter() => "12345".Truncate(3).ShouldBe("123");

    [Fact]
    public void ThrowExceptionIfParameterNegative()
    {
        var act = () => "12345".Truncate(-1);

        act.ShouldThrow<ArgumentOutOfRangeException>();
    }
}
