using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Location.Api.Models;
using Location.Api.Services;
using Location.Api.Services.Authorization;
using Location.Shared.Mappers;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Testing.Shared.Assertions;
using Resource = Location.Shared.Models.Resource;
using OrganizationTagEntity = Location.Shared.Database.Entities.OrganizationTag;
using LocationEntity = Location.Shared.Database.Entities.Location;
using ResourceEntity = Location.Shared.Database.Entities.Resource;

namespace Location.Api.UnitTests.Services.BulkAddResourcesServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ImportShould
{
    // ─── Input validation ────────────────────────────────────────────────────

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_Rows_Is_Empty(BulkAddResourcesService sut, CancellationToken cancellationToken)
    {
        var input = new BulkAddResources("location-1", []);

        await Should.ThrowAsync<ArgumentException>(() => sut.ImportAsync(input, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_Total_Quantity_Exceeds_100(
        BulkAddResourcesService sut,
        CancellationToken cancellationToken)
    {
        // Two rows with quantities that sum to 101
        var rows = new List<BulkAddResourceRow> { new("type-tag-1", "Desk", 51, [], [], []), new("type-tag-2", "Room", 50, [], [], []) };
        var input = new BulkAddResources("location-1", rows);

        await Should.ThrowAsync<ArgumentException>(() => sut.ImportAsync(input, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Failure_For_Row_With_Quantity_Less_Than_1(BulkAddResourcesService sut, CancellationToken cancellationToken)
    {
        // All rows have invalid quantity → service returns before location fetch; no mocks needed
        var rows = new List<BulkAddResourceRow> { new("type-tag-1", "Desk", 0, [], [], []) };
        var input = new BulkAddResources("location-1", rows);

        var results = await sut.ImportAsync(input, cancellationToken);

        results.Count.ShouldBe(1);
        results[0].FailureReason.ShouldBe("Quantity must be at least 1.");
        results[0].CreatedResources.ShouldBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Failure_For_Row_With_Invalid_Resource_Type_Tag(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IResourceRepository resourceRepository,
        [Frozen] IOrganizationTagRepository organizationTagRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationOfferingService organizationOfferingService,
        BulkAddResourcesService sut,
        CancellationToken cancellationToken)
    {
        var existingLocation = new LocationEntity { Id = "location-1", OrganizationId = "org-1" };

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => repositoryFactory.OrganizationTagRepository).Returns(organizationTagRepository);
        A.CallTo(() => locationRepository.GetByIdAsync("location-1", cancellationToken)).Returns(existingLocation);
        // Return empty — tag not found
        A.CallTo(() => organizationTagRepository.GetActiveByIdsForOrganizationAsync(
            A<IReadOnlyList<string>>._, "org-1", null, cancellationToken)).Returns([]);
        SetupAuth(cachedCustomerService, organizationAuthorizationService, organizationOfferingService, cancellationToken);

        var rows = new List<BulkAddResourceRow> { new("invalid-type-tag", "Desk", 1, [], [], []) };
        var input = new BulkAddResources("location-1", rows);

        var results = await sut.ImportAsync(input, cancellationToken);

        results.Count.ShouldBe(1);
        results[0].FailureReason.ShouldBe("Resource type not found or invalid.");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Failure_When_A_Custom_Tag_Is_Invalid(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IResourceRepository resourceRepository,
        [Frozen] IOrganizationTagRepository organizationTagRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationOfferingService organizationOfferingService,
        BulkAddResourcesService sut,
        CancellationToken cancellationToken)
    {
        var existingLocation = new LocationEntity { Id = "location-1", OrganizationId = "org-1" };
        var validTypeTag = new OrganizationTagEntity { Id = "type-tag-1", Type = "RESOURCE_DESK", Name = "Desk" };

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => repositoryFactory.OrganizationTagRepository).Returns(organizationTagRepository);
        A.CallTo(() => locationRepository.GetByIdAsync("location-1", cancellationToken)).Returns(existingLocation);
        A.CallTo(() => organizationTagRepository.GetActiveByIdsForOrganizationAsync(
                A<IReadOnlyList<string>>.That.Contains("type-tag-1"), "org-1", null, cancellationToken))
            .Returns([validTypeTag]);
        // For non-type tags call, return empty — simulates invalid custom tag
        A.CallTo(() => organizationTagRepository.GetActiveByIdsForOrganizationAsync(
                A<IReadOnlyList<string>>.That.Contains("bad-custom-tag"), "org-1", null, cancellationToken))
            .Returns([]);
        A.CallTo(() => resourceRepository.GetActiveNamesByLocationIdAsync("location-1", cancellationToken)).Returns([]);
        SetupAuth(cachedCustomerService, organizationAuthorizationService, organizationOfferingService, cancellationToken);

        var rows = new List<BulkAddResourceRow> { new("type-tag-1", "Desk", 1, ["bad-custom-tag"], [], []) };
        var input = new BulkAddResources("location-1", rows);

        var results = await sut.ImportAsync(input, cancellationToken);

        results.Count.ShouldBe(1);
        results[0].FailureReason.ShouldBe("One or more tag identifiers are invalid.");
    }

    // ─── Naming logic ────────────────────────────────────────────────────────

    [Theory]
    [AutoFakeItEasyData]
    public async Task Generate_Name_With_BaseName_And_Incrementing_Suffix(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IResourceRepository resourceRepository,
        [Frozen] IOrganizationTagRepository organizationTagRepository,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationOfferingService organizationOfferingService,
        BulkAddResourcesService sut,
        CancellationToken cancellationToken)
    {
        SetupSuccessfulTransaction(transactionBuilder, repositoryFactory, locationRepository, resourceRepository,
            organizationTagRepository, entityMapper, randomHelper,
            cachedCustomerService, organizationAuthorizationService, organizationOfferingService,
            cancellationToken, [], "Desk");

        var rows = new List<BulkAddResourceRow> { new("type-tag-1", "Desk", 3, [], [], []) };
        var input = new BulkAddResources("location-1", rows);

        var results = await sut.ImportAsync(input, cancellationToken);

        results.ShouldHaveSingleItem();
        results[0].FailureReason.ShouldBeNull();
        results[0].CreatedResources.Select(r => r.Name).ShouldBe(["Desk-1", "Desk-2", "Desk-3"]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Use_Resource_Type_Name_As_BaseName_When_BaseName_Is_Empty(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IResourceRepository resourceRepository,
        [Frozen] IOrganizationTagRepository organizationTagRepository,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationOfferingService organizationOfferingService,
        BulkAddResourcesService sut,
        CancellationToken cancellationToken)
    {
        SetupSuccessfulTransaction(transactionBuilder, repositoryFactory, locationRepository, resourceRepository,
            organizationTagRepository, entityMapper, randomHelper,
            cachedCustomerService, organizationAuthorizationService, organizationOfferingService,
            cancellationToken, [], "Meeting Room");

        var rows = new List<BulkAddResourceRow>
        {
            new("type-tag-1", null, 2, [], [], []) // no base name
        };
        var input = new BulkAddResources("location-1", rows);

        var results = await sut.ImportAsync(input, cancellationToken);

        results.ShouldHaveSingleItem();
        results[0].FailureReason.ShouldBeNull();
        results[0].CreatedResources.Select(r => r.Name).ShouldBe(["Meeting Room-1", "Meeting Room-2"]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Append_After_Highest_Existing_Suffix_Rather_Than_Filling_Gaps(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IResourceRepository resourceRepository,
        [Frozen] IOrganizationTagRepository organizationTagRepository,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationOfferingService organizationOfferingService,
        BulkAddResourcesService sut,
        CancellationToken cancellationToken)
    {
        // Existing: Desk-1, Desk-3 → next should be Desk-4, Desk-5, Desk-6
        SetupSuccessfulTransaction(transactionBuilder, repositoryFactory, locationRepository, resourceRepository,
            organizationTagRepository, entityMapper, randomHelper,
            cachedCustomerService, organizationAuthorizationService, organizationOfferingService,
            cancellationToken, ["Desk-1", "Desk-3"], "Desk");

        var rows = new List<BulkAddResourceRow> { new("type-tag-1", "Desk", 3, [], [], []) };
        var input = new BulkAddResources("location-1", rows);

        var results = await sut.ImportAsync(input, cancellationToken);

        results.ShouldHaveSingleItem();
        results[0].FailureReason.ShouldBeNull();
        results[0].CreatedResources.Select(r => r.Name).ShouldBe(["Desk-4", "Desk-5", "Desk-6"]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Avoid_Within_Batch_Name_Collisions_Across_Rows(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IResourceRepository resourceRepository,
        [Frozen] IOrganizationTagRepository organizationTagRepository,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationOfferingService organizationOfferingService,
        BulkAddResourcesService sut,
        CancellationToken cancellationToken)
    {
        // Two rows with same base name — names must not collide within the batch
        SetupSuccessfulTransaction(transactionBuilder, repositoryFactory, locationRepository, resourceRepository,
            organizationTagRepository, entityMapper, randomHelper,
            cachedCustomerService, organizationAuthorizationService, organizationOfferingService,
            cancellationToken, [], "Desk");

        var rows = new List<BulkAddResourceRow> { new("type-tag-1", "Desk", 2, [], [], []), new("type-tag-1", "Desk", 1, [], [], []) };
        var input = new BulkAddResources("location-1", rows);

        var results = await sut.ImportAsync(input, cancellationToken);

        var allNames = results.SelectMany(r => r.CreatedResources).Select(r => r.Name).ToList();
        allNames.Distinct().Count().ShouldBe(allNames.Count); // all unique
        allNames.ShouldContain("Desk-1");
        allNames.ShouldContain("Desk-2");
        allNames.ShouldContain("Desk-3");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Avoid_Cross_Row_Name_Collision_When_Existing_Names_Are_Present(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IResourceRepository resourceRepository,
        [Frozen] IOrganizationTagRepository organizationTagRepository,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationOfferingService organizationOfferingService,
        BulkAddResourcesService sut,
        CancellationToken cancellationToken)
    {
        // Existing: Desk-2. Two rows both with BaseName="Desk", Quantity=2.
        // Expected: Desk-3, Desk-4 for row 0 and Desk-5, Desk-6 for row 1.
        SetupSuccessfulTransaction(transactionBuilder, repositoryFactory, locationRepository, resourceRepository,
            organizationTagRepository, entityMapper, randomHelper,
            cachedCustomerService, organizationAuthorizationService, organizationOfferingService,
            cancellationToken, ["Desk-2"], "Desk");

        var rows = new List<BulkAddResourceRow> { new("type-tag-1", "Desk", 2, [], [], []), new("type-tag-1", "Desk", 2, [], [], []) };
        var input = new BulkAddResources("location-1", rows);

        var results = await sut.ImportAsync(input, cancellationToken);

        var row0Names = results[0].CreatedResources.Select(r => r.Name).OrderBy(n => n).ToList();
        var row1Names = results[1].CreatedResources.Select(r => r.Name).OrderBy(n => n).ToList();

        row0Names.ShouldBe(["Desk-3", "Desk-4"]);
        row1Names.ShouldBe(["Desk-5", "Desk-6"]);

        var allNames = row0Names.Concat(row1Names).ToList();
        allNames.Distinct().Count().ShouldBe(allNames.Count); // all unique across both rows
    }

    // ─── Partial success ─────────────────────────────────────────────────────

    [Theory]
    [AutoFakeItEasyData]
    public async Task Create_Valid_Rows_And_Return_Failure_For_Invalid_Rows(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IResourceRepository resourceRepository,
        [Frozen] IOrganizationTagRepository organizationTagRepository,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationOfferingService organizationOfferingService,
        BulkAddResourcesService sut,
        CancellationToken cancellationToken)
    {
        var existingLocation = new LocationEntity { Id = "location-1", OrganizationId = "org-1" };
        var validTypeTag = new OrganizationTagEntity { Id = "type-tag-1", Type = "RESOURCE_DESK", Name = "Desk" };
        var fakeTransaction = A.Fake<IDbContextTransaction>();
        var locationModel = new Shared.Models.Location { Id = "location-1" };
        var resourceEntity = new ResourceEntity { Id = "res-1", Name = "Desk-1", Location = existingLocation };
        var resourceModel = new Resource { Id = "res-1", Name = "Desk-1", Location = locationModel };

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => repositoryFactory.OrganizationTagRepository).Returns(organizationTagRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(A.Fake<IUnitOfWork>());
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(A<IUnitOfWork>._, cancellationToken)).Returns(fakeTransaction);
        A.CallTo(() => locationRepository.GetByIdAsync("location-1", cancellationToken)).Returns(existingLocation);
        A.CallTo(() => organizationTagRepository.GetActiveByIdsForOrganizationAsync(
            A<IReadOnlyList<string>>._, "org-1", null, cancellationToken)).Returns([validTypeTag]);
        A.CallTo(() => resourceRepository.GetActiveNamesByLocationIdAsync("location-1", cancellationToken)).Returns([]);
        A.CallTo(() => randomHelper.Generate()).Returns("res-1");
        A.CallTo(() => entityMapper.MapTo(existingLocation)).Returns(locationModel);
        A.CallTo(() => entityMapper.MapTo(A<Resource>._, existingLocation, A<IReadOnlyList<OrganizationTagEntity>>._))
            .Returns(resourceEntity);
        A.CallTo(() => resourceRepository.Add(A<ResourceEntity>._)).ReturnsLazily(call =>
        {
            var r = call.GetArgument<ResourceEntity>(0)!;
            return r;
        });
        A.CallTo(() => entityMapper.MapTo(A<ResourceEntity>._, locationModel)).Returns(resourceModel);
        SetupAuth(cachedCustomerService, organizationAuthorizationService, organizationOfferingService, cancellationToken);

        // Row 0: invalid (quantity 0), Row 1: valid
        var rows = new List<BulkAddResourceRow>
        {
            new("type-tag-1", "Desk", 0, [], [], []), // invalid
            new("type-tag-1", "Desk", 1, [], [], []) // valid
        };
        var input = new BulkAddResources("location-1", rows);

        var results = await sut.ImportAsync(input, cancellationToken);

        results.Count.ShouldBe(2);
        var failed = results.Single(r => r.FailureReason != null);
        var succeeded = results.Single(r => r.FailureReason == null);

        failed.RowIndex.ShouldBe(0);
        failed.CreatedResources.ShouldBeEmpty();

        succeeded.RowIndex.ShouldBe(1);
        succeeded.CreatedResources.ShouldNotBeEmpty();
    }

    // ─── Cache invalidation ──────────────────────────────────────────────────

    [Theory]
    [AutoFakeItEasyData]
    public async Task Invalidate_Location_Cache_After_Successful_Import(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IResourceRepository resourceRepository,
        [Frozen] IOrganizationTagRepository organizationTagRepository,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] ICachedLocationService cachedLocationService,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationOfferingService organizationOfferingService,
        BulkAddResourcesService sut,
        CancellationToken cancellationToken)
    {
        SetupSuccessfulTransaction(transactionBuilder, repositoryFactory, locationRepository, resourceRepository,
            organizationTagRepository, entityMapper, randomHelper,
            cachedCustomerService, organizationAuthorizationService, organizationOfferingService,
            cancellationToken, [], "Desk");

        var rows = new List<BulkAddResourceRow> { new("type-tag-1", "Desk", 1, [], [], []) };
        var input = new BulkAddResources("location-1", rows);

        await sut.ImportAsync(input, cancellationToken);

        A.CallTo(() => cachedLocationService.RemoveByIdAsync("location-1", cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Not_Invalidate_Location_Cache_When_All_Rows_Are_Invalid(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IResourceRepository resourceRepository,
        [Frozen] IOrganizationTagRepository organizationTagRepository,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] ICachedLocationService cachedLocationService,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationOfferingService organizationOfferingService,
        BulkAddResourcesService sut,
        CancellationToken cancellationToken)
    {
        SetupSuccessfulTransaction(transactionBuilder, repositoryFactory, locationRepository, resourceRepository,
            organizationTagRepository, entityMapper, randomHelper,
            cachedCustomerService, organizationAuthorizationService, organizationOfferingService,
            cancellationToken, [], "Desk");

        // All rows have quantity < 1 — nothing will be written
        var rows = new List<BulkAddResourceRow> { new("type-tag-1", "Desk", 0, [], [], []) };
        var input = new BulkAddResources("location-1", rows);

        await sut.ImportAsync(input, cancellationToken);

        A.CallTo(() => cachedLocationService.RemoveByIdAsync(A<string>._, cancellationToken))
            .MustNotHaveHappened();
    }

    // ─── Logging ─────────────────────────────────────────────────────────────

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Batch_Received_And_Completion_Summary(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] ILocationRepository locationRepository,
        [Frozen] IResourceRepository resourceRepository,
        [Frozen] IOrganizationTagRepository organizationTagRepository,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] ILogger<BulkAddResourcesService> logger,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationOfferingService organizationOfferingService,
        BulkAddResourcesService sut,
        CancellationToken cancellationToken)
    {
        SetupSuccessfulTransaction(transactionBuilder, repositoryFactory, locationRepository, resourceRepository,
            organizationTagRepository, entityMapper, randomHelper,
            cachedCustomerService, organizationAuthorizationService, organizationOfferingService,
            cancellationToken, [], "Desk");

        var rows = new List<BulkAddResourceRow> { new("type-tag-1", "Desk", 1, [], [], []) };
        var input = new BulkAddResources("location-1", rows);

        await sut.ImportAsync(input, cancellationToken);

        // LOG-004: verify structured log entries are emitted (batch received + completion)
        LogAssertions.ACallToLogInfo(logger).MustHaveHappenedTwiceOrMore();
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static void SetupAuth(
        ICachedCustomerService cachedCustomerService,
        IOrganizationAuthorizationService organizationAuthorizationService,
        IOrganizationOfferingService organizationOfferingService,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => organizationOfferingService.IsMoreInteractionAllowedAsync("org-1", "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync("org-1", "customer-1", cancellationToken)).Returns(true);
    }

    private static void SetupSuccessfulTransaction(
        IDbTransactionBuilder transactionBuilder,
        IRepositoryFactory repositoryFactory,
        ILocationRepository locationRepository,
        IResourceRepository resourceRepository,
        IOrganizationTagRepository organizationTagRepository,
        IEntityMapper entityMapper,
        IRandomHelper randomHelper,
        ICachedCustomerService cachedCustomerService,
        IOrganizationAuthorizationService organizationAuthorizationService,
        IOrganizationOfferingService organizationOfferingService,
        CancellationToken cancellationToken,
        IReadOnlyList<string> existingNames,
        string typeTagName)
    {
        var existingLocation = new LocationEntity { Id = "location-1", OrganizationId = "org-1" };
        var validTypeTag = new OrganizationTagEntity { Id = "type-tag-1", Type = "RESOURCE_DESK", Name = typeTagName };
        var fakeTransaction = A.Fake<IDbContextTransaction>();
        var locationModel = new Shared.Models.Location { Id = "location-1" };

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => repositoryFactory.OrganizationTagRepository).Returns(organizationTagRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(A.Fake<IUnitOfWork>());
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(A<IUnitOfWork>._, cancellationToken)).Returns(fakeTransaction);
        A.CallTo(() => locationRepository.GetByIdAsync("location-1", cancellationToken)).Returns(existingLocation);
        A.CallTo(() => organizationTagRepository.GetActiveByIdsForOrganizationAsync(
            A<IReadOnlyList<string>>._, "org-1", null, cancellationToken)).Returns([validTypeTag]);
        A.CallTo(() => resourceRepository.GetActiveNamesByLocationIdAsync("location-1", cancellationToken)).Returns(existingNames);

        var counter = 0;
        A.CallTo(() => randomHelper.Generate()).ReturnsLazily(() => $"res-{++counter}");

        A.CallTo(() => entityMapper.MapTo(existingLocation)).Returns(locationModel);
        A.CallTo(() => entityMapper.MapTo(
                A<Resource>._,
                existingLocation,
                A<IReadOnlyList<OrganizationTagEntity>>._))
            .ReturnsLazily(call =>
            {
                var r = call.GetArgument<Resource>(0)!;
                return new ResourceEntity { Id = r.Id, Name = r.Name, Location = existingLocation };
            });

        A.CallTo(() => resourceRepository.Add(A<ResourceEntity>._))
            .ReturnsLazily(call => call.GetArgument<ResourceEntity>(0)!);

        A.CallTo(() => entityMapper.MapTo(A<ResourceEntity>._, locationModel))
            .ReturnsLazily(call =>
            {
                var entity = call.GetArgument<ResourceEntity>(0)!;
                return new Resource { Id = entity.Id, Name = entity.Name, Location = locationModel };
            });

        SetupAuth(cachedCustomerService, organizationAuthorizationService, organizationOfferingService, cancellationToken);
    }
}
