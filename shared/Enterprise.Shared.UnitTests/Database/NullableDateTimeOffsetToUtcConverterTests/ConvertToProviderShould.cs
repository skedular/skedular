using Enterprise.Shared.Database;
using FluentAssertions;
using FluentAssertions.Execution;
using Testing.Shared;
using Xunit;

namespace Enterprise.Shared.UnitTests.Database.NullableDateTimeOffsetToUtcConverterTests;

public class ConvertToProviderShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Not_Change_When_Offset_Is_Zero(NullableDateTimeOffsetToUtcConverter sut)
    {
        DateTimeOffset? input = new DateTimeOffset(2022, 10, 09,
            10, 9, 8,
            TimeSpan.Zero);
        var value = sut;
        var func = value.ConvertToProviderExpression.Compile();
        var output = func(input);

        using (new AssertionScope())
        {
            output.Should().Be(input);
            output.Value.Offset.Should().Be(TimeSpan.Zero);
        }
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Set_Offset_To_Zero(NullableDateTimeOffsetToUtcConverter sut)
    {
        DateTimeOffset? input = new DateTimeOffset(2022, 10, 09,
            10, 9, 8,
            TimeSpan.FromHours(2.5));
        var value = sut;
        var func = value.ConvertToProviderExpression.Compile();
        var output = func(input);

        using (new AssertionScope())
        {
            output.Should().Be(input);
            output.Value.Offset.Should().Be(TimeSpan.Zero);
        }
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Handle_Null(NullableDateTimeOffsetToUtcConverter sut)
    {
        var value = sut;
        var func = value.ConvertToProviderExpression.Compile();
        var output = func(null);

        output.Should().BeNull();
    }
}
