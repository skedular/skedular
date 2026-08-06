using Api.Shared.Services.Models;
using Location.Api.Models;
using Location.Api.Services;
using Location.Api.Services.Authorization;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;
using Microsoft.Extensions.Logging;
using Testing.Shared.Assertions;

namespace Location.Api.UnitTests.Services.ResourceAvailableHoursServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdateAvailableHoursPatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Authorization_Rejection_And_Rethrow(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IResourceRepository resourceRepository,
        [Frozen]
        ILocationRepository locationRepository,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        IOrganizationOfferingService organizationOfferingService,
        [Frozen]
        ILogger<ResourceAvailableHoursService> logger,
        ResourceAvailableHoursService sut,
        CancellationToken cancellationToken)
    {
        var locationEntity = new Shared.Database.Entities.Location
        {
            Id = "location-1",
            OrganizationId = "org-1",
            Organization = new Organization
            {
                Id = "org-1",
            },
        };
        var existingResource = new Resource
        {
            Id = "resource-1",
            Location = locationEntity,
            OrganizationTags = [],
        };
        var request = new ResourceAvailableHoursPatchRequest(
            "resource-1",
            true,
            WeekOpeningHours.Default,
            new HashSet<LocationResourceAvailableHoursPatchField>
            {
                LocationResourceAvailableHoursPatchField.AvailableHours,
            });

        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("cust-1");
        A.CallTo(() => resourceRepository.GetByIdAsync("resource-1", cancellationToken)).Returns(existingResource);
        A.CallTo(() => locationRepository.GetByIdAsync("location-1", cancellationToken)).Returns(locationEntity);
        A.CallTo(() => organizationOfferingService.IsMoreInteractionAllowedAsync("org-1", "cust-1", cancellationToken))
            .Returns(new ValueTask<bool>(true));
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync("org-1", "cust-1", cancellationToken))
            .Returns(new ValueTask<bool>(false));

        await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            sut.UpdateAvailableHoursAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Warning)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("rejected by authorization"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Error_And_Rethrow_On_General_Failure(
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        ILogger<ResourceAvailableHoursService> logger,
        ResourceAvailableHoursService sut,
        CancellationToken cancellationToken)
    {
        var request = new ResourceAvailableHoursPatchRequest(
            "resource-1",
            true,
            WeekOpeningHours.Default,
            new HashSet<LocationResourceAvailableHoursPatchField>
            {
                LocationResourceAvailableHoursPatchField.AvailableHours,
            });

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken))
            .ThrowsAsync(new InvalidOperationException("cache failure"));

        await Should.ThrowAsync<InvalidOperationException>(() =>
            sut.UpdateAvailableHoursAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Error)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("Resource available hours patch autosave failed"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Autosave_Started(
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        ILogger<ResourceAvailableHoursService> logger,
        ResourceAvailableHoursService sut,
        CancellationToken cancellationToken)
    {
        var request = new ResourceAvailableHoursPatchRequest(
            "resource-1",
            true,
            WeekOpeningHours.Default,
            new HashSet<LocationResourceAvailableHoursPatchField>
            {
                LocationResourceAvailableHoursPatchField.AvailableHours,
            });

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken))
            .ThrowsAsync(new InvalidOperationException("forced early failure"));

        await Should.ThrowAsync<InvalidOperationException>(() =>
            sut.UpdateAvailableHoursAsync(request, cancellationToken));

        LogAssertions.ACallToLogInfoContaining(logger, "Resource available hours patch autosave started")
            .MustHaveHappenedOnceExactly();
    }
}
