using Api.Shared.Services.Models;
using Organization.Api.GraphQL.Organization;

namespace Organization.Api.UnitTests.GraphQL.Organization.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class OrganizationXeroBillingModesShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_All_Allowed_Billing_Modes(RootQuery sut)
    {
        var result = sut.OrganizationXeroBillingModes().ToList();

        result.Count.ShouldBe(3);
        result.Select(item => item.Type).ShouldBe(
        [
            OrganizationXeroBillingMode.Disabled,
            OrganizationXeroBillingMode.Enabled,
            OrganizationXeroBillingMode.RepeatingInvoices,
        ]);
        result.Select(item => item.Name).ShouldBe(
        [
            "Disabled",
            "Enabled",
            "Repeating Invoices",
        ]);
    }
}
