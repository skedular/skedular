using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Location.Api.Models;
using Location.Api.Services;
using Location.Api.Services.Authorization;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;
using Microsoft.Extensions.Logging;
using Testing.Shared.Assertions;

namespace Location.Api.UnitTests.Services.LocationOpeningHoursServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdateOpeningHoursPatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Authorization_Rejection_And_Rethrow(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationOfferingService organizationOfferingService,
        [Frozen] ILogger<LocationOpeningHoursService> logger,
        [Frozen] IUnitOfWork unitOfWork,
        LocationOpeningHoursService sut,
        CancellationToken cancellationToken)
    {
        var locationEntity = new Shared.Database.Entities.Location
        {
            Id = "location-1", OrganizationId = "org-1", Organization = new Organization { Id = "org-1" }
        };
        var request = new LocationOpeningHoursPatchRequest(
            "location-1",
            WeekOpeningHours.Default,
            new HashSet<LocationOpeningHoursPatchField> { LocationOpeningHoursPatchField.WeekOpeningHours });

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => locationRepository.GetByIdAsync("location-1", cancellationToken)).Returns(locationEntity);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("cust-1");
        A.CallTo(() => organizationOfferingService.IsMoreInteractionAllowedAsync("org-1", "cust-1", cancellationToken))
            .Returns(new ValueTask<bool>(true));
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync("org-1", "cust-1", cancellationToken)).Returns(new ValueTask<bool>(false));

        await Should.ThrowAsync<UnauthorizedAccessException>(() => sut.UpdateOpeningHoursAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Warning)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!.Contains("rejected by authorization"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Error_And_Rethrow_On_General_Failure(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] ILogger<LocationOpeningHoursService> logger,
        LocationOpeningHoursService sut,
        CancellationToken cancellationToken)
    {
        var request = new LocationOpeningHoursPatchRequest(
            "location-1",
            WeekOpeningHours.Default,
            new HashSet<LocationOpeningHoursPatchField> { LocationOpeningHoursPatchField.WeekOpeningHours });

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken))
            .ThrowsAsync(new InvalidOperationException("cache failure"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateOpeningHoursAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Error)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("Location opening hours patch autosave failed"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Autosave_Started(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] ILogger<LocationOpeningHoursService> logger,
        LocationOpeningHoursService sut,
        CancellationToken cancellationToken)
    {
        var request = new LocationOpeningHoursPatchRequest(
            "location-1",
            WeekOpeningHours.Default,
            new HashSet<LocationOpeningHoursPatchField> { LocationOpeningHoursPatchField.WeekOpeningHours });

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).ThrowsAsync(new InvalidOperationException("forced early failure"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateOpeningHoursAsync(request, cancellationToken));

        LogAssertions.ACallToLogInfoContaining(logger, "Location opening hours patch autosave started").MustHaveHappenedOnceExactly();
    }
}
