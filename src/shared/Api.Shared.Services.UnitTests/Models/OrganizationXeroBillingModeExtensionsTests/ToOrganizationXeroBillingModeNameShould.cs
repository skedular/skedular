using Api.Shared.Services.Models;

namespace Api.Shared.Services.UnitTests.Models.OrganizationXeroBillingModeExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ToOrganizationXeroBillingModeNameShould
{
    [Theory]
    [InlineData(OrganizationXeroBillingMode.Disabled, "Disabled")]
    [InlineData(OrganizationXeroBillingMode.Enabled, "Enabled")]
    [InlineData(OrganizationXeroBillingMode.RepeatingInvoices, "Repeating Invoices")]
    public void Return_Display_Name_For_Billing_Mode(
        OrganizationXeroBillingMode billingMode,
        string expectedBillingModeName) =>
        billingMode
            .ToOrganizationXeroBillingModeName()
            .ShouldBe(expectedBillingModeName);

    [Fact]
    public void Throw_When_Enum_Value_Is_Unsupported() =>
        Should.Throw<ArgumentOutOfRangeException>(() => ((OrganizationXeroBillingMode)999).ToOrganizationXeroBillingModeName());
}
