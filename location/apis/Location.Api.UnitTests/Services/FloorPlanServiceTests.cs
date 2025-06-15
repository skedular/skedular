using Api.Shared.Services.Models;
using AutoFixture.Xunit3;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using FakeItEasy;
using FluentAssertions;
using Location.Api.Exceptions;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using Location.Shared.Repositories;
using Location.Shared.Services;
using Microsoft.AspNetCore.Http;
using Testing.Shared;
using Xunit;

namespace Location.Api.UnitTests.Services;

public class FloorPlanServiceTests
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task GetByIdAsync_WhenFloorPlanExists_ReturnsFloorPlan(
        string floorPlanId,
        Customer customer,
        FloorPlan expectedFloorPlan,
        Shared.Database.Entities.FloorPlan floorPlanEntity,
        Shared.Database.Entities.Location locationEntity,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMapper mapper,
        FloorPlanService sut)
    {
        var customerEntity = A.Fake<Shared.Database.Entities.Customer>();
        A.CallTo(() => cachedCustomerService.GetAsync(A<CancellationToken>._))
            .Returns((customer, customerEntity));
        A.CallTo(() => repositoryFactory.FloorPlanRepository.GetByIdAsync(floorPlanId, A<CancellationToken>._))
            .Returns(floorPlanEntity);
        A.CallTo(() => repositoryFactory.LocationRepository.GetByIdAsync(floorPlanEntity.LocationId, A<CancellationToken>._))
            .Returns(locationEntity);
        A.CallTo(() => organizationAuthorizationService.CanView(locationEntity.Organization, customer))
            .Returns(true);
        A.CallTo(() => mapper.MapFloorPlan(floorPlanEntity))
            .Returns(expectedFloorPlan);

        var result = await sut.GetByIdAsync(floorPlanId, CancellationToken.None);

        result.Should().Be(expectedFloorPlan);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task GetByIdAsync_WhenFloorPlanDoesNotExist_ReturnsNull(
        string floorPlanId,
        Customer customer,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICachedCustomerService cachedCustomerService,
        FloorPlanService sut)
    {
        var customerEntity = A.Fake<Shared.Database.Entities.Customer>();
        A.CallTo(() => cachedCustomerService.GetAsync(A<CancellationToken>._))
            .Returns((customer, customerEntity));
        A.CallTo(() => repositoryFactory.FloorPlanRepository.GetByIdAsync(floorPlanId, A<CancellationToken>._))
            .Returns((Shared.Database.Entities.FloorPlan?)null);

        var result = await sut.GetByIdAsync(floorPlanId, CancellationToken.None);

        result.Should().BeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task GetByIdAsync_WhenUserNotAuthorized_ThrowsUnauthorized(
        string floorPlanId,
        Customer customer,
        Shared.Database.Entities.FloorPlan floorPlanEntity,
        Shared.Database.Entities.Location locationEntity,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        FloorPlanService sut)
    {
        var customerEntity = A.Fake<Shared.Database.Entities.Customer>();
        A.CallTo(() => cachedCustomerService.GetAsync(A<CancellationToken>._))
            .Returns((customer, customerEntity));
        A.CallTo(() => repositoryFactory.FloorPlanRepository.GetByIdAsync(floorPlanId, A<CancellationToken>._))
            .Returns(floorPlanEntity);
        A.CallTo(() => repositoryFactory.LocationRepository.GetByIdAsync(floorPlanEntity.LocationId, A<CancellationToken>._))
            .Returns(locationEntity);
        A.CallTo(() => organizationAuthorizationService.CanView(locationEntity.Organization, customer))
            .Returns(false);

        var act = () => sut.GetByIdAsync(floorPlanId, CancellationToken.None);

        await act.Should().ThrowAsync<Unauthorized>();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task AddAsync_WhenFileSizeExceedsLimit_ThrowsFileSizeExceedsLimit(
        string locationId,
        string name,
        int floorLevel,
        string floorName,
        IFormFile imageFile,
        FloorPlanService sut)
    {
        A.CallTo(() => imageFile.Length).Returns(3 * 1024 * 1024); // 3MB

        var act = () => sut.AddAsync(locationId, name, floorLevel, floorName, imageFile, CancellationToken.None);

        await act.Should().ThrowAsync<FileSizeExceedsLimit>();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task AddAsync_WhenFloorPlanAlreadyExists_ThrowsFloorPlanAlreadyExistsForLevel(
        string locationId,
        string name,
        int floorLevel,
        string floorName,
        IFormFile imageFile,
        Customer customer,
        Shared.Database.Entities.Location locationEntity,
        Shared.Database.Entities.FloorPlan existingFloorPlan,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        FloorPlanService sut)
    {
        A.CallTo(() => imageFile.Length).Returns(1024 * 1024); // 1MB
        var customerEntity = A.Fake<Shared.Database.Entities.Customer>();
        A.CallTo(() => cachedCustomerService.GetAsync(A<CancellationToken>._))
            .Returns((customer, customerEntity));
        A.CallTo(() => repositoryFactory.LocationRepository.GetByIdAsync(locationId, A<CancellationToken>._))
            .Returns(locationEntity);
        A.CallTo(() => organizationAuthorizationService.CanModify(locationEntity.Organization, customer))
            .Returns(true);
        A.CallTo(() => repositoryFactory.FloorPlanRepository.GetByLocationIdAndFloorLevelAsync(locationId, floorLevel, A<CancellationToken>._))
            .Returns(existingFloorPlan);

        var act = () => sut.AddAsync(locationId, name, floorLevel, floorName, imageFile, CancellationToken.None);

        await act.Should().ThrowAsync<FloorPlanAlreadyExistsForLevel>();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task UpdateResourcePositionAsync_WhenResourceAndFloorPlanMismatch_ThrowsException(
        string resourceId,
        string floorPlanId,
        int x,
        int y,
        int width,
        int height,
        string shape,
        Dictionary<string, object> metadata,
        Customer customer,
        Shared.Database.Entities.Resource resourceEntity,
        Shared.Database.Entities.FloorPlan floorPlanEntity,
        Shared.Database.Entities.Location resourceLocation,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICachedCustomerService cachedCustomerService,
        FloorPlanService sut)
    {
        resourceEntity.Location = resourceLocation;
        resourceLocation.Id = "location1";
        floorPlanEntity.LocationId = "location2";

        var customerEntity = A.Fake<Shared.Database.Entities.Customer>();
        A.CallTo(() => cachedCustomerService.GetAsync(A<CancellationToken>._))
            .Returns((customer, customerEntity));
        A.CallTo(() => repositoryFactory.ResourceRepository.GetByIdAsync(resourceId, A<CancellationToken>._))
            .Returns(resourceEntity);
        A.CallTo(() => repositoryFactory.FloorPlanRepository.GetByIdAsync(floorPlanId, A<CancellationToken>._))
            .Returns(floorPlanEntity);

        var act = () => sut.UpdateResourcePositionAsync(resourceId, floorPlanId, x, y, width, height, shape, metadata, CancellationToken.None);

        await act.Should().ThrowAsync<ResourceAndFloorPlanLocationMismatch>();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task RemoveResourcePositionAsync_WhenPositionDoesNotExist_CompletesSuccessfully(
        string resourceId,
        Customer customer,
        Shared.Database.Entities.Resource resourceEntity,
        Shared.Database.Entities.Location locationEntity,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        FloorPlanService sut)
    {
        resourceEntity.Location = locationEntity;
        
        var customerEntity = A.Fake<Shared.Database.Entities.Customer>();
        A.CallTo(() => cachedCustomerService.GetAsync(A<CancellationToken>._))
            .Returns((customer, customerEntity));
        A.CallTo(() => repositoryFactory.ResourceRepository.GetByIdAsync(resourceId, A<CancellationToken>._))
            .Returns(resourceEntity);
        A.CallTo(() => repositoryFactory.LocationRepository.GetByIdAsync(locationEntity.Id, A<CancellationToken>._))
            .Returns(locationEntity);
        A.CallTo(() => organizationAuthorizationService.CanModify(locationEntity.Organization, customer))
            .Returns(true);
        A.CallTo(() => repositoryFactory.ResourcePositionRepository.GetByResourceIdAsync(resourceId, A<CancellationToken>._))
            .Returns((Shared.Database.Entities.ResourcePosition?)null);

        await sut.RemoveResourcePositionAsync(resourceId, CancellationToken.None);

        A.CallTo(() => repositoryFactory.ResourcePositionRepository.Remove(A<Shared.Database.Entities.ResourcePosition>._))
            .MustNotHaveHappened();
    }
}