using Api.Shared.Services.Models;
using Customer.Shared.Mappers;
using CustomerEntity = Customer.Shared.Database.Entities.Customer;
using LocationEntity = Customer.Shared.Database.Entities.Location;
using ResourceEntity = Customer.Shared.Database.Entities.Resource;

namespace Customer.Shared.UnitTests.Mappers.EntityMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapToCustomerShould
{
    [Fact]
    public void Map_Resource_Location_As_Shallow_Reference()
    {
        var location = new LocationEntity { Id = "location-id", Type = LocationTypeConstants.Private };
        var resource = new ResourceEntity { Id = "resource-id", Location = location };
        location.Resources.Add(resource);

        var customer = new CustomerEntity
        {
            Id = "customer-id",
            PersonalInformationVisibility = PersonalInformationVisibilityConstants.Visible,
            Type = CustomerTypeConstants.Registered,
            PreferredLocations = [location],
            PreferredResources = [resource]
        };

        var sut = new EntityMapper();

        var result = sut.MapTo(customer);

        var preferredResourceLocation = result.PreferredResources.Single().Location;
        preferredResourceLocation.ShouldNotBeNull();
        preferredResourceLocation.Id.ShouldBe(location.Id);
        preferredResourceLocation.Resources.ShouldBeEmpty();

        var preferredLocationResourceLocation = result.PreferredLocations.Single().Resources.Single().Location;
        preferredLocationResourceLocation.ShouldNotBeNull();
        preferredLocationResourceLocation.Id.ShouldBe(location.Id);
        preferredLocationResourceLocation.Resources.ShouldBeEmpty();
    }
}