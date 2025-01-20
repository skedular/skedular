namespace Api.Shared.Services.Offering;

public class Feature
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public enum FeatureSetCode
{
    OrganizationUpToXUsers = 0,
    OrganizationUpTo10Users = 1,
    OrganizationUnlimitedUsers = 2,
    OrganizationUpToOneLocation = 3,
    OrganizationUpToOneTeam = 4,
    OrganizationUnlimitedLocations = 5,
    OrganizationUnlimitedTeams = 6,
    OrganizationUpTo100Users = 7
}

public class Features
{
    public static IDictionary<FeatureSetCode, Feature> FeatureSet { get; } = new Dictionary<FeatureSetCode, Feature>
    {
        {
            FeatureSetCode.OrganizationUpToXUsers,
            new Feature { Name = "For up to X users", Description = "Allow up to X monthly active users in an organization" }
        },
        {
            FeatureSetCode.OrganizationUpTo10Users,
            new Feature { Name = "For up to 10 users", Description = "Allow up to 10 monthly active users in an organization" }
        },
        {
            FeatureSetCode.OrganizationUpTo100Users,
            new Feature { Name = "For up to 100 users", Description = "Allow up to 100 monthly active users in an organization" }
        },
        {
            FeatureSetCode.OrganizationUnlimitedUsers,
            new Feature { Name = "Unlimited users", Description = "Allow unlimited monthly active users in an organization" }
        },
        {
            FeatureSetCode.OrganizationUpToOneLocation,
            new Feature { Name = "Up to 1 location in an organization", Description = "Allow creating one location in an organization" }
        },
        {
            FeatureSetCode.OrganizationUpToOneTeam,
            new Feature { Name = "Up to 1 team in an organization", Description = "Allow creating one team in an organization" }
        },
        {
            FeatureSetCode.OrganizationUnlimitedLocations,
            new Feature { Name = "Unlimited locations", Description = "Allow unlimited locations for an organization" }
        },
        {
            FeatureSetCode.OrganizationUnlimitedTeams,
            new Feature { Name = "Unlimited teams", Description = "Allow unlimited teams for an organization" }
        }
    };
}
