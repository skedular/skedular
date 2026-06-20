using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Location.Domain.IntegrationTests.Skedular.GraphQL.V1;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using NanoidDotNet;
using LocationEntity = Location.Shared.Database.Entities.Location;
using Offering = Api.Shared.Services.Models.Offering;
using ResourceEntity = Location.Shared.Database.Entities.Resource;

namespace Location.Domain.IntegrationTests.Api.GraphQL.Resource;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Location.Api")]
public class BulkAddResourcesShould(
    IBulkAddResourcesMutation bulkAddResourcesMutation,
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider)
{
    // ─── Helpers ─────────────────────────────────────────────────────────────

    /// <summary>
    ///     Seeds a minimal organization + location + resource-type tag so the mutation has valid IDs to work with.
    ///     Also seeds a customer with an identity whose ID becomes the bearer token for authenticated calls.
    /// </summary>
    private async Task<(string LocationId, string ResourceTypeTagId, string VerifiableToken)> SeedLocationWithResourceTypeTagAsync(
        string tagType,
        string tagName,
        CancellationToken cancellationToken)
    {
        var organizationId = await Nanoid.GenerateAsync();
        var locationId = await Nanoid.GenerateAsync();
        var tagId = await Nanoid.GenerateAsync();
        var customerId = await Nanoid.GenerateAsync();
        var verifiableToken = await Nanoid.GenerateAsync();
        var memberId = await Nanoid.GenerateAsync();
        var now = timeProvider.GetUtcNow();

        var organization = new Organization
        {
            Id = organizationId,
            CreatedAt = now,
            Offering = new Offering
            {
                Id = await Nanoid.GenerateAsync(), Code = OfferingCode.EnterpriseCustomV1, Start = now.AddYears(-1), End = now.AddYears(1)
            }
        };
        var location = new LocationEntity
        {
            Id = locationId,
            Name = "Bulk Import Test Location",
            OrganizationId = organizationId,
            Type = LocationTypeConstants.Private,
            CreatedAt = now
        };
        var resourceTypeTag = new OrganizationTag
        {
            Id = tagId,
            Type = tagType,
            Name = tagName,
            Organization = organization,
            CreatedAt = now
        };
        var customer = new Customer { Id = customerId, CreatedAt = now };
        var identity = new Identity { Id = verifiableToken, Customer = customer, CreatedAt = now };
        var member = new OrganizationMember
        {
            Id = memberId,
            OrganizationId = organizationId,
            CustomerId = customerId,
            Role = OrganizationMemberRoleConstants.Owner,
            Status = OrganizationMemberStatusConstants.Active,
            CreatedAt = now
        };

        await repositoryFactory.DbContext.Organization.AddAsync(organization, cancellationToken);
        await repositoryFactory.DbContext.Location.AddAsync(location, cancellationToken);
        await repositoryFactory.DbContext.OrganizationTag.AddAsync(resourceTypeTag, cancellationToken);
        await repositoryFactory.DbContext.Customer.AddAsync(customer, cancellationToken);
        await repositoryFactory.DbContext.Identity.AddAsync(identity, cancellationToken);
        await repositoryFactory.DbContext.OrganizationMember.AddAsync(member, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return (locationId, tagId, verifiableToken);
    }

    /// <summary>
    ///     Seeds a resource with a given name for a location to test name-collision handling.
    /// </summary>
    private async Task SeedResourceAsync(
        string locationId,
        string resourceName,
        string tagType,
        CancellationToken cancellationToken)
    {
        var resourceId = await Nanoid.GenerateAsync();
        var tagId = await Nanoid.GenerateAsync();
        var now = TimeProvider.System.GetUtcNow();

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        var tag = new OrganizationTag
        {
            Id = tagId,
            Type = tagType,
            Name = resourceName,
            Organization = location!.Organization,
            CreatedAt = now
        };
        var resource = new ResourceEntity
        {
            Id = resourceId,
            Name = resourceName,
            CreatedAt = now,
            Location = location,
            OrganizationTags = [tag]
        };

        await repositoryFactory.DbContext.OrganizationTag.AddAsync(tag, cancellationToken);
        await repositoryFactory.DbContext.Resource.AddAsync(resource, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    // ─── Tests ───────────────────────────────────────────────────────────────

    [Theory]
    [AutoFakeItEasyData]
    public async Task Create_Resources_For_A_Valid_Batch(CancellationToken cancellationToken)
    {
        var (locationId, tagId, verifiableToken) = await SeedLocationWithResourceTypeTagAsync(
            OrganizationTagTypeConstants.ResourceDesk, "Desk", cancellationToken);

        TestBearerTokenHandler.SetToken(verifiableToken);
        try
        {
            IReadOnlyList<BulkAddResourceRowInput> rows =
            [
                new()
                {
                    OrganizationResourceTypeTagId = tagId,
                    BaseName = "Desk",
                    Quantity = 3,
                    CustomTagIds = [],
                    ZoneIds = [],
                    ProductTagIds = []
                }
            ];

            var result = await bulkAddResourcesMutation.ExecuteAsync(locationId, rows, cancellationToken);

            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();
            var payload = result.Data.BulkAddResources;
            payload.Results.ShouldHaveSingleItem();
            payload.Results[0].FailureReason.ShouldBeNull();
            payload.Results[0].CreatedResources.Count.ShouldBe(3);

            // Verify names were auto-generated correctly
            var names = payload.Results[0].CreatedResources.Select(r => r.Name).OrderBy(n => n).ToList();
            names.ShouldBe(["Desk-1", "Desk-2", "Desk-3"]);

            // Verify resources persisted in DB
            var dbResources = await repositoryFactory.ResourceRepository.GetActiveNamesByLocationIdAsync(locationId, cancellationToken);
            dbResources.ShouldContain("Desk-1");
            dbResources.ShouldContain("Desk-2");
            dbResources.ShouldContain("Desk-3");
        }
        finally
        {
            TestBearerTokenHandler.ClearToken();
        }
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Create_Valid_Row_And_Return_Failure_For_Invalid_Row(CancellationToken cancellationToken)
    {
        var (locationId, tagId, verifiableToken) = await SeedLocationWithResourceTypeTagAsync(
            OrganizationTagTypeConstants.ResourceDesk, "Desk", cancellationToken);

        TestBearerTokenHandler.SetToken(verifiableToken);
        try
        {
            IReadOnlyList<BulkAddResourceRowInput> rows =
            [
                new()
                {
                    OrganizationResourceTypeTagId = tagId,
                    BaseName = "Desk",
                    Quantity = 1,
                    CustomTagIds = [],
                    ZoneIds = [],
                    ProductTagIds = []
                },
                new()
                {
                    OrganizationResourceTypeTagId = "invalid-tag-id",
                    BaseName = "Room",
                    Quantity = 1,
                    CustomTagIds = [],
                    ZoneIds = [],
                    ProductTagIds = []
                }
            ];

            var result = await bulkAddResourcesMutation.ExecuteAsync(locationId, rows, cancellationToken);

            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();
            var payload = result.Data.BulkAddResources;
            payload.Results.Count.ShouldBe(2);

            var succeeded = payload.Results.Single(r => r.FailureReason is null);
            var failed = payload.Results.Single(r => r.FailureReason is not null);

            succeeded.CreatedResources.ShouldHaveSingleItem();
            failed.CreatedResources.ShouldBeEmpty();
            failed.FailureReason.ShouldNotBeNullOrWhiteSpace();
        }
        finally
        {
            TestBearerTokenHandler.ClearToken();
        }
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Use_Resource_Type_Name_When_BaseName_Is_Not_Provided(CancellationToken cancellationToken)
    {
        var (locationId, tagId, verifiableToken) = await SeedLocationWithResourceTypeTagAsync(
            OrganizationTagTypeConstants.ResourceRoom, "Meeting Room", cancellationToken);

        TestBearerTokenHandler.SetToken(verifiableToken);
        try
        {
            IReadOnlyList<BulkAddResourceRowInput> rows =
            [
                new()
                {
                    OrganizationResourceTypeTagId = tagId,
                    BaseName = null,
                    Quantity = 2,
                    CustomTagIds = [],
                    ZoneIds = [],
                    ProductTagIds = []
                }
            ];

            var result = await bulkAddResourcesMutation.ExecuteAsync(locationId, rows, cancellationToken);

            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();
            var payload = result.Data.BulkAddResources;
            payload.Results.ShouldHaveSingleItem();
            payload.Results[0].FailureReason.ShouldBeNull();

            var names = payload.Results[0].CreatedResources.Select(r => r.Name).OrderBy(n => n).ToList();
            names.ShouldBe(["Meeting Room-1", "Meeting Room-2"]);
        }
        finally
        {
            TestBearerTokenHandler.ClearToken();
        }
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Append_After_Highest_Suffix_Not_Fill_Gaps(CancellationToken cancellationToken)
    {
        var (locationId, tagId, verifiableToken) = await SeedLocationWithResourceTypeTagAsync(
            OrganizationTagTypeConstants.ResourceDesk, "Desk", cancellationToken);

        // Pre-seed Desk-1 and Desk-3 (gap at Desk-2)
        await SeedResourceAsync(locationId, "Desk-1", OrganizationTagTypeConstants.ResourceDesk, cancellationToken);
        await SeedResourceAsync(locationId, "Desk-3", OrganizationTagTypeConstants.ResourceDesk, cancellationToken);

        TestBearerTokenHandler.SetToken(verifiableToken);
        try
        {
            IReadOnlyList<BulkAddResourceRowInput> rows =
            [
                new()
                {
                    OrganizationResourceTypeTagId = tagId,
                    BaseName = "Desk",
                    Quantity = 2,
                    CustomTagIds = [],
                    ZoneIds = [],
                    ProductTagIds = []
                }
            ];

            var result = await bulkAddResourcesMutation.ExecuteAsync(locationId, rows, cancellationToken);

            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();
            var payload = result.Data.BulkAddResources;
            payload.Results.ShouldHaveSingleItem();
            payload.Results[0].FailureReason.ShouldBeNull();

            // Should append Desk-4, Desk-5 (not fill Desk-2 gap)
            var names = payload.Results[0].CreatedResources.Select(r => r.Name).OrderBy(n => n).ToList();
            names.ShouldBe(["Desk-4", "Desk-5"]);
        }
        finally
        {
            TestBearerTokenHandler.ClearToken();
        }
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Error_When_Total_Quantity_Exceeds_100(CancellationToken cancellationToken)
    {
        var (locationId, tagId, _) = await SeedLocationWithResourceTypeTagAsync(
            OrganizationTagTypeConstants.ResourceDesk, "Desk", cancellationToken);

        IReadOnlyList<BulkAddResourceRowInput> rows =
        [
            new()
            {
                OrganizationResourceTypeTagId = tagId,
                BaseName = "Desk",
                Quantity = 101,
                CustomTagIds = [],
                ZoneIds = [],
                ProductTagIds = []
            }
        ];

        var result = await bulkAddResourcesMutation.ExecuteAsync(locationId, rows, cancellationToken);

        // Mutation should fail due to total quantity exceeding 100
        var hasError = result.Errors is { Count: > 0 } || result.Data is null;
        hasError.ShouldBeTrue();

        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldNotBeEmpty();
        var errorMessage = result.Errors[0].Message;
        errorMessage.ShouldContain("exceeds the maximum");

        var dbResources = await repositoryFactory.ResourceRepository.GetActiveNamesByLocationIdAsync(locationId, cancellationToken);
        dbResources.ShouldBeEmpty();
    }
}
