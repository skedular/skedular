using Api.Shared.Services.Models;
using Organization.Api.GraphQL.Organization;

namespace Organization.Api.UnitTests.GraphQL.Organization.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class OrganizationTypesShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_All_Organization_Types(RootQuery sut)
    {
        var result = sut.OrganizationTypes().ToList();

        result.Count.ShouldBe(3);
        result.ShouldContain(item =>
            item.Type == OrganizationType.Private &&
            item.Name == OrganizationTypeConstants.Private.ToOrganizationTypeName());
        result.ShouldContain(item =>
            item.Type == OrganizationType.Marketplace &&
            item.Name == OrganizationTypeConstants.Marketplace.ToOrganizationTypeName());
        result.ShouldContain(item =>
            item.Type == OrganizationType.Individual &&
            item.Name == OrganizationTypeConstants.Individual.ToOrganizationTypeName());
    }
}
