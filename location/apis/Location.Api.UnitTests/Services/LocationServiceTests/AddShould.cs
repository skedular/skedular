using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Shared.Models;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Location.Shared.Services;
using Location.Shared.Services.Cache;
using Location.Shared.Workflows;
using Microsoft.EntityFrameworkCore.Storage;

namespace Location.Api.UnitTests.Services.LocationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Load_Organisation_Tags_Through_The_Repository_Method(
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] ILocationOutboxPublisher locationOutboxPublisher,
        [Frozen] ITemporalOutboxService temporalOutboxService,
        [Frozen] IMapper mapper,
        [Frozen] ICachedLocationService cachedLocationService,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationTagRepository organizationTagRepository,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        [Frozen] TimeProvider timeProvider,
        LocationService sut,
        CancellationToken cancellationToken)
    {
        var locationToAdd = new Shared.Models.Location
        {
            Name = "Head Office", Organization = new Organization { Id = "org-1" }, OrganizationTags = [new OrganizationTag { Id = "tag-1" }]
        };
        var organizationEntity = new Shared.Database.Entities.Organization { Id = "org-1" };
        var organizationTagEntity = new Shared.Database.Entities.OrganizationTag { Id = "tag-1", Organization = organizationEntity };
        var locationEntity = new Shared.Database.Entities.Location
        {
            Id = "location-1",
            Name = "Head Office",
            OrganizationId = "org-1",
            Organization = organizationEntity,
            OrganizationTags = [organizationTagEntity]
        };
        var mappedLocation = new Shared.Models.Location
        {
            Id = "location-1",
            Name = "Head Office",
            Organization = new Organization { Id = "org-1" },
            OrganizationTags = [new OrganizationTag { Id = "tag-1" }]
        };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationTagRepository).Returns(organizationTagRepository);
        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => cachedCustomerService.GetNullableAsync(cancellationToken)).Returns(null);
        A.CallTo(() => organizationRepository.UpsertNakedAsync("org-1", cancellationToken)).Returns(organizationEntity);
        A.CallTo(() => randomHelper.Generate()).Returns("location-1");
        A.CallTo(() => organizationTagRepository.GetActiveByIdsForOrganizationAsync(
                A<ICollection<string>>.That.Matches(ids => ids.Count == 1 && ids.Single() == "tag-1"),
                "org-1",
                null,
                cancellationToken))
            .Returns([organizationTagEntity]);
        A.CallTo(() => mapper.MapTo(locationToAdd, organizationEntity, A<ICollection<Shared.Database.Entities.OrganizationTag>>._))
            .Returns(locationEntity);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => locationRepository.Add(locationEntity)).Returns(locationEntity);
        A.CallTo(() => mapper.MapTo(locationEntity)).Returns(mappedLocation);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(new DateTimeOffset(2026, 4, 19, 12, 0, 0, TimeSpan.Zero));
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).Returns(Task.CompletedTask);

        var result = await sut.AddAsync(locationToAdd, true, cancellationToken);

        result.Id.ShouldBe("location-1");
        A.CallTo(() => organizationTagRepository.GetActiveByIdsForOrganizationAsync(
                A<ICollection<string>>.That.Matches(ids => ids.Count == 1 && ids.Single() == "tag-1"),
                "org-1",
                null,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => locationOutboxPublisher.PublishLocations(A<ICollection<Shared.Models.Location>>._, unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => temporalOutboxService.StartWorkflowLocationDailyAnalytics(A<GenerateLocationDailyAnalyticsInput>._, unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedLocationService.UpdateByIdAsync("location-1", cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
