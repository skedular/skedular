using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Location.Api.Services;
using Location.Shared.Repositories;

namespace Location.Api.UnitTests.Services.ResourceServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddShould
{
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
        var existingLocation = new Location.Shared.Database.Entities.Location
        {
            Id = "location-1",
            OrganizationId = "org-1",
            Organization = new Location.Shared.Database.Entities.Organization { Id = "org-1" }
        };
        var resourceToAdd = new Location.Shared.Models.Resource
        {
            Name = "Desk A",
            Location = new Location.Shared.Models.Location { Id = "location-1" },
            Tags = [new Location.Shared.Models.OrganizationTag { Id = "tag-1" }]
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