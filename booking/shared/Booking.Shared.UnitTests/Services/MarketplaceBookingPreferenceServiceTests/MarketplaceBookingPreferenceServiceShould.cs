using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingPreferenceServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceBookingPreferenceServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task PickResourceBasedOnCustomerPreferencesAsync_Returns_Resources_When_No_Customer_Preferences(
        [Frozen] IRepositoryFactory repositoryFactory,
        MarketplaceBookingPreferenceService sut,
        IResourceRepository resourceRepository,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        // Arrange
        var from = timeProvider.GetUtcNow();
        var until = from.AddHours(1);
        var emptyIds = Array.Empty<string>();
        var productVersion = new ProductVersion { OrganizationTags = [] };
        var availableResources = new List<Resource>
        {
            new() { Id = "res-1", Name = "Resource 1" }, new() { Id = "res-2", Name = "Resource 2" }, new() { Id = "res-3", Name = "Resource 3" }
        };

        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(null, null, from, until, emptyIds, emptyIds, emptyIds, cancellationToken))
            .Returns(availableResources);

        // Act
        var result = await sut.PickResourceBasedOnCustomerPreferencesAsync(null, from, until, productVersion, 2, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.First().Id.ShouldBe("res-1");
        result.Last().Id.ShouldBe("res-2");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task PickResourceBasedOnCustomerPreferencesAsync_Prioritizes_Customer_Preferred_Resources(
        [Frozen] IRepositoryFactory repositoryFactory,
        MarketplaceBookingPreferenceService sut,
        IResourceRepository resourceRepository,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        // Arrange
        var from = timeProvider.GetUtcNow();
        var until = from.AddHours(1);
        var emptyIds = Array.Empty<string>();
        var productVersion = new ProductVersion { OrganizationTags = [] };
        var preferredResource = new Resource { Id = "preferred-res", Name = "Preferred Resource" };
        var otherResource = new Resource { Id = "other-res", Name = "Other Resource" };
        var availableResources = new List<Resource> { otherResource, preferredResource };

        var customer = new Customer { Id = "customer-1", PreferredResources = [preferredResource] };

        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(null, null, from, until, emptyIds, emptyIds, emptyIds, cancellationToken))
            .Returns(availableResources);

        // Act
        var result = await sut.PickResourceBasedOnCustomerPreferencesAsync(customer, from, until, productVersion, 1, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result.First().Id.ShouldBe("preferred-res");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task PickResourceBasedOnCustomerPreferencesAsync_Throws_NoResourceAvailable_When_Insufficient_Resources(
        [Frozen] IRepositoryFactory repositoryFactory,
        MarketplaceBookingPreferenceService sut,
        IResourceRepository resourceRepository,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        // Arrange
        var from = timeProvider.GetUtcNow();
        var until = from.AddHours(1);
        var emptyIds = Array.Empty<string>();
        var productVersion = new ProductVersion { OrganizationTags = [] };
        var availableResources = new List<Resource> { new() { Id = "res-1" } };

        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(null, null, from, until, emptyIds, emptyIds, emptyIds, cancellationToken))
            .Returns(availableResources);

        // Act & Assert
        await Should.ThrowAsync<NoResourceAvailable>(async () =>
            await sut.PickResourceBasedOnCustomerPreferencesAsync(null, from, until, productVersion, 2, cancellationToken));
    }
}
