using Api.Shared.Services.Models;
using FakeItEasy;
using Organization.Api.GraphQL.Organization;
using Organization.Api.Mappers;

namespace Organization.Api.UnitTests.GraphQL.Organization.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class OrganizationXeroBillingModesShould
{
    [Fact]
    public void Return_All_Allowed_Billing_Modes()
    {
        var sut = new RootQuery(A.Fake<IMapper>());

        var result = sut.OrganizationXeroBillingModes().ToList();

        result.Count.ShouldBe(3);
        result.Select(item => item.Type).ShouldBe(
        [
            OrganizationXeroBillingMode.Disabled,
            OrganizationXeroBillingMode.Enabled,
            OrganizationXeroBillingMode.RepeatingInvoices
        ]);
        result.Select(item => item.Name).ShouldBe(
        [
            "Disabled",
            "Enabled",
            "Repeating Invoices"
        ]);
    }
}
