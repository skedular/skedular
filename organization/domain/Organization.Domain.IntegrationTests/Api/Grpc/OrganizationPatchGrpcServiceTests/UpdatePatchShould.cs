using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Api.Shared.Grpc.Skedular.Organization.Tags.V1;
using Api.Shared.Grpc.Skedular.Organization.Zones.V1;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Grpc;
using Organization.Shared.Repositories;
using GrpcOrganizationConfiguration = Api.Shared.Services.Configurations.Grpc.OrganizationConfiguration;
using BillingDetailsEntity = Organization.Shared.Database.Entities.OrganizationBillingDetails;
using Constants = Api.Shared.Services.Constants;
using IdentityEntity = Organization.Shared.Database.Entities.Identity;
using OrganizationMemberEntity = Organization.Shared.Database.Entities.OrganizationMember;
using OrganizationOfferingEntity = Organization.Shared.Database.Entities.OrganizationOffering;
using OrganizationEntity = Organization.Shared.Database.Entities.Organization;
using TagEntity = Organization.Shared.Database.Entities.Tag;

namespace Organization.Domain.IntegrationTests.Api.Grpc.OrganizationPatchGrpcServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Organization.Api")]
public class UpdatePatchShould
{
    [Theory]
    [AutoFakeItEasyData([typeof(OrganizationIntegrationServiceFixtureCustomizer)])]
    public async Task Update_Billing_Company_Name_And_Preserve_Omitted_Email(
        OrganizationBillingService.OrganizationBillingServiceClient organizationBillingServiceClient,
        IRepositoryFactory repositoryFactory,
        GrpcOrganizationConfiguration organizationConfiguration,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string offeringId,
        string billingDetailsId,
        string oldCompanyName,
        string newCompanyName,
        string emailPrefix,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var organization = await SeedOwnedOrganizationAsync(
            repositoryFactory,
            organizationId,
            customerId,
            identityId,
            memberId,
            offeringId,
            timeProvider,
            cancellationToken);
        var email = $"{emailPrefix}@example.test";
        repositoryFactory.OrganizationBillingDetailsRepository.Add(new BillingDetailsEntity
        {
            Id = billingDetailsId,
            Organization = organization,
            CompanyName = oldCompanyName,
            Email = email,
            AddressLine1 = "1 Example Street",
            Zipcode = "1010",
            Country = "New Zealand"
        });
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var result = await organizationBillingServiceClient.UpdateBillingDetailsAsync(
            new UpdateBillingDetailsInput
            {
                OrganizationId = organization.Id, CompanyName = newCompanyName, FieldsToUpdate = { BillingDetailsPatchField.CompanyName }
            },
            organizationConfiguration.ApiKey.CreateMetadata(identityId),
            cancellationToken: cancellationToken);

        result.CompanyName.ShouldBe(newCompanyName);
        result.Email.ShouldBe(email);

        var billingDetails = await repositoryFactory.OrganizationBillingDetailsRepository.GetByIdUntrackedAsync(
                                 billingDetailsId,
                                 cancellationToken) ??
                             throw new InvalidOperationException($"Billing details {billingDetailsId} were not persisted.");
        billingDetails.CompanyName.ShouldBe(newCompanyName);
        billingDetails.Email.ShouldBe(email);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(OrganizationIntegrationServiceFixtureCustomizer)])]
    public async Task Update_Tag_Name_And_Preserve_Omitted_Description(
        OrganizationTagsService.OrganizationTagsServiceClient organizationTagsServiceClient,
        IRepositoryFactory repositoryFactory,
        GrpcOrganizationConfiguration organizationConfiguration,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string offeringId,
        string tagId,
        string oldName,
        string newName,
        string description,
        string color,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var storedColor = color[..Math.Min(color.Length, Constants.MaxColorValueLength)];
        var tag = await SeedOwnedTagAsync(
            repositoryFactory,
            organizationId,
            customerId,
            identityId,
            memberId,
            offeringId,
            tagId,
            oldName,
            description,
            storedColor,
            OrganizationTagTypeConstants.Custom,
            timeProvider,
            cancellationToken);

        var result = await organizationTagsServiceClient.UpdateTagAsync(
            new UpdateTagInput { Id = tag.Id, Name = newName, FieldsToUpdate = { TagPatchField.Name } },
            organizationConfiguration.ApiKey.CreateMetadata(identityId),
            cancellationToken: cancellationToken);

        result.Name.ShouldBe(newName);
        result.Description.ShouldBe(description);

        var persistedTag = await GetPersistedTagAsync(repositoryFactory, tag.Id, cancellationToken);
        persistedTag.Name.ShouldBe(newName);
        persistedTag.Description.ShouldBe(description);
        persistedTag.Color.ShouldBe(storedColor);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(OrganizationIntegrationServiceFixtureCustomizer)])]
    public async Task Update_Zone_Description_And_Preserve_Omitted_Name(
        OrganizationZonesService.OrganizationZonesServiceClient organizationZonesServiceClient,
        IRepositoryFactory repositoryFactory,
        GrpcOrganizationConfiguration organizationConfiguration,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string offeringId,
        string zoneId,
        string name,
        string oldDescription,
        string newDescription,
        string color,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var storedColor = color[..Math.Min(color.Length, Constants.MaxColorValueLength)];
        var zone = await SeedOwnedTagAsync(
            repositoryFactory,
            organizationId,
            customerId,
            identityId,
            memberId,
            offeringId,
            zoneId,
            name,
            oldDescription,
            storedColor,
            OrganizationTagTypeConstants.Zone,
            timeProvider,
            cancellationToken);

        var result = await organizationZonesServiceClient.UpdateZoneAsync(
            new UpdateZoneInput { Id = zone.Id, Description = newDescription, FieldsToUpdate = { ZonePatchField.Description } },
            organizationConfiguration.ApiKey.CreateMetadata(identityId),
            cancellationToken: cancellationToken);

        result.Name.ShouldBe(name);
        result.Description.ShouldBe(newDescription);

        var persistedZone = await GetPersistedTagAsync(repositoryFactory, zone.Id, cancellationToken);
        persistedZone.Name.ShouldBe(name);
        persistedZone.Description.ShouldBe(newDescription);
        persistedZone.Color.ShouldBe(storedColor);
    }

    private static async Task<TagEntity> SeedOwnedTagAsync(
        IRepositoryFactory repositoryFactory,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string offeringId,
        string tagId,
        string name,
        string description,
        string color,
        string type,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var organization = await SeedOwnedOrganizationAsync(
            repositoryFactory,
            organizationId,
            customerId,
            identityId,
            memberId,
            offeringId,
            timeProvider,
            cancellationToken);
        var tag = repositoryFactory.TagRepository.Add(new TagEntity
        {
            Id = tagId,
            Organization = organization,
            Name = name,
            Description = description,
            Color = color,
            Type = type
        });
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return tag;
    }

    private static async Task<OrganizationEntity> SeedOwnedOrganizationAsync(
        IRepositoryFactory repositoryFactory,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string offeringId,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var organization = repositoryFactory.OrganizationRepository.Add(new OrganizationEntity
        {
            Id = organizationId,
            Name = "Patch gRPC organization",
            Type = OrganizationTypeConstants.Private,
            BillingCycle = OrganizationBillingCycleConstants.Monthly,
            InvoiceDueInDays = 7
        });
        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, cancellationToken);
        repositoryFactory.IdentityRepository.Add(new IdentityEntity { Id = identityId, Customer = customer });
        repositoryFactory.OrganizationMemberRepository.Add(new OrganizationMemberEntity
        {
            Id = memberId,
            Organization = organization,
            Customer = customer,
            Role = OrganizationMemberRoleConstants.Owner,
            Status = OrganizationMemberStatusConstants.Active
        });
        repositoryFactory.OrganizationOfferingRepository.Add(new OrganizationOfferingEntity
        {
            Id = offeringId,
            Organization = organization,
            Code = OfferingCode.FreeTierV1,
            Start = timeProvider.GetUtcNow().AddDays(-1),
            End = timeProvider.GetUtcNow().AddDays(1),
            AutoRenew = true,
            UnitPrice = OfferingCode.FreeTierV1.GetOffering().UnitPrice
        });
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return organization;
    }

    private static async Task<TagEntity> GetPersistedTagAsync(
        IRepositoryFactory repositoryFactory,
        string tagId,
        CancellationToken cancellationToken) =>
        await repositoryFactory.TagRepository.GetByIdUntrackedAsync(tagId, cancellationToken) ??
        throw new InvalidOperationException($"Tag {tagId} was not persisted.");
}
