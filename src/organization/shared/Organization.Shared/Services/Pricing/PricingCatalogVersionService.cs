using Organization.Shared.Models.PricingCatalog;

namespace Organization.Shared.Services.Pricing;

public interface IPricingCatalogVersionService
{
    PricingCatalogVersion GetCurrentTeamsVersion();
    PricingCatalogVersion GetCurrentSpacesVersion();
    PricingCatalogVersion GetCurrentHostVersion();
}

public class PricingCatalogVersionService : IPricingCatalogVersionService
{
    public PricingCatalogVersion GetCurrentTeamsVersion() =>
        new(
            PricingCatalogConstants.CurrentTeamsCatalogVersion,
            PricingCatalogVersionStatus.Active,
            DateTimeOffset.UnixEpoch,
            null,
            "Extends the existing V1 Teams offering model while preserving existing Free and Early Bird behavior.");

    public PricingCatalogVersion GetCurrentSpacesVersion() =>
        new(
            PricingCatalogConstants.CurrentSpacesCatalogVersion,
            PricingCatalogVersionStatus.Active,
            DateTimeOffset.UnixEpoch,
            null,
            "Initial V1 Spaces offering model for marketplace and co-working space organizations.");

    public PricingCatalogVersion GetCurrentHostVersion() =>
        new(
            PricingCatalogConstants.CurrentHostCatalogVersion,
            PricingCatalogVersionStatus.Active,
            DateTimeOffset.UnixEpoch,
            null,
            "Initial V1 Host offering model for individual space and resource rental.");
}
