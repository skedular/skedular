using Location.Api.Models;
using Location.Api.Services;
using Location.Api.Services.Authorization;
using Location.Shared.Database.Entities;
using Location.Shared.Mappers;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;
using Microsoft.Extensions.Logging;
using Testing.Shared.Assertions;

namespace Location.Api.UnitTests.Services.LocationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdatePatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Authorization_Rejection_And_Rethrow(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationOfferingService organizationOfferingService,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] ILogger<LocationService> logger,
        LocationService sut,
        CancellationToken cancellationToken)
    {
        var locationEntity = new Shared.Database.Entities.Location
        {
            Id = "location-1", OrganizationId = "org-1", Organization = new Organization { Id = "org-1" }, OrganizationTags = []
        };
        var locationModel = new Shared.Models.Location
        {
            Id = "location-1", Organization = new Shared.Models.Organization { Id = "org-1" }, OrganizationTags = []
        };
        var request = new LocationPatchRequest(
            new Shared.Models.Location { Id = "location-1", Organization = new Shared.Models.Organization { Id = "org-1" }, OrganizationTags = [] },
            new HashSet<LocationPatchField> { LocationPatchField.Name });

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => locationRepository.GetByIdAsync("location-1", cancellationToken)).Returns(locationEntity);
        A.CallTo(() => entityMapper.MapTo(locationEntity)).Returns(locationModel);
        A.CallTo(() => cachedCustomerService.GetNullableAsync(cancellationToken))
            .ReturnsLazily(_ => new ValueTask<Customer?>(new Customer { Id = "cust-1" }));
        A.CallTo(() => organizationOfferingService.IsMoreInteractionAllowedAsync("org-1", "cust-1", cancellationToken))
            .Returns(new ValueTask<bool>(true));
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync("org-1", "cust-1", cancellationToken))
            .Returns(new ValueTask<bool>(false));

        await Should.ThrowAsync<UnauthorizedAccessException>(() => sut.UpdateAsync(request, false, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Warning)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("rejected by authorization"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Error_And_Rethrow_On_General_Failure(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] ILogger<LocationService> logger,
        LocationService sut,
        CancellationToken cancellationToken)
    {
        var request = new LocationPatchRequest(
            new Shared.Models.Location { Id = "location-1", Organization = new Shared.Models.Organization { Id = "org-1" }, OrganizationTags = [] },
            new HashSet<LocationPatchField> { LocationPatchField.Name });

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => locationRepository.GetByIdAsync("location-1", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("db failure"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateAsync(request, false, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Error)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("Location patch autosave failed"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Autosave_Started(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] ILogger<LocationService> logger,
        LocationService sut,
        CancellationToken cancellationToken)
    {
        var request = new LocationPatchRequest(
            new Shared.Models.Location { Id = "location-1", Organization = new Shared.Models.Organization { Id = "org-1" }, OrganizationTags = [] },
            new HashSet<LocationPatchField> { LocationPatchField.Name });

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => locationRepository.GetByIdAsync("location-1", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("forced early failure"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateAsync(request, false, cancellationToken));

        LogAssertions.ACallToLogInfoContaining(logger, "Location patch autosave started")
            .MustHaveHappenedOnceExactly();
    }
}
