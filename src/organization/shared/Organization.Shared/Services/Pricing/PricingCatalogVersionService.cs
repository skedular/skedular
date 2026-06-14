using Organization.Shared.Models.PricingCatalog;

namespace Organization.Shared.Services.Pricing;

public interface IPricingCatalogVersionService
{
    PricingCatalogVersion GetCurrentTeamsVersion();
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
}
