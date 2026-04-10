using Api.Shared.Services.Models;
using Organization.Api.GraphQL.Organization;
using Organization.Api.Mappers;

namespace Organization.Api.UnitTests.GraphQL.Organization.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class OrganizationTypesShould
{
    [Fact]
    public void Return_All_Organization_Types()
    {
        var sut = new RootQuery(A.Fake<IMapper>());

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
