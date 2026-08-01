using Location.Api.Models;
using Location.Api.Services;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;
using Microsoft.Extensions.Logging;
using Testing.Shared.Assertions;
using FloorPlan = Location.Shared.Database.Entities.FloorPlan;

namespace Location.Api.UnitTests.Services.FloorPlanServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdatePatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Authorization_Rejection_And_Rethrow(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IFloorPlanRepository floorPlanRepository,
        [Frozen] ICachedLocationService cachedLocationService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] ILogger<FloorPlanService> logger,
        FloorPlanService sut,
        ILocationRepository locationRepository,
        CancellationToken cancellationToken)
    {
        var locationModel = new Shared.Models.Location { Id = "location-1", Organization = new Organization { Id = "org-1" } };
        var locationEntity = new Shared.Database.Entities.Location
        {
            Id = "location-1", OrganizationId = "org-1", Organization = new Shared.Database.Entities.Organization { Id = "org-1" }
        };
        var floorPlanEntity = new FloorPlan { Id = "fp-1", Location = locationEntity };
        var request = new FloorPlanPatchRequest(
            new Shared.Models.FloorPlan { Id = "fp-1", Name = "Updated Floor", Location = locationModel },
            new HashSet<FloorPlanPatchField> { FloorPlanPatchField.Name });

        A.CallTo(() => repositoryFactory.FloorPlanRepository).Returns(floorPlanRepository);
        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => floorPlanRepository.GetByIdAsync("fp-1", cancellationToken)).Returns(floorPlanEntity);
        A.CallTo(() => cachedLocationService.GetByIdAsync("location-1", cancellationToken))
            .ReturnsLazily(_ => new ValueTask<Shared.Database.Entities.Location?>(locationEntity));
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("cust-1");
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync("org-1", "cust-1", cancellationToken))
            .Returns(new ValueTask<bool>(false));

        await Should.ThrowAsync<UnauthorizedAccessException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Warning)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("rejected by authorization"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Error_And_Rethrow_On_General_Failure(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IFloorPlanRepository floorPlanRepository,
        [Frozen] ILogger<FloorPlanService> logger,
        FloorPlanService sut,
        CancellationToken cancellationToken)
    {
        var request = new FloorPlanPatchRequest(
            new Shared.Models.FloorPlan { Id = "fp-1", Name = "Updated Floor", Location = new Shared.Models.Location { Id = "location-1" } },
            new HashSet<FloorPlanPatchField> { FloorPlanPatchField.Name });

        A.CallTo(() => repositoryFactory.FloorPlanRepository).Returns(floorPlanRepository);
        A.CallTo(() => floorPlanRepository.GetByIdAsync("fp-1", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("db failure"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Error)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("Floor plan patch autosave failed"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Autosave_Started(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IFloorPlanRepository floorPlanRepository,
        [Frozen] ILogger<FloorPlanService> logger,
        FloorPlanService sut,
        CancellationToken cancellationToken)
    {
        var request = new FloorPlanPatchRequest(
            new Shared.Models.FloorPlan { Id = "fp-1", Name = "Updated Floor", Location = new Shared.Models.Location { Id = "location-1" } },
            new HashSet<FloorPlanPatchField> { FloorPlanPatchField.Name });

        A.CallTo(() => repositoryFactory.FloorPlanRepository).Returns(floorPlanRepository);
        A.CallTo(() => floorPlanRepository.GetByIdAsync("fp-1", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("forced early failure"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLogInfoContaining(logger, "Floor plan patch autosave started")
            .MustHaveHappenedOnceExactly();
    }
}
