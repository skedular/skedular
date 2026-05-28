using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.ResourceServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ResourceServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Empty_Collection_When_No_Resource_Ids_Provided(
        ResourceService sut,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var from = timeProvider.GetUtcNow();

        var result = await sut.GetResourceEntitiesAndValidateAvailabilityAsync(
            from,
            from.AddHours(1),
            [],
            [],
            cancellationToken);

        result.ShouldBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Available_Resources_When_All_Requested_Resources_Are_Available(
        [Frozen] IRepositoryFactory repositoryFactory,
        ResourceService sut,
        TimeProvider timeProvider,
        IResourceRepository resourceRepository,
        CancellationToken cancellationToken)
    {
        var from = timeProvider.GetUtcNow();
        var until = from.AddHours(1);
        var resourceIds = new[] { "res-1", "res-2" };
        var tagIds = new[] { "tag-1" };
        var availableResources = new[] { new Resource { Id = "res-1" }, new Resource { Id = "res-2" } };

        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(
                null,
                null,
                from,
                until,
                A<IReadOnlyList<string>>.That.IsSameSequenceAs(resourceIds),
                A<IReadOnlyList<string>>.That.IsSameSequenceAs(tagIds),
                A<IReadOnlyList<string>>._,
                cancellationToken))
            .Returns(availableResources);

        var result = await sut.GetResourceEntitiesAndValidateAvailabilityAsync(
            from,
            until,
            resourceIds,
            tagIds,
            cancellationToken);

        result.ShouldBe(availableResources);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_ResourceNotAvailable_When_Not_All_Requested_Resources_Are_Available(
        [Frozen] IRepositoryFactory repositoryFactory,
        ResourceService sut,
        TimeProvider timeProvider,
        IResourceRepository resourceRepository,
        CancellationToken cancellationToken)
    {
        var from = timeProvider.GetUtcNow();
        var until = from.AddHours(1);
        var resourceIds = new[] { "res-1", "res-2" };
        var tagIds = new[] { "tag-1" };
        var availableResources = new[] { new Resource { Id = "res-1" } };

        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(
                null,
                null,
                from,
                until,
                A<IReadOnlyList<string>>.That.IsSameSequenceAs(resourceIds),
                A<IReadOnlyList<string>>.That.IsSameSequenceAs(tagIds),
                A<IReadOnlyList<string>>._,
                cancellationToken))
            .Returns(availableResources);

        await Should.ThrowAsync<ResourceNotAvailable>(async () =>
            await sut.GetResourceEntitiesAndValidateAvailabilityAsync(
                from,
                until,
                resourceIds,
                tagIds,
                cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_ResourceNotAvailable_When_Available_Resources_Do_Not_Match_Requested_Ids(
        [Frozen] IRepositoryFactory repositoryFactory,
        ResourceService sut,
        TimeProvider timeProvider,
        IResourceRepository resourceRepository,
        CancellationToken cancellationToken)
    {
        var from = timeProvider.GetUtcNow();
        var until = from.AddHours(1);
        var resourceIds = new[] { "res-1", "res-2" };
        var tagIds = new[] { "tag-1" };
        var availableResources = new[] { new Resource { Id = "res-1" }, new Resource { Id = "res-3" } };

        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(
                null,
                null,
                from,
                until,
                A<IReadOnlyList<string>>.That.IsSameSequenceAs(resourceIds),
                A<IReadOnlyList<string>>.That.IsSameSequenceAs(tagIds),
                A<IReadOnlyList<string>>._,
                cancellationToken))
            .Returns(availableResources);

        await Should.ThrowAsync<ResourceNotAvailable>(async () =>
            await sut.GetResourceEntitiesAndValidateAvailabilityAsync(
                from,
                until,
                resourceIds,
                tagIds,
                cancellationToken));
    }
}
