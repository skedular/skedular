using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Organization.Domain.IntegrationTests.Skedular.GraphQL.V1;
using Organization.Shared.Database.Entities;
using Organization.Shared.Repositories;
using OrganizationEntity = Organization.Shared.Database.Entities.Organization;

namespace Organization.Domain.IntegrationTests.Api.GraphQL.OrganizationMutationTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Organization.Api")]
public class UpdateOrganizationShould
{
    [Theory]
    [AutoFakeItEasyData([typeof(OrganizationIntegrationServiceFixtureCustomizer)])]
    public async Task Update_Selected_Name_And_Preserve_Omitted_Website(
        IUpdateOrganizationMutation updateOrganizationMutation,
        IRepositoryFactory repositoryFactory,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string offeringId,
        string originalName,
        string updatedName,
        string websiteSeed,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var website = $"https://{websiteSeed}.example";
        await SeedOwnedOrganizationAsync(
            repositoryFactory,
            organizationId,
            customerId,
            identityId,
            memberId,
            offeringId,
            originalName,
            website,
            timeProvider,
            cancellationToken);

        TestBearerTokenHandler.SetToken(identityId);
        try
        {
            var result = await updateOrganizationMutation.ExecuteAsync(
                organizationId,
                [OrganizationPatchField.Name],
                updatedName,
                null,
                cancellationToken);

            result.Errors.Select(error => error.Message).ShouldBeEmpty();
            result.Data.ShouldNotBeNull();
            result.Data.UpdateOrganization.Organization.Name.ShouldBe(updatedName);
            result.Data.UpdateOrganization.Organization.Website.ShouldBe(website);

            var organization = await GetPersistedOrganizationAsync(repositoryFactory, organizationId, cancellationToken);
            organization.Name.ShouldBe(updatedName);
            organization.Website.ShouldBe(website);
        }
        finally
        {
            TestBearerTokenHandler.ClearToken();
        }
    }

    [Theory]
    [AutoFakeItEasyData([typeof(OrganizationIntegrationServiceFixtureCustomizer)])]
    public async Task Return_Current_Organization_For_A_No_Op_Name_Update(
        IUpdateOrganizationMutation updateOrganizationMutation,
        IRepositoryFactory repositoryFactory,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string offeringId,
        string name,
        string websiteSeed,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var website = $"https://{websiteSeed}.example";
        await SeedOwnedOrganizationAsync(
            repositoryFactory,
            organizationId,
            customerId,
            identityId,
            memberId,
            offeringId,
            name,
            website,
            timeProvider,
            cancellationToken);

        TestBearerTokenHandler.SetToken(identityId);
        try
        {
            var result = await updateOrganizationMutation.ExecuteAsync(
                organizationId,
                [OrganizationPatchField.Name],
                name,
                null,
                cancellationToken);

            result.Errors.Select(error => error.Message).ShouldBeEmpty();
            result.Data.ShouldNotBeNull();
            result.Data.UpdateOrganization.Organization.Name.ShouldBe(name);
            result.Data.UpdateOrganization.Organization.Website.ShouldBe(website);

            var organization = await GetPersistedOrganizationAsync(repositoryFactory, organizationId, cancellationToken);
            organization.Name.ShouldBe(name);
            organization.Website.ShouldBe(website);
        }
        finally
        {
            TestBearerTokenHandler.ClearToken();
        }
    }

    [Theory]
    [AutoFakeItEasyData([typeof(OrganizationIntegrationServiceFixtureCustomizer)])]
    public async Task Reject_Empty_Field_Selection_Without_Changing_Organization(
        IUpdateOrganizationMutation updateOrganizationMutation,
        IRepositoryFactory repositoryFactory,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string offeringId,
        string originalName,
        string updatedName,
        string websiteSeed,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var website = $"https://{websiteSeed}.example";
        await SeedOwnedOrganizationAsync(
            repositoryFactory,
            organizationId,
            customerId,
            identityId,
            memberId,
            offeringId,
            originalName,
            website,
            timeProvider,
            cancellationToken);

        TestBearerTokenHandler.SetToken(identityId);
        try
        {
            var result = await updateOrganizationMutation.ExecuteAsync(
                organizationId,
                [],
                updatedName,
                null,
                cancellationToken);

            result.Errors.Select(error => error.Message).ShouldNotBeEmpty();

            var organization = await GetPersistedOrganizationAsync(repositoryFactory, organizationId, cancellationToken);
            organization.Name.ShouldBe(originalName);
            organization.Website.ShouldBe(website);
        }
        finally
        {
            TestBearerTokenHandler.ClearToken();
        }
    }

    [Theory]
    [AutoFakeItEasyData([typeof(OrganizationIntegrationServiceFixtureCustomizer)])]
    public async Task Preserve_Concurrent_Selected_Field_Updates(
        IUpdateOrganizationMutation updateOrganizationMutation,
        IRepositoryFactory repositoryFactory,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string offeringId,
        string originalName,
        string updatedName,
        string websiteUpdateSeed,
        string websiteSeed,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var website = $"https://{websiteSeed}.example";
        var updatedWebsite = $"https://{websiteUpdateSeed}.example";
        await SeedOwnedOrganizationAsync(
            repositoryFactory,
            organizationId,
            customerId,
            identityId,
            memberId,
            offeringId,
            originalName,
            website,
            timeProvider,
            cancellationToken);

        TestBearerTokenHandler.SetToken(identityId);
        try
        {
            var nameResultTask = updateOrganizationMutation.ExecuteAsync(
                organizationId,
                [OrganizationPatchField.Name],
                updatedName,
                null,
                cancellationToken);
            var websiteResultTask = updateOrganizationMutation.ExecuteAsync(
                organizationId,
                [OrganizationPatchField.Website],
                null,
                updatedWebsite,
                cancellationToken);

            var results = await Task.WhenAll(nameResultTask, websiteResultTask);

            results[0].Errors.Select(error => error.Message).ShouldBeEmpty();
            results[1].Errors.Select(error => error.Message).ShouldBeEmpty();

            var organization = await GetPersistedOrganizationAsync(repositoryFactory, organizationId, cancellationToken);
            organization.Name.ShouldBe(updatedName);
            organization.Website.ShouldBe(updatedWebsite);
        }
        finally
        {
            TestBearerTokenHandler.ClearToken();
        }
    }

    private static async Task SeedOwnedOrganizationAsync(
        IRepositoryFactory repositoryFactory,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string offeringId,
        string name,
        string website,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var organization = repositoryFactory.OrganizationRepository.Add(new OrganizationEntity
        {
            Id = organizationId,
            Name = name,
            Website = website,
            Type = OrganizationTypeConstants.Private,
            BillingCycle = OrganizationBillingCycleConstants.Monthly,
            InvoiceDueInDays = 7,
        });
        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, cancellationToken);

        repositoryFactory.IdentityRepository.Add(new Identity
        {
            Id = identityId,
            Customer = customer,
        });
        repositoryFactory.OrganizationMemberRepository.Add(new OrganizationMember
        {
            Id = memberId,
            Organization = organization,
            Customer = customer,
            Role = OrganizationMemberRoleConstants.Owner,
            Status = OrganizationMemberStatusConstants.Active,
        });
        repositoryFactory.OrganizationOfferingRepository.Add(new OrganizationOffering
        {
            Id = offeringId,
            Organization = organization,
            Code = OfferingCode.FreeTierV1,
            Start = timeProvider.GetUtcNow().AddDays(-1),
            End = timeProvider.GetUtcNow().AddDays(1),
            AutoRenew = true,
            UnitPrice = OfferingCode.FreeTierV1.GetOffering().UnitPrice,
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static async Task<OrganizationEntity> GetPersistedOrganizationAsync(
        IRepositoryFactory repositoryFactory,
        string organizationId,
        CancellationToken cancellationToken) =>
        await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainUntrackedAsync(organizationId, null, cancellationToken) ??
        throw new InvalidOperationException($"Organization {organizationId} was not persisted.");
}
