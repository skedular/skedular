using Api.Shared.Services.Models;
using Location.Api.GraphQL.Location;
using Location.Api.Mappers;

namespace Location.Api.UnitTests.GraphQL.Location.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class LocationTypesShould
{
    [Fact]
    public void Return_All_Location_Types()
    {
        var sut = new RootQuery(A.Fake<IMapper>());

        var result = sut.LocationTypes().ToList();

        result.Count.ShouldBe(2);
        result.ShouldContain(item =>
            item.Type == LocationType.Private &&
            item.Name == LocationTypeConstants.Private.ToLocationTypeName());
        result.ShouldContain(item =>
            item.Type == LocationType.Marketplace &&
            item.Name == LocationTypeConstants.Marketplace.ToLocationTypeName());
    }
}
