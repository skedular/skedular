using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Organization.Shared.Database.Entities;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Shared.Services.Pricing;

public static class OrganizationOfferingPricingExtensions
{
    extension(OrganizationOffering organizationOffering)
    {
        public void ApplyOfferingTemplate(OfferingCode offeringCode)
        {
            var offering = offeringCode.GetOffering();

            organizationOffering.Code = offeringCode;
            organizationOffering.UnitPrice = offering.UnitPrice;
            organizationOffering.FixedPrice = offeringCode.IsPayAsYouGoOffering() ? null : 0;
            organizationOffering.Currency = PricingCatalogConstants.SkedularPricingCurrency;
            organizationOffering.PurchasedUserCapacity = offering.MaxUserCount;
            organizationOffering.PurchasedLocationCapacity = offering.MaxLocationCount;
            organizationOffering.PurchasedTeamCapacity = offering.MaxTeamCount;
            organizationOffering.CatalogVersion = PricingCatalogConstants.CurrentTeamsCatalogVersion;
        }

        /// <summary>
        ///     Applies an offering template for a renewal or update of an existing offering.
        ///     Preserves the existing currency from the current offering instead of overriding it.
        /// </summary>
        public void ApplyRenewalTemplate(OfferingCode offeringCode)
        {
            var offering = offeringCode.GetOffering();

            organizationOffering.Code = offeringCode;
            organizationOffering.UnitPrice = offering.UnitPrice;
            organizationOffering.FixedPrice = offeringCode.IsPayAsYouGoOffering() ? null : 0;
            // Preserve existing currency instead of overriding with PricingCatalogConstants.SkedularPricingCurrency
            //organizationOffering.Currency = PricingCatalogConstants.SkedularPricingCurrency;
            organizationOffering.PurchasedUserCapacity = offering.MaxUserCount;
            organizationOffering.PurchasedLocationCapacity = offering.MaxLocationCount;
            organizationOffering.PurchasedTeamCapacity = offering.MaxTeamCount;
            organizationOffering.CatalogVersion = PricingCatalogConstants.CurrentTeamsCatalogVersion;
        }

        public void ApplyNegotiatedEnterpriseTerms(
            int fixedPrice,
            Currency currency,
            int purchasedUserCapacity,
            int purchasedLocationCapacity,
            int purchasedTeamCapacity)
        {
            organizationOffering.ApplyOfferingTemplate(OfferingCode.EnterpriseCustomV1);
            organizationOffering.UnitPrice = null;
            organizationOffering.FixedPrice = fixedPrice;
            organizationOffering.PurchasedUserCapacity = purchasedUserCapacity;
            organizationOffering.PurchasedLocationCapacity = purchasedLocationCapacity;
            organizationOffering.PurchasedTeamCapacity = purchasedTeamCapacity;
            organizationOffering.Currency = currency.ToCurrency();
        }
    }
}
