using Api.Shared.Services.Models;
using Organization.Api.GraphQL.Organization;

namespace Organization.Api.UnitTests.GraphQL.Organization.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class OrganizationBillingCyclesShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_All_Billing_Cycles(RootQuery sut)
    {
        var result = sut.OrganizationBillingCycles().ToList();

        result.Count.ShouldBe(3);
        result.ShouldContain(item =>
            item.Type == OrganizationBillingCycle.Weekly &&
            item.Name == OrganizationBillingCycle.Weekly.ToOrganizationBillingCycleName());
        result.ShouldContain(item =>
            item.Type == OrganizationBillingCycle.Fortnightly &&
            item.Name == OrganizationBillingCycle.Fortnightly.ToOrganizationBillingCycleName());
        result.ShouldContain(item =>
            item.Type == OrganizationBillingCycle.Monthly &&
            item.Name == OrganizationBillingCycle.Monthly.ToOrganizationBillingCycleName());
    }
}
