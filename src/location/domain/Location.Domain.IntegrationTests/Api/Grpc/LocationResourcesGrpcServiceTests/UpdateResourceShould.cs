using Api.Shared.Grpc.Skedular.Location.Resources.V1;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Grpc;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using LocationEntity = Location.Shared.Database.Entities.Location;
using LocationGrpcConfig = Api.Shared.Services.Configurations.Grpc.LocationConfiguration;
using Offering = Api.Shared.Services.Models.Offering;

namespace Location.Domain.IntegrationTests.Api.Grpc.LocationResourcesGrpcServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Location.Api")]
public class UpdateResourceShould(
    LocationResourcesService.LocationResourcesServiceClient locationResourcesServiceClient,
    IRepositoryFactory repositoryFactory,
    LocationGrpcConfig locationConfiguration,
    TimeProvider timeProvider)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Omitted_Details(
        string resourceId,
        string locationId,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string originalName,
        string updatedName,
        string originalColor,
        string tagId,
        CancellationToken cancellationToken)
    {
        await SeedResourceAsync(
            resourceId,
            locationId,
            organizationId,
            customerId,
            identityId,
            memberId,
            originalName,
            originalColor,
            tagId,
            cancellationToken);

        var result = await locationResourcesServiceClient.UpdateResourceAsync(
            new UpdateResourceInput { Id = resourceId, Name = updatedName, FieldsToUpdate = { ResourcePatchField.Name } },
            locationConfiguration.ApiKey.CreateMetadata(identityId),
            cancellationToken: cancellationToken);

        result.ShouldNotBeNull();
        result.Name.ShouldBe(updatedName);

        var resources = await repositoryFactory.ResourceRepository.GetByIdsWithOrganizationTagsUntrackedAsync([resourceId], cancellationToken);
        var resource = resources.ShouldHaveSingleItem();
        resource.Name.ShouldBe(updatedName);
        resource.Color.ShouldBe(originalColor[..Math.Min(originalColor.Length, 32)]);
    }

    private async Task SeedResourceAsync(
        string resourceId,
        string locationId,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string name,
        string color,
        string tagId,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        organization.Offering = new Offering
        {
            Id = organizationId, Code = OfferingCode.EnterpriseCustomV1, Start = now.AddDays(-1), End = now.AddDays(1)
        };

        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, cancellationToken);

        repositoryFactory.IdentityRepository.Add(new Identity { Id = identityId, Customer = customer });
        repositoryFactory.OrganizationMemberRepository.Add(new OrganizationMember
        {
            Id = memberId,
            Organization = organization,
            Customer = customer,
            Role = OrganizationMemberRoleConstants.Owner,
            Status = OrganizationMemberStatusConstants.Active
        });

        var location = new LocationEntity
        {
            Id = locationId, Organization = organization, Name = "test-location", Type = LocationTypeConstants.Private
        };
        repositoryFactory.LocationRepository.Add(location);

        var resourceTypeTag = repositoryFactory.OrganizationTagRepository.Add(new OrganizationTag
        {
            Id = tagId, Organization = organization, Type = OrganizationTagTypeConstants.ResourceDesk, Name = "Desk"
        });

        repositoryFactory.ResourceRepository.Add(new Resource
        {
            Id = resourceId,
            Location = location,
            Name = name,
            Color = color[..Math.Min(color.Length, 32)],
            Capacity = 1,
            OrganizationTags = [resourceTypeTag]
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
