using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceEventResourceServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceEventResourceServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task PickEventResourcesAsync_Returns_All_Matching_Resources_For_Event_Product(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        MarketplaceEventResourceService sut,
        ILocationRepository locationRepository,
        IResourceRepository resourceRepository,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var from = timeProvider.GetUtcNow();
        var until = from.AddHours(2);
        var productTag = new OrganizationTag
        {
            Id = "tag-1",
            Type = OrganizationTagTypeConstants.Product,
        };
        var location = new Location
        {
            Id = "loc-1",
        };
        var firstResource = new Resource
        {
            Id = "res-1",
            Location = location,
            OrganizationTags = [productTag],
        };
        var secondResource = new Resource
        {
            Id = "res-2",
            Location = location,
            OrganizationTags = [productTag],
        };
        location.Resources = [firstResource, secondResource];
        var productVersion = new ProductVersion
        {
            Type = ProductTypeConstants.Event,
            OrganizationTags = [productTag],
        };

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => locationRepository.GetAllWithActiveOrganizationAsync(
                false,
                false,
                A<IReadOnlyList<string>>._,
                cancellationToken))
            .Returns([location]);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(
                null,
                null,
                from,
                until,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                cancellationToken))
            .Returns([firstResource, secondResource]);

        var result = await sut.PickEventResourcesAsync(from, until, productVersion, cancellationToken);

        result.Select(item => item.Id).ShouldBe(["res-1", "res-2"]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task PickEventResourcesAsync_Throws_NoResourceAvailable_When_Event_Product_Cannot_Book_Full_Set(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        MarketplaceEventResourceService sut,
        ILocationRepository locationRepository,
        IResourceRepository resourceRepository,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var from = timeProvider.GetUtcNow();
        var until = from.AddHours(2);
        var productTag = new OrganizationTag
        {
            Id = "tag-1",
            Type = OrganizationTagTypeConstants.Product,
        };
        var location = new Location
        {
            Id = "loc-1",
        };
        var firstResource = new Resource
        {
            Id = "res-1",
            Location = location,
            OrganizationTags = [productTag],
        };
        var secondResource = new Resource
        {
            Id = "res-2",
            Location = location,
            OrganizationTags = [productTag],
        };
        location.Resources = [firstResource, secondResource];
        var productVersion = new ProductVersion
        {
            Type = ProductTypeConstants.Event,
            OrganizationTags = [productTag],
        };

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => locationRepository.GetAllWithActiveOrganizationAsync(
                false,
                false,
                A<IReadOnlyList<string>>._,
                cancellationToken))
            .Returns([location]);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(
                null,
                null,
                from,
                until,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                cancellationToken))
            .Returns([firstResource]);

        await Should.ThrowAsync<NoResourceAvailable>(async () =>
            await sut.PickEventResourcesAsync(from, until, productVersion, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task PickEventResourcesAsync_Returns_All_Matching_Resources_Across_Multiple_Locations_For_Event_Product(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        MarketplaceEventResourceService sut,
        ILocationRepository locationRepository,
        IResourceRepository resourceRepository,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var from = timeProvider.GetUtcNow();
        var until = from.AddHours(2);
        var productTag = new OrganizationTag
        {
            Id = "tag-1",
            Type = OrganizationTagTypeConstants.Product,
        };
        var firstLocation = new Location
        {
            Id = "loc-1",
        };
        var secondLocation = new Location
        {
            Id = "loc-2",
        };
        var firstResource = new Resource
        {
            Id = "res-1",
            Location = firstLocation,
            OrganizationTags = [productTag],
        };
        var secondResource = new Resource
        {
            Id = "res-2",
            Location = secondLocation,
            OrganizationTags = [productTag],
        };
        firstLocation.Resources = [firstResource];
        secondLocation.Resources = [secondResource];
        var productVersion = new ProductVersion
        {
            Type = ProductTypeConstants.Event,
            OrganizationTags = [productTag],
        };

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => locationRepository.GetAllWithActiveOrganizationAsync(
                false,
                false,
                A<IReadOnlyList<string>>._,
                cancellationToken))
            .Returns([firstLocation, secondLocation]);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(
                null,
                null,
                from,
                until,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                cancellationToken))
            .Returns([firstResource, secondResource]);

        var result = await sut.PickEventResourcesAsync(from, until, productVersion, cancellationToken);

        result.Select(item => item.Id).ShouldBe(["res-1", "res-2"]);
    }
}
