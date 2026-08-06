using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Organization.Shared.Database.Entities;
using Organization.Shared.Mappers;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Api.UnitTests.Mappers.LegacyOfferingCompatibilityMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MapToReadOnlyOfferingPlanShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Preserve_Early_Bird_As_Read_Only_Legacy_Offering_Plan(LegacyOfferingCompatibilityMapper sut)
    {
        var createdAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z");
        var organizationOffering = new OrganizationOffering
        {
            Id = "legacy-offering-1",
            Code = OfferingCode.EarlyBirdV1,
            Start = createdAt,
            End = createdAt.AddMonths(1),
            AutoRenew = true,
            CreatedAt = createdAt,
            UnitPrice = null,
            Organization = new Shared.Database.Entities.Organization
            {
                Id = "organization-1",
            },
            Currency = CurrencyConstants.Usd,
        };

        var result = sut.MapToReadOnlyOfferingPlan(organizationOffering);

        result.Id.ShouldBe(organizationOffering.Id);
        result.OrganizationId.ShouldBe("organization-1");
        result.ProductOfferingCode.ShouldBe(PricingCatalogProductOfferingCode.Teams);
        result.PlanCode.ShouldBe(PricingCatalogSubscriptionPlanCode.LegacyEarlyBird);
        result.Status.ShouldBe(OrganizationOfferingPlanStatus.Legacy);
        result.EffectiveFrom.ShouldBe(organizationOffering.Start);
        result.EffectiveUntil.ShouldBe(organizationOffering.End);
        result.AutoRenew.ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Preserve_Free_Offering_As_Active_Free_Offering_Plan(LegacyOfferingCompatibilityMapper sut)
    {
        var createdAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z");
        var organizationOffering = new OrganizationOffering
        {
            Id = "free-offering-1",
            Code = OfferingCode.FreeTierV1,
            Start = createdAt,
            End = createdAt.AddMonths(1),
            AutoRenew = true,
            CreatedAt = createdAt,
            UnitPrice = null,
            Organization = new Shared.Database.Entities.Organization
            {
                Id = "organization-1",
            },
            Currency = CurrencyConstants.Usd,
        };

        var result = sut.MapToReadOnlyOfferingPlan(organizationOffering);

        result.ProductOfferingCode.ShouldBe(PricingCatalogProductOfferingCode.Teams);
        result.PlanCode.ShouldBe(PricingCatalogSubscriptionPlanCode.Free);
        result.Status.ShouldBe(OrganizationOfferingPlanStatus.Active);
        result.PurchasedUserCapacity.ShouldBeNull();
    }
}
