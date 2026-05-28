using Enterprise.Shared.Context;
using Location.Api.Models;
using Location.Api.Services;
using Location.Api.Services.Authorization;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Microsoft.Extensions.Logging;
using Testing.Shared.Assertions;

namespace Location.Api.UnitTests.Services.LocationRestrictedInformationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdatePatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Authorization_Rejection_And_Rethrow(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRestrictedInformationRepository restrictedInfoRepository,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] IContext context,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ILogger<LocationRestrictedInformationService> logger,
        LocationRestrictedInformationService sut,
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var locationEntity = new Shared.Database.Entities.Location
        {
            Id = "location-1", OrganizationId = "org-1", Organization = new Organization { Id = "org-1" }
        };
        var existingEntity = new LocationRestrictedInformation { Id = "ri-1", Location = locationEntity };
        var request = new LocationRestrictedInformationPatchRequest(
            new Shared.Models.LocationRestrictedInformation
            {
                Id = "ri-1", Title = "Updated Title", Location = new Shared.Models.Location { Id = "location-1" }
            },
            new HashSet<LocationRestrictedInformationPatchField> { LocationRestrictedInformationPatchField.Title });

        A.CallTo(() => repositoryFactory.LocationRestrictedInformationRepository).Returns(restrictedInfoRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => restrictedInfoRepository.GetByIdAsync("ri-1", cancellationToken)).Returns(existingEntity);
        A.CallTo(() => context.GetVerifiableToken()).Returns(verifiableToken);
        A.CallTo(() => customerRepository.GetMinimalByVerifiableTokenUntrackedAsync(verifiableToken, cancellationToken))
            .Returns(new Customer { Id = "cust-1" });
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
        [Frozen] ILocationRestrictedInformationRepository restrictedInfoRepository,
        [Frozen] ILogger<LocationRestrictedInformationService> logger,
        LocationRestrictedInformationService sut,
        CancellationToken cancellationToken)
    {
        var request = new LocationRestrictedInformationPatchRequest(
            new Shared.Models.LocationRestrictedInformation
            {
                Id = "ri-1", Title = "Updated Title", Location = new Shared.Models.Location { Id = "location-1" }
            },
            new HashSet<LocationRestrictedInformationPatchField> { LocationRestrictedInformationPatchField.Title });

        A.CallTo(() => repositoryFactory.LocationRestrictedInformationRepository).Returns(restrictedInfoRepository);
        A.CallTo(() => restrictedInfoRepository.GetByIdAsync("ri-1", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("db failure"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Error)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("Location restricted information patch autosave failed"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Autosave_Started(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRestrictedInformationRepository restrictedInfoRepository,
        [Frozen] ILogger<LocationRestrictedInformationService> logger,
        LocationRestrictedInformationService sut,
        CancellationToken cancellationToken)
    {
        var request = new LocationRestrictedInformationPatchRequest(
            new Shared.Models.LocationRestrictedInformation
            {
                Id = "ri-1", Title = "Updated Title", Location = new Shared.Models.Location { Id = "location-1" }
            },
            new HashSet<LocationRestrictedInformationPatchField> { LocationRestrictedInformationPatchField.Title });

        A.CallTo(() => repositoryFactory.LocationRestrictedInformationRepository).Returns(restrictedInfoRepository);
        A.CallTo(() => restrictedInfoRepository.GetByIdAsync("ri-1", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("forced early failure"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLogInfoContaining(logger, "Location restricted information patch autosave started")
            .MustHaveHappenedOnceExactly();
    }
}
