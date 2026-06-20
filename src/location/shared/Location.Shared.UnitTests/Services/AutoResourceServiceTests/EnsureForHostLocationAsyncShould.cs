using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Location.Shared.Repositories;
using Location.Shared.Services;
using Microsoft.Extensions.Logging;
using LocationEntity = Location.Shared.Database.Entities.Location;
using OrganizationEntity = Location.Shared.Database.Entities.Organization;
using OrganizationTagEntity = Location.Shared.Database.Entities.OrganizationTag;
using ResourceEntity = Location.Shared.Database.Entities.Resource;

namespace Location.Shared.UnitTests.Services.AutoResourceServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EnsureForHostLocationAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task EnsureOnlyMatchingHostLocationsForProduct(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IOrganizationTagRepository organizationTagRepository,
        [Frozen] IResourceRepository resourceRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] ILogger<AutoResourceService> logger,
        string locationId,
        string productTagId,
        CancellationToken cancellationToken)
    {
        var matching = CreateLocation(locationId, OrganizationTypeConstants.Host);
        var productTag = new OrganizationTagEntity
        {
            Id = productTagId, Type = OrganizationTagTypeConstants.Product, Organization = matching.Organization
        };
        matching.OrganizationTags.Add(productTag);
        var resourceType = new OrganizationTagEntity
        {
            Id = "resource-tag", Type = OrganizationTagTypeConstants.ResourceEntireLocation, Organization = matching.Organization
        };
        var sut = new AutoResourceService(repositoryFactory, logger);

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.OrganizationTagRepository).Returns(organizationTagRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => locationRepository.GetByIdAsync(locationId, A<CancellationToken>._)).Returns(matching);
        A.CallTo(() => organizationTagRepository.GetActiveByTypeForOrganizationAsync(
                matching.OrganizationId, OrganizationTagTypeConstants.ResourceEntireLocation, A<CancellationToken>._))
            .Returns(resourceType);
        A.CallTo(() => organizationTagRepository.UpsertNakedAsync(productTag.Id, matching.Organization, A<CancellationToken>._))
            .Returns(productTag);

        await sut.EnsureForHostLocationAsync(locationId, productTagId, cancellationToken);

        A.CallTo(() => resourceRepository.Add(A<ResourceEntity>.That.Matches(resource =>
                resource.Location.Id == matching.Id && resource.OrganizationTags.Contains(productTag))))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task IgnoreNonHostLocation(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] ILogger<AutoResourceService> logger,
        string locationId,
        string productTagId,
        CancellationToken cancellationToken)
    {
        var location = CreateLocation(locationId, OrganizationTypeConstants.Marketplace);
        var sut = new AutoResourceService(repositoryFactory, logger);

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => locationRepository.GetByIdAsync(locationId, A<CancellationToken>._)).Returns(location);

        await sut.EnsureForHostLocationAsync(locationId, productTagId, cancellationToken);

        A.CallTo(() => repositoryFactory.ResourceRepository.Add(A<ResourceEntity>._)).MustNotHaveHappened();
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task RemainIdempotentWhenHiddenResourceExists(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IOrganizationTagRepository organizationTagRepository,
        [Frozen] IResourceRepository resourceRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] ILogger<AutoResourceService> logger,
        string locationId,
        string productTagId,
        CancellationToken cancellationToken)
    {
        var location = CreateLocation(locationId, OrganizationTypeConstants.Host);
        location.Resources.Add(new ResourceEntity { Id = $"host-location-resource-{location.Id}", Name = "Old host name", Location = location });
        var sut = new AutoResourceService(repositoryFactory, logger);

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.OrganizationTagRepository).Returns(organizationTagRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => locationRepository.GetByIdAsync(locationId, A<CancellationToken>._)).Returns(location);
        A.CallTo(() => organizationTagRepository.UpsertNakedAsync(productTagId, location.Organization, A<CancellationToken>._))
            .Returns(new OrganizationTagEntity { Id = productTagId, Organization = location.Organization });

        await sut.EnsureForHostLocationAsync(locationId, productTagId, cancellationToken);

        A.CallTo(() => resourceRepository.Add(A<ResourceEntity>._)).MustNotHaveHappened();
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task RestoreSoftDeletedHiddenResource(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IOrganizationTagRepository organizationTagRepository,
        [Frozen] IResourceRepository resourceRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] ILogger<AutoResourceService> logger,
        string locationId,
        string productTagId,
        CancellationToken cancellationToken)
    {
        var location = CreateLocation(locationId, OrganizationTypeConstants.Host);
        var hiddenResource = new ResourceEntity
        {
            Id = $"host-location-resource-{location.Id}", Name = "Archived resource", Location = location, DeletedAt = DateTimeOffset.UtcNow
        };
        location.Resources.Add(hiddenResource);
        var sut = new AutoResourceService(repositoryFactory, logger);

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.OrganizationTagRepository).Returns(organizationTagRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => locationRepository.GetByIdAsync(locationId, A<CancellationToken>._)).Returns(location);
        A.CallTo(() => organizationTagRepository.UpsertNakedAsync(productTagId, location.Organization, A<CancellationToken>._))
            .Returns(new OrganizationTagEntity { Id = productTagId, Organization = location.Organization });

        await sut.EnsureForHostLocationAsync(locationId, productTagId, cancellationToken);

        hiddenResource.DeletedAt.ShouldBeNull();
        hiddenResource.OrganizationTags.ShouldContain(tag => tag.Id == productTagId);
        A.CallTo(() => resourceRepository.Update(hiddenResource)).MustHaveHappenedOnceExactly();
        A.CallTo(() => resourceRepository.Add(A<ResourceEntity>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task CreateOneHiddenResourceForHostLocation(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IOrganizationTagRepository organizationTagRepository,
        [Frozen] IResourceRepository resourceRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] ILogger<AutoResourceService> logger,
        string locationId,
        string productTagId,
        CancellationToken cancellationToken)
    {
        var location = CreateLocation(locationId, OrganizationTypeConstants.Host);
        var resourceType = new OrganizationTagEntity
        {
            Id = "tag-resource-others", Type = OrganizationTagTypeConstants.ResourceEntireLocation, Organization = location.Organization
        };
        var sut = new AutoResourceService(repositoryFactory, logger);

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.OrganizationTagRepository).Returns(organizationTagRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => locationRepository.GetByIdAsync(locationId, A<CancellationToken>._)).Returns(location);
        A.CallTo(() => organizationTagRepository.GetActiveByTypeForOrganizationAsync(
                location.OrganizationId,
                OrganizationTagTypeConstants.ResourceEntireLocation,
                A<CancellationToken>._))
            .Returns(resourceType);
        var productTag = new OrganizationTagEntity { Id = productTagId, Organization = location.Organization };
        A.CallTo(() => organizationTagRepository.UpsertNakedAsync(productTagId, location.Organization, A<CancellationToken>._))
            .Returns(productTag);

        ResourceEntity? added = null;
        A.CallTo(() => resourceRepository.Add(A<ResourceEntity>._))
            .Invokes(call => added = call.GetArgument<ResourceEntity>(0))
            .ReturnsLazily(call => call.GetArgument<ResourceEntity>(0)!);

        await sut.EnsureForHostLocationAsync(locationId, productTagId, cancellationToken);

        added.ShouldNotBeNull();
        added.Id.ShouldBe($"host-location-resource-{location.Id}");
        added.Name.ShouldBe($"Host: {location.Name}");
        added.Location.ShouldBeSameAs(location);
        added.Capacity.ShouldBe(1);
        added.OrganizationTags.ShouldBe([resourceType, productTag]);
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    private static LocationEntity CreateLocation(string locationId, string organizationType)
    {
        var organization = new OrganizationEntity { Id = "organization-1", Type = organizationType };

        return new LocationEntity
        {
            Id = locationId,
            Name = "Garden Studio",
            Type = LocationTypeConstants.Marketplace,
            OrganizationId = organization.Id,
            Organization = organization,
            Resources = [],
            OrganizationTags = []
        };
    }
}
