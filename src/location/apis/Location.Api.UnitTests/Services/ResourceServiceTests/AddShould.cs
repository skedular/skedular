using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Random;
using Location.Api.Services;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using OrganizationTag = Location.Shared.Models.OrganizationTag;
using Resource = Location.Shared.Models.Resource;

namespace Location.Api.UnitTests.Services.ResourceServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_Direct_Resource_Creation_For_Host_Organizations(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        ResourceService sut,
        string locationId,
        string organizationId,
        string resourceName,
        CancellationToken cancellationToken)
    {
        var existingLocation = new Shared.Database.Entities.Location
        {
            Id = locationId,
            OrganizationId = organizationId,
            Organization = new Organization { Id = organizationId, Type = OrganizationTypeConstants.Host }
        };
        var resourceToAdd = new Resource { Name = resourceName, Location = new Shared.Models.Location { Id = locationId }, Tags = [] };

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => locationRepository.GetByIdAsync(locationId, cancellationToken)).Returns(existingLocation);

        var exception = await Should.ThrowAsync<InvalidOperationException>(() => sut.AddAsync(resourceToAdd, true, cancellationToken));

        exception.Message.ShouldBe("Host resources are system managed and cannot be changed directly.");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_Another_Active_Resource_Already_Uses_The_Name(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IResourceRepository resourceRepository,
        ResourceService sut,
        CancellationToken cancellationToken)
    {
        var existingLocation = new Shared.Database.Entities.Location
        {
            Id = "location-1", OrganizationId = "org-1", Organization = new Organization { Id = "org-1" }
        };
        var resourceToAdd = new Resource
        {
            Name = "Desk A", Location = new Shared.Models.Location { Id = "location-1" }, Tags = [new OrganizationTag { Id = "tag-1" }]
        };

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => randomHelper.Generate()).Returns("resource-1");
        A.CallTo(() => locationRepository.GetByIdAsync("location-1", cancellationToken)).Returns(existingLocation);
        A.CallTo(() => resourceRepository.ExistsActiveWithNameAsync("location-1", "Desk A", null, cancellationToken)).Returns(true);

        await Should.ThrowAsync<ResourceWithSameNameExist>(() => sut.AddAsync(resourceToAdd, true, cancellationToken));

        A.CallTo(() => resourceRepository.ExistsActiveWithNameAsync("location-1", "Desk A", null, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
