using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Organization.Shared.Models.PricingCatalog;
using Organization.Shared.Services.Pricing;
using OrganizationOffering = Organization.Shared.Database.Entities.OrganizationOffering;

namespace Organization.Shared.Mappers;

public interface ILegacyOfferingCompatibilityMapper
{
    OrganizationOfferingPlan MapToReadOnlyOfferingPlan(OrganizationOffering organizationOffering);
}

public class LegacyOfferingCompatibilityMapper : ILegacyOfferingCompatibilityMapper
{
    public OrganizationOfferingPlan MapToReadOnlyOfferingPlan(OrganizationOffering organizationOffering) =>
        new(
            organizationOffering.Id,
            organizationOffering.Organization.Id,
            PricingCatalogProductOfferingCode.Teams,
            organizationOffering.Code.ToPricingCatalogSubscriptionPlanCode(),
            organizationOffering.UnitPrice,
            organizationOffering.FixedPrice,
            organizationOffering.Currency.ToCurrency(),
            organizationOffering.PurchasedUserCapacity,
            organizationOffering.PurchasedLocationCapacity,
            organizationOffering.PurchasedTeamCapacity,
            organizationOffering.CatalogVersion ?? PricingCatalogConstants.CurrentTeamsCatalogVersion,
            organizationOffering.Code.IsEarlyBirdOffering() ? OrganizationOfferingPlanStatus.Legacy : OrganizationOfferingPlanStatus.Active,
            organizationOffering.Start,
            organizationOffering.End,
            organizationOffering.AutoRenew,
            organizationOffering.CreatedAt,
            organizationOffering.ModifiedAt ?? organizationOffering.CreatedAt);
}
