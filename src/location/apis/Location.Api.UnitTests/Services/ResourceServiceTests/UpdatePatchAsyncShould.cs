using Location.Api.Models;
using Location.Api.Services;
using Location.Api.Services.Authorization;
using Location.Shared.Database.Entities;
using Location.Shared.Mappers;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;
using Microsoft.Extensions.Logging;
using Testing.Shared.Assertions;
using OrganizationTag = Location.Shared.Models.OrganizationTag;

namespace Location.Api.UnitTests.Services.ResourceServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdatePatchAsyncShould
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
        IEntityMapper entityMapper,
        [Frozen]
        ILogger<ResourceService> logger,
        ResourceService sut,
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
        var resourceModel = new Shared.Models.Resource
        {
            Id = "resource-1",
            Name = "Desk A",
            Location = new Shared.Models.Location
            {
                Id = "location-1",
            },
            Tags =
            [
                new OrganizationTag
                {
                    Id = "tag-1",
                },
            ],
        };
        var request = new ResourcePatchRequest(
            new Shared.Models.Resource
            {
                Id = "resource-1",
                Name = "Updated Desk",
                Location = new Shared.Models.Location
                {
                    Id = "location-1",
                },
                Tags =
                [
                    new OrganizationTag
                    {
                        Id = "tag-1",
                    },
                ],
            },
            new HashSet<ResourcePatchField>
            {
                ResourcePatchField.Name,
            });

        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => resourceRepository.GetByIdAsync("resource-1", cancellationToken)).Returns(existingResource);
        A.CallTo(() => entityMapper.MapTo(existingResource)).Returns(resourceModel);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("cust-1");
        A.CallTo(() => locationRepository.GetByIdAsync("location-1", cancellationToken)).Returns(locationEntity);
        A.CallTo(() => organizationOfferingService.IsMoreInteractionAllowedAsync("org-1", "cust-1", cancellationToken))
            .Returns(new ValueTask<bool>(true));
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
        IResourceRepository resourceRepository,
        [Frozen]
        ILogger<ResourceService> logger,
        ResourceService sut,
        CancellationToken cancellationToken)
    {
        var request = new ResourcePatchRequest(
            new Shared.Models.Resource
            {
                Id = "resource-1",
                Name = "Desk A",
                Location = new Shared.Models.Location
                {
                    Id = "location-1",
                },
                Tags = [],
            },
            new HashSet<ResourcePatchField>
            {
                ResourcePatchField.Name,
            });

        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => resourceRepository.GetByIdAsync("resource-1", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("db failure"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Error)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("Resource patch autosave failed"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Autosave_Started(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IResourceRepository resourceRepository,
        [Frozen]
        ILogger<ResourceService> logger,
        ResourceService sut,
        CancellationToken cancellationToken)
    {
        var request = new ResourcePatchRequest(
            new Shared.Models.Resource
            {
                Id = "resource-1",
                Name = "Desk A",
                Location = new Shared.Models.Location
                {
                    Id = "location-1",
                },
                Tags = [],
            },
            new HashSet<ResourcePatchField>
            {
                ResourcePatchField.Name,
            });

        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => resourceRepository.GetByIdAsync("resource-1", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("forced early failure"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLogInfoContaining(logger, "Resource patch autosave started")
            .MustHaveHappenedOnceExactly();
    }
}
