using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Organization.Api.Services.Pricing;
using Organization.Shared.Database.Entities;
using Organization.Shared.Mappers;
using Organization.Shared.Models.PricingCatalog;
using Organization.Shared.Repositories;
using SharedOffering = Api.Shared.Services.Models.Offering;

namespace Organization.Api.UnitTests.Services.Pricing.OrganizationSpacesSubscriptionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Legacy_Early_Bird_For_Marketplace_Organization(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        TimeProvider timeProvider,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        ISpacesAccessEvaluator spacesAccessEvaluator,
        OrganizationSpacesSubscriptionService sut,
        string organizationId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = organizationId,
            Type = OrganizationTypeConstants.Marketplace,
        };
        var offering = new OrganizationOffering
        {
            Id = "legacy-early-bird",
            Code = OfferingCode.EarlyBirdV1,
            Start = now.AddDays(-1),
            End = now.AddDays(1),
            CatalogVersion = PricingCatalogConstants.CurrentSpacesCatalogVersion,
            Organization = organization,
        };
        organization.OrganizationOfferings = [offering];

        A.CallTo(() => repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                organizationId,
                null,
                cancellationToken))
            .Returns(organization);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => entityMapper.MapTo(organization)).Returns(new Shared.Models.Organization
        {
            Id = organizationId,
        });
        A.CallTo(() => spacesAccessEvaluator.Evaluate(now, A<SharedOffering>._, SpacesAccessAction.Read))
            .Returns(new SpacesAccessEvaluator().Evaluate(now,
                new SharedOffering
                {
                    Id = offering.Id,
                    Code = offering.Code,
                    Start = offering.Start,
                    End = offering.End,
                    SpacesProductEnabled = true,
                }, SpacesAccessAction.Read));

        var result = await sut.GetAsync(organizationId, cancellationToken);

        result.ShouldNotBeNull();
        result.PlanCode.ShouldBe(PricingCatalogSubscriptionPlanCode.LegacyEarlyBird);
        result.CommercialModel.ShouldBe(PricingCatalogCommercialModel.Free);
        result.UsageLimit.ShouldBeNull();
        result.CatalogVersion.ShouldBe(PricingCatalogConstants.CurrentSpacesCatalogVersion);
    }
}
