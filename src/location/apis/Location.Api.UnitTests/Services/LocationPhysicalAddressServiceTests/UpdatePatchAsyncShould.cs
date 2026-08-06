using Location.Api.Models;
using Location.Api.Services;
using Location.Api.Services.Authorization;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;
using Microsoft.Extensions.Logging;
using Testing.Shared.Assertions;

namespace Location.Api.UnitTests.Services.LocationPhysicalAddressServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdatePatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Authorization_Rejection_And_Rethrow(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ILocationPhysicalAddressRepository physicalAddressRepository,
        [Frozen]
        ILocationRepository locationRepository,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        ILogger<LocationPhysicalAddressService> logger,
        LocationPhysicalAddressService sut,
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
        var physicalAddressEntity = new LocationPhysicalAddress
        {
            Id = "pa-1",
            Location = locationEntity,
        };
        var request = new LocationPhysicalAddressPatchRequest(
            new Shared.Models.LocationPhysicalAddress
            {
                Id = "pa-1",
                Location = new Shared.Models.Location
                {
                    Id = "location-1",
                },
            },
            new HashSet<LocationPhysicalAddressPatchField>
            {
                LocationPhysicalAddressPatchField.Address,
            });

        A.CallTo(() => repositoryFactory.LocationPhysicalAddressRepository).Returns(physicalAddressRepository);
        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => physicalAddressRepository.GetByIdAsync("pa-1", cancellationToken)).Returns(physicalAddressEntity);
        A.CallTo(() => locationRepository.GetByIdAsync("location-1", cancellationToken)).Returns(locationEntity);
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
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ILocationPhysicalAddressRepository physicalAddressRepository,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        ILogger<LocationPhysicalAddressService> logger,
        LocationPhysicalAddressService sut,
        CancellationToken cancellationToken)
    {
        var request = new LocationPhysicalAddressPatchRequest(
            new Shared.Models.LocationPhysicalAddress
            {
                Id = "pa-1",
                Location = new Shared.Models.Location
                {
                    Id = "location-1",
                },
            },
            new HashSet<LocationPhysicalAddressPatchField>
            {
                LocationPhysicalAddressPatchField.Address,
            });

        A.CallTo(() => repositoryFactory.LocationPhysicalAddressRepository).Returns(physicalAddressRepository);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken))
            .ThrowsAsync(new InvalidOperationException("cache failure"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Error)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("Location physical address patch autosave failed"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Autosave_Started(
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        ILogger<LocationPhysicalAddressService> logger,
        LocationPhysicalAddressService sut,
        CancellationToken cancellationToken)
    {
        var request = new LocationPhysicalAddressPatchRequest(
            new Shared.Models.LocationPhysicalAddress
            {
                Id = "pa-1",
                Location = new Shared.Models.Location
                {
                    Id = "location-1",
                },
            },
            new HashSet<LocationPhysicalAddressPatchField>
            {
                LocationPhysicalAddressPatchField.Address,
            });

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken))
            .ThrowsAsync(new InvalidOperationException("forced early failure"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLogInfoContaining(logger, "Location physical address patch autosave started")
            .MustHaveHappenedOnceExactly();
    }
}
