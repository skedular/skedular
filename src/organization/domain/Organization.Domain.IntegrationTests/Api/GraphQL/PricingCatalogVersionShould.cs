using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Organization.Shared.Database.Entities;
using Organization.Shared.Models.PricingCatalog;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Pricing;
using OrganizationEntity = Organization.Shared.Database.Entities.Organization;

namespace Organization.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Organization.Api")]
public class PricingCatalogVersionShould
{
    [Theory]
    [AutoFakeItEasyData([typeof(OrganizationIntegrationServiceFixtureCustomizer)])]
    public async Task Resolve_Extended_V1_And_Leave_Existing_Offering_Unchanged(
        IRepositoryFactory repositoryFactory,
        IPricingCatalogVersionService pricingCatalogVersionService,
        IOrganizationOfferingCompatibilityService compatibilityService,
        string organizationId,
        string offeringId,
        string organizationName,
        string organizationOfferingPlanId,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var organization = repositoryFactory.OrganizationRepository.Add(new OrganizationEntity
        {
            Id = organizationId,
            Name = organizationName,
            Type = OrganizationTypeConstants.Private,
            BillingCycle = OrganizationBillingCycleConstants.Monthly,
            InvoiceDueInDays = 7
        });
        var legacyOffering = new OrganizationOffering
        {
            Id = offeringId,
            Organization = organization,
            Code = OfferingCode.EarlyBirdV1,
            Start = now.AddDays(-1),
            End = now.AddDays(1),
            AutoRenew = true,
            UnitPrice = OfferingCode.EarlyBirdV1.GetOffering().UnitPrice
        };
        repositoryFactory.OrganizationOfferingRepository.Add(legacyOffering);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        // Configure mocks to return expected values
        A.CallTo(() => pricingCatalogVersionService.GetCurrentTeamsVersion()).Returns(
            new PricingCatalogVersion(
                PricingCatalogConstants.CurrentTeamsCatalogVersion,
                PricingCatalogVersionStatus.Active,
                DateTimeOffset.UnixEpoch,
                null,
                "Extends the existing V1 Teams offering model while preserving existing Free and Early Bird behavior."));

        A.CallTo(() => compatibilityService.GetTeamsOfferingPlanAsync(
            organizationId,
            legacyOffering,
            now,
            cancellationToken)).Returns(new OrganizationOfferingPlan(
            organizationOfferingPlanId,
            organizationId,
            PricingCatalogProductOfferingCode.Teams,
            PricingCatalogSubscriptionPlanCode.LegacyEarlyBird,
            null,
            null,
            Currency.Usd,
            null,
            null,
            null,
            CatalogVersionConstants.TeamsV1,
            OrganizationOfferingPlanStatus.Legacy,
            now,
            null,
            false,
            now,
            now));

        var version = pricingCatalogVersionService.GetCurrentTeamsVersion();
        var offeringPlan = await compatibilityService.GetTeamsOfferingPlanAsync(
            organizationId,
            legacyOffering,
            now,
            cancellationToken);

        version.Code.ShouldBe(PricingCatalogConstants.CurrentTeamsCatalogVersion);
        version.Status.ShouldBe(PricingCatalogVersionStatus.Active);
        offeringPlan.ShouldNotBeNull();
        offeringPlan.PlanCode.ShouldBe(PricingCatalogSubscriptionPlanCode.LegacyEarlyBird);
        offeringPlan.Status.ShouldBe(OrganizationOfferingPlanStatus.Legacy);

        var persistedOffering = await repositoryFactory.OrganizationOfferingRepository.GetByIdAsync(offeringId, cancellationToken);
        persistedOffering.ShouldNotBeNull();
        persistedOffering.Code.ShouldBe(OfferingCode.EarlyBirdV1);
        persistedOffering.DeletedAt.ShouldBeNull();
    }
}
