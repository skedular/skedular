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
            organizationOffering.FixedPrice = offering.FixedPrice;
            organizationOffering.Currency = offering.Currency.ToCurrency();
            organizationOffering.PurchasedUserCapacity = offering.MaxUserCount;
            organizationOffering.PurchasedLocationCapacity = offering.MaxLocationCount;
            organizationOffering.PurchasedTeamCapacity = offeringCode.GetDefaultPurchasedTeamCapacity();
            organizationOffering.CatalogVersion = offeringCode.GetCurrentCatalogVersion();
            organizationOffering.HostCommissionPercentage = offering.HostCommissionPercentage;
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
            organizationOffering.FixedPrice = offering.FixedPrice;
            organizationOffering.PurchasedUserCapacity = offering.MaxUserCount;
            organizationOffering.PurchasedLocationCapacity = offering.MaxLocationCount;
            organizationOffering.PurchasedTeamCapacity = offeringCode.GetDefaultPurchasedTeamCapacity();
            organizationOffering.CatalogVersion = offeringCode.GetCurrentCatalogVersion();
            organizationOffering.HostCommissionPercentage = offering.HostCommissionPercentage;
        }

        public void ApplyNegotiatedOfferingTerms(
            OfferingCode offeringCode,
            int fixedPrice,
            Currency currency,
            int? purchasedUserCapacity,
            int? purchasedLocationCapacity,
            int? purchasedTeamCapacity,
            int? monthlyBookingInstanceQuota,
            int? discountPercentage)
        {
            organizationOffering.ApplyOfferingTemplate(offeringCode);
            organizationOffering.UnitPrice = null;
            organizationOffering.FixedPrice = fixedPrice;
            organizationOffering.PurchasedUserCapacity = purchasedUserCapacity ?? organizationOffering.PurchasedUserCapacity;
            organizationOffering.PurchasedLocationCapacity = purchasedLocationCapacity ?? organizationOffering.PurchasedLocationCapacity;
            organizationOffering.PurchasedTeamCapacity =
                monthlyBookingInstanceQuota ?? purchasedTeamCapacity ?? organizationOffering.PurchasedTeamCapacity;
            organizationOffering.Currency = currency.ToCurrency();
            organizationOffering.DiscountPercentage = discountPercentage ?? 0;
        }

        public int GetBillingAmount()
        {
            if (organizationOffering.Code.IsEarlyBirdOffering())
            {
                return 0;
            }

            var totalCost = organizationOffering.FixedPrice ??
                            organizationOffering.OrganizationOfferingActiveMembers.Count *
                            (organizationOffering.UnitPrice ??
                             throw new InvalidOperationException(
                                 "Organization offering requires either a fixed price or unit price before billing."));
            return organizationOffering.DiscountPercentage switch
            {
                0 => totalCost,
                100 => 0,
                var discountPercentage => totalCost * (100 - discountPercentage) / 100,
            };
        }
    }

    extension(OfferingCode offeringCode)
    {
        private int? GetDefaultPurchasedTeamCapacity()
        {
            var offering = offeringCode.GetOffering();
            return offeringCode.IsSpacesOffering() ? offering.MaxBookingInstanceCount : offering.MaxTeamCount;
        }

        public string GetCurrentCatalogVersion() =>
            offeringCode switch
            {
                _ when offeringCode.IsSpacesOffering() => PricingCatalogConstants.CurrentSpacesCatalogVersion,
                _ when offeringCode.IsHostOffering() => PricingCatalogConstants.CurrentHostCatalogVersion,
                _ => PricingCatalogConstants.CurrentTeamsCatalogVersion,
            };

        private bool IsSpacesOffering() =>
            offeringCode is OfferingCode.SpacesFreeTierV1
                or OfferingCode.SpacesGrowthV1
                or OfferingCode.SpacesBusinessV1
                or OfferingCode.SpacesContactUsV1;

        private bool IsHostOffering() =>
            offeringCode is OfferingCode.HostStandardV1;
    }
}
