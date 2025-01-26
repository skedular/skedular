namespace Api.Shared.Services.Offering;

public class Feature
{
    public string Description { get; set; } = string.Empty;
}

public enum FeatureSetCode
{
    OrganizationUpToOneLocation,
    OrganizationUpToOneTeam,
    OrganizationUnlimitedLocations,
    OrganizationUnlimitedTeams,
    OrganizationUnlimitedBookings,
    OrganizationCompanyResources,
    OrganizationAnalytics,
    OrganizationPremiumSupport,
}

public class Features
{
    public static IDictionary<FeatureSetCode, Feature> FeatureSet { get; } = new Dictionary<FeatureSetCode, Feature>
    {
        {
            FeatureSetCode.OrganizationUpToOneLocation,
            new Feature {  Description = "One Location" }
        },
        {
            FeatureSetCode.OrganizationUpToOneTeam,
            new Feature {  Description = "One Team" }
        },
        {
            FeatureSetCode.OrganizationUnlimitedLocations,
            new Feature { Description = "Unlimited Locations" }
        },
        {
            FeatureSetCode.OrganizationUnlimitedTeams,
            new Feature { Description = "Unlimited Teams" }
        },
        {
            FeatureSetCode.OrganizationUnlimitedBookings,
            new Feature { Description = "Unlimited Bookings" }
        },
        {
            FeatureSetCode.OrganizationCompanyResources,
            new Feature { Description = "Manage company resources" }
        },
        {
            FeatureSetCode.OrganizationAnalytics,
            new Feature { Description = "Powerful analytics tools" }
        },
        {
            FeatureSetCode.OrganizationPremiumSupport,
            new Feature { Description = "Premium support" }
        }
    };
}
