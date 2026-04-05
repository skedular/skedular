using Api.Shared.Services.Models;

namespace Api.Shared.Services.UnitTests.Models.OrganizationXeroBillingModeExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ToOrganizationXeroBillingModeShould
{
    [Theory]
    [InlineData(OrganizationXeroBillingMode.Disabled, XeroBillingModeConstants.Disabled)]
    [InlineData(OrganizationXeroBillingMode.Enabled, XeroBillingModeConstants.Enabled)]
    [InlineData(OrganizationXeroBillingMode.RepeatingInvoices, XeroBillingModeConstants.RepeatingInvoices)]
    public void Convert_Enum_To_String_Constant(
        OrganizationXeroBillingMode billingMode,
        string expectedBillingMode) =>
        billingMode
            .ToOrganizationXeroBillingMode()
            .ShouldBe(expectedBillingMode);

    [Theory]
    [InlineData(XeroBillingModeConstants.Disabled, OrganizationXeroBillingMode.Disabled)]
    [InlineData(XeroBillingModeConstants.Enabled, OrganizationXeroBillingMode.Enabled)]
    [InlineData(XeroBillingModeConstants.RepeatingInvoices, OrganizationXeroBillingMode.RepeatingInvoices)]
    public void Convert_String_Constant_To_Enum(
        string billingMode,
        OrganizationXeroBillingMode expectedBillingMode) =>
        billingMode
            .ToOrganizationXeroBillingMode()
            .ShouldBe(expectedBillingMode);

    [Fact]
    public void Throw_When_Enum_Value_Is_Unsupported() =>
        Should.Throw<ArgumentOutOfRangeException>(() => ((OrganizationXeroBillingMode)999).ToOrganizationXeroBillingMode());

    [Fact]
    public void Throw_When_String_Constant_Is_Unsupported() =>
        Should.Throw<ArgumentOutOfRangeException>(() => "UNSUPPORTED".ToOrganizationXeroBillingMode());
}
