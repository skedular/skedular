using Api.Shared.Services.Models;

namespace Api.Shared.Services.UnitTests.Models.DurationDisplayUnitTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ToDurationDisplayUnitShould
{
    [Theory]
    [InlineData(DurationDisplayUnit.Minutes, DurationDisplayUnitConstants.Minutes)]
    [InlineData(DurationDisplayUnit.Hours, DurationDisplayUnitConstants.Hours)]
    public void Convert_Enum_To_String_Constant(DurationDisplayUnit unit, string expected) =>
        unit.ToDurationDisplayUnit().ShouldBe(expected);

    [Theory]
    [InlineData(DurationDisplayUnitConstants.Minutes, DurationDisplayUnit.Minutes)]
    [InlineData(DurationDisplayUnitConstants.Hours, DurationDisplayUnit.Hours)]
    public void Convert_String_Constant_To_Enum(string value, DurationDisplayUnit expected) =>
        value.ToDurationDisplayUnit().ShouldBe(expected);

    [Fact]
    public void Default_Null_String_To_Hours() =>
        ((string?)null).ToDurationDisplayUnit().ShouldBe(DurationDisplayUnit.Hours);

    [Fact]
    public void Throw_When_String_Is_Empty() =>
        Should.Throw<ArgumentOutOfRangeException>(() => string.Empty.ToDurationDisplayUnit());

    [Fact]
    public void Throw_When_String_Constant_Is_Unsupported() =>
        Should.Throw<ArgumentOutOfRangeException>(() => "UNSUPPORTED".ToDurationDisplayUnit());
}
