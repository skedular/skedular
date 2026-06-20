using Organization.Shared.Models.PricingCatalog;
using Organization.Shared.Services.Pricing;

namespace Organization.Api.Services.Pricing;

public interface IPricingCatalogService
{
    PricingCatalog GetCatalog(PricingCatalogProductOfferingCode? productOfferingCode);
}

public class PricingCatalogService(
    IPricingCatalogVersionService pricingCatalogVersionService,
    TimeProvider timeProvider,
    ILogger<PricingCatalogService> logger) : IPricingCatalogService
{
    public PricingCatalog GetCatalog(PricingCatalogProductOfferingCode? productOfferingCode)
    {
        var offerings = new[]
        {
            TeamsPricingCatalogProvider.GetTeamsOffering(), SpacesPricingCatalogProvider.GetSpacesOffering(),
            HostPricingCatalogProvider.GetHostOffering()
        };
        var filteredOfferings = productOfferingCode is null or PricingCatalogProductOfferingCode.NotSet
            ? offerings
            : offerings.Where(offering => offering.Code == productOfferingCode.Value).ToArray();

        logger.LogInformation(
            "Pricing catalog filtered for product offering {ProductOfferingCode}; returned {ReturnedOfferingCount} of {TotalOfferingCount} offerings",
            productOfferingCode?.ToString() ?? "All",
            filteredOfferings.Length,
            offerings.Length);

        return new PricingCatalog(
            "skedular-pricing-catalog",
            GetActiveVersion(productOfferingCode),
            filteredOfferings,
            timeProvider.GetUtcNow());
    }

    private PricingCatalogVersion GetActiveVersion(PricingCatalogProductOfferingCode? productOfferingCode) =>
        productOfferingCode switch
        {
            PricingCatalogProductOfferingCode.Spaces => pricingCatalogVersionService.GetCurrentSpacesVersion(),
            PricingCatalogProductOfferingCode.Host => pricingCatalogVersionService.GetCurrentHostVersion(),
            _ => pricingCatalogVersionService.GetCurrentTeamsVersion()
        };
}
