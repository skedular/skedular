using Api.Shared.Services;
using Enterprise.Shared.Database;
using Location.Api.Services;
using Location.Api.Services.Authorization;
using Location.Shared.Repositories;
using Location.Shared.Services;
using Location.Shared.Services.Cache;

namespace Location.Api.UnitTests.Services.ResourceServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdateShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Exclude_The_Current_Resource_When_Checking_For_A_Duplicate_Name(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationOfferingService organizationOfferingService,
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
        var existingResource = new Location.Shared.Database.Entities.Resource { Id = "resource-1", Location = existingLocation };
        var resourceToUpdate = new Location.Shared.Models.Resource
        {
            Id = "resource-1",
            Name = "Desk A",
            Location = new Location.Shared.Models.Location { Id = "location-1" },
            Tags = [new Location.Shared.Models.OrganizationTag { Id = "tag-1" }]
        };

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => resourceRepository.GetByIdAsync("resource-1", cancellationToken)).Returns(existingResource);
        A.CallTo(() => locationRepository.GetByIdAsync("location-1", cancellationToken)).Returns(existingLocation);
        A.CallTo(() => organizationOfferingService.IsMoreInteractionAllowedAsync("org-1", "customer-1", cancellationToken))
            .Returns(new ValueTask<bool>(true));
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync("org-1", "customer-1", cancellationToken))
            .Returns(new ValueTask<bool>(true));
        A.CallTo(() => resourceRepository.ExistsActiveWithNameAsync("location-1", "Desk A", "resource-1", cancellationToken))
            .Returns(true);

        await Should.ThrowAsync<ResourceWithSameNameExist>(() => sut.UpdateAsync(resourceToUpdate, cancellationToken));

        A.CallTo(() => resourceRepository.ExistsActiveWithNameAsync("location-1", "Desk A", "resource-1", cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}