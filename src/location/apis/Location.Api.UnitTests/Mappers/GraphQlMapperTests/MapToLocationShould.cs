using Api.Shared.Services.Models;
using Location.Api.Mappers;
using Location.Shared.Models;

namespace Location.Api.UnitTests.Mappers.GraphQlMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapToLocationShould
{
    [Fact]
    public void Map_Floor_Plan_Count()
    {
        var sut = new GraphQlMapper();
        var location = new Shared.Models.Location
        {
            Id = "location-1",
            Name = "HQ",
            Type = LocationType.Private,
            Organization = new Organization
            {
                Id = "organization-1",
                CustomDomain = "acme",
            },
            FloorPlans =
            [
                new FloorPlan
                {
                    Id = "floor-plan-1",
                },
                new FloorPlan
                {
                    Id = "floor-plan-2",
                },
            ],
        };

        var result = sut.MapTo(location);

        result.ShouldNotBeNull();
        result.FloorPlanCount.ShouldBe(2);
    }
}
