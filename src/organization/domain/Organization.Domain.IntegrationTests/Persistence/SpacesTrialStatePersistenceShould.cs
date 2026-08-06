using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Organization.Shared.Database.Entities;
using Organization.Shared.Repositories;
using OrganizationEntity = Organization.Shared.Database.Entities.Organization;

namespace Organization.Domain.IntegrationTests.Persistence;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Organization.Api")]
public class SpacesTrialStatePersistenceShould
{
    [Theory]
    [InlineAutoFakeItEasyData(
        new[] { typeof(OrganizationIntegrationServiceFixtureCustomizer) },
        OrganizationTypeConstants.Marketplace,
        OfferingCode.SpacesFreeTierV1,
        true)]
    [InlineAutoFakeItEasyData(
        new[] { typeof(OrganizationIntegrationServiceFixtureCustomizer) },
        OrganizationTypeConstants.Marketplace,
        OfferingCode.SpacesGrowthV1,
        true)]
    [InlineAutoFakeItEasyData(
        new[] { typeof(OrganizationIntegrationServiceFixtureCustomizer) },
        OrganizationTypeConstants.Private,
        OfferingCode.PayAsYouGoV1,
        false)]
    public async Task Round_Trip_Trial_And_Billing_State_Through_Repositories(
        string organizationType,
        OfferingCode offeringCode,
        bool hasSpacesTrial,
        IRepositoryFactory repositoryFactory,
        string organizationId,
        string offeringId,
        string organizationName,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var trialStartedAt = hasSpacesTrial ? now.AddDays(-1) : (DateTimeOffset?)null;
        var billingStartsAt = offeringCode == OfferingCode.SpacesGrowthV1 ? now.AddDays(1) : (DateTimeOffset?)null;
        var organization = repositoryFactory.OrganizationRepository.Add(new OrganizationEntity
        {
            Id = organizationId,
            Name = organizationName,
            Type = organizationType,
            BillingCycle = OrganizationBillingCycleConstants.Monthly,
            InvoiceDueInDays = 7,
            SpacesTrialStartedAt = trialStartedAt,
        });
        repositoryFactory.OrganizationOfferingRepository.Add(new OrganizationOffering
        {
            Id = offeringId,
            Organization = organization,
            Code = offeringCode,
            Start = now,
            End = now.AddMonths(1),
            Currency = CurrencyConstants.Usd,
            SpacesBillingStartsAt = billingStartsAt,
        });
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var persisted = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainUntrackedAsync(
            organizationId,
            null,
            cancellationToken);

        persisted.ShouldNotBeNull();
        persisted.SpacesTrialStartedAt.ShouldBe(trialStartedAt);
        persisted.OrganizationOfferings.Single().SpacesBillingStartsAt.ShouldBe(billingStartsAt);
    }
}
