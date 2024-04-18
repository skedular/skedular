using Enterprise.Shared.GraphQL;
using FluentAssertions;
using Testing.Shared;
using Xunit;

namespace Enterprise.Shared.UnitTests.GraphQL.CustomNamingConventionTests;

public class GetEnumValueNameShould
{
    // ReSharper disable InconsistentNaming
    public enum TestNames
    {
        Something,
        Maybe_This,
        My123,
        lowerUPPER
    }

    [Theory]
    [InlineData(TestNames.Something, "Something")]
    [InlineData(TestNames.My123, "My123")]
    [InlineData(TestNames.Maybe_This, "Maybe_This")]
    [InlineData(TestNames.lowerUPPER, "lowerUPPER")]
    public void Output_The_Correct_Name(TestNames name, string expected)
    {
        var customNamingConventions = new CustomNamingConventions();
        customNamingConventions.GetEnumValueName(name).Should().Be(expected);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Throw_ArgumentException_When_Value_Is_Null(CustomNamingConventions sut)
    {
        var act = new Action(() => { sut.GetEnumValueName(null!); });

        act.Should().Throw<ArgumentException>();
    }
}
