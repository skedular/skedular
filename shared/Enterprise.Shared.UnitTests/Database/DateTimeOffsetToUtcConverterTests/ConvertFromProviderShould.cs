using Enterprise.Shared.Database;
using FluentAssertions;
using FluentAssertions.Execution;
using Testing.Shared;
using Xunit;

namespace Enterprise.Shared.UnitTests.Database.DateTimeOffsetToUtcConverterTests;

public class ConvertFromProviderShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Not_Change_When_Offset_Is_Zero(DateTimeOffsetToUtcConverter sut)
    {
        var input = new DateTimeOffset(2022, 10, 09,
            10, 9, 8,
            TimeSpan.Zero);
        var value = sut;
        var func = value.ConvertFromProviderExpression.Compile();
        var output = func(input);

        using (new AssertionScope())
        {
            output.Should().Be(input);
            output.Offset.Should().Be(TimeSpan.Zero);
        }
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Not_Change_When_Offset_Is_Not_Zero(DateTimeOffsetToUtcConverter sut)
    {
        var fromHours = TimeSpan.FromHours(2.5);

        var input = new DateTimeOffset(2022, 10, 09,
            10, 9, 8,
            fromHours);
        var value = sut;
        var func = value.ConvertFromProviderExpression.Compile();
        var output = func(input);

        using (new AssertionScope())
        {
            output.Should().Be(input);
            output.Offset.Should().Be(fromHours);
        }
    }
}
