using Api.Shared.Services.Models;

namespace Api.Shared.Services.Offering;

public class Offering
{
    public string Name { get; set; } = string.Empty;
    public IReadOnlyCollection<FeatureSetCode> FeatureSets { get; set; } = [];
    public IReadOnlyCollection<string> UnderPriceLines { get; set; } = [];
    public int? UnitPrice { get; set; }
    public int? FixedPrice { get; set; }
    public Currency Currency { get; set; }
    public int? MaxUserCount { get; set; }
    public int? MaxLocationCount { get; set; }
    public int? MaxTeamCount { get; set; }
    public int? MaxResourceCount { get; set; }
    public int? MaxBookingInstanceCount { get; set; }
    public decimal HostCommissionPercentage { get; set; }
    public bool CanCancel { get; set; } = true;
}

public enum OfferingCode
{
    EarlyBirdV1 = 0,
    FreeTierV1 = 10000,
    PayAsYouGoV1 = 20000,
    EnterpriseCustomV1 = 30000,
    SpacesFreeTierV1 = 40000,
    SpacesGrowthV1 = 50000,
    SpacesBusinessV1 = 60000,
    SpacesContactUsV1 = 70000,
    HostStandardV1 = 80000,
}

public static class Offerings
{
    public static readonly IReadOnlyList<OfferingCode> TeamsOfferings =
    [
        OfferingCode.PayAsYouGoV1,
        OfferingCode.EnterpriseCustomV1,
    ];

    public static readonly IReadOnlyList<OfferingCode> SpacesOfferings =
    [
        OfferingCode.SpacesFreeTierV1,
        OfferingCode.SpacesGrowthV1,
        OfferingCode.SpacesBusinessV1,
        OfferingCode.SpacesContactUsV1,
    ];

    public static readonly IReadOnlyList<OfferingCode> HostOfferings =
    [
        OfferingCode.HostStandardV1,
    ];

    public static readonly IReadOnlyList<OfferingCode> AllOfferings =
    [
        .. TeamsOfferings,
        .. SpacesOfferings,
        .. HostOfferings,
    ];

    public static IDictionary<OfferingCode, Offering> OfferingSet { get; } = new Dictionary<OfferingCode, Offering>
    {
        {
            OfferingCode.EarlyBirdV1, new Offering
            {
                Name = "Early bird",
                FeatureSets =
                [
                    FeatureSetCode.OrganizationUnlimitedLocations,
                    FeatureSetCode.OrganizationUnlimitedTeams,
                    FeatureSetCode.OrganizationUnlimitedBookings,
                ],
                UnitPrice = null,
                FixedPrice = null,
                Currency = Currency.Usd,
                MaxUserCount = null,
                MaxLocationCount = null,
                MaxTeamCount = null,
                MaxResourceCount = null,
                MaxBookingInstanceCount = null,
                UnderPriceLines = ["Completely Free"],
            }
        },
        {
            OfferingCode.FreeTierV1, new Offering
            {
                Name = "Basic",
                FeatureSets =
                [
                    FeatureSetCode.OrganizationUpToOneLocation,
                    FeatureSetCode.OrganizationUpToOneTeam,
                    FeatureSetCode.OrganizationUnlimitedBookings,
                ],
                UnitPrice = null,
                FixedPrice = null,
                Currency = Currency.Usd,
                MaxUserCount = 10,
                MaxLocationCount = 1,
                MaxTeamCount = 1,
                MaxResourceCount = null,
                MaxBookingInstanceCount = null,
                UnderPriceLines = ["Completely Free", "Up to 10 Monthly Active Users"],
            }
        },
        {
            OfferingCode.PayAsYouGoV1, new Offering
            {
                Name = "Pay as you go",
                FeatureSets =
                [
                    FeatureSetCode.OrganizationUnlimitedLocations,
                    FeatureSetCode.OrganizationUnlimitedTeams,
                    FeatureSetCode.OrganizationUnlimitedBookings,
                ],
                UnitPrice = 300,
                FixedPrice = null,
                Currency = Currency.Usd,
                MaxUserCount = null,
                MaxLocationCount = null,
                MaxTeamCount = null,
                MaxResourceCount = null,
                MaxBookingInstanceCount = null,
                UnderPriceLines = ["Per monthly active user/month", "Best for most users"],
            }
        },
        {
            OfferingCode.EnterpriseCustomV1, new Offering
            {
                Name = "Enterprise",
                FeatureSets =
                [
                    FeatureSetCode.OrganizationCompanyResources,
                    FeatureSetCode.OrganizationAnalytics,
                    FeatureSetCode.OrganizationPremiumSupport,
                ],
                UnitPrice = null,
                FixedPrice = null,
                Currency = Currency.Usd,
                MaxUserCount = null,
                MaxLocationCount = null,
                MaxTeamCount = null,
                MaxResourceCount = null,
                MaxBookingInstanceCount = null,
                UnderPriceLines = ["Best for large organizations with multiple locations"],
            }
        },
        {
            OfferingCode.SpacesFreeTierV1, new Offering
            {
                Name = "Spaces Free",
                FeatureSets =
                [
                    FeatureSetCode.OrganizationUnlimitedLocations,
                    FeatureSetCode.OrganizationUnlimitedBookings,
                    FeatureSetCode.OrganizationAnalytics,
                ],
                UnitPrice = null,
                FixedPrice = null,
                Currency = Currency.Usd,
                MaxUserCount = null,
                MaxLocationCount = null,
                MaxTeamCount = null,
                MaxResourceCount = null,
                MaxBookingInstanceCount = 100,
                UnderPriceLines = ["Free for 14 days", "Up to 100 booking instances per month"],
            }
        },
        {
            OfferingCode.SpacesGrowthV1, new Offering
            {
                Name = "Spaces Growth",
                FeatureSets =
                [
                    FeatureSetCode.OrganizationUnlimitedLocations,
                    FeatureSetCode.OrganizationUnlimitedBookings,
                    FeatureSetCode.OrganizationAnalytics,
                    FeatureSetCode.OrganizationPremiumSupport,
                ],
                UnitPrice = null,
                FixedPrice = 4900,
                Currency = Currency.Usd,
                MaxUserCount = null,
                MaxLocationCount = null,
                MaxTeamCount = null,
                MaxResourceCount = null,
                MaxBookingInstanceCount = 500,
                UnderPriceLines = ["Per month", "Up to 500 booking instances/month"],
            }
        },
        {
            OfferingCode.SpacesBusinessV1, new Offering
            {
                Name = "Spaces Business",
                FeatureSets =
                [
                    FeatureSetCode.OrganizationUnlimitedLocations,
                    FeatureSetCode.OrganizationUnlimitedBookings,
                    FeatureSetCode.OrganizationCompanyResources,
                    FeatureSetCode.OrganizationAnalytics,
                    FeatureSetCode.OrganizationPremiumSupport,
                ],
                UnitPrice = null,
                FixedPrice = 14900,
                Currency = Currency.Usd,
                MaxUserCount = null,
                MaxLocationCount = null,
                MaxTeamCount = null,
                MaxResourceCount = null,
                MaxBookingInstanceCount = 1000,
                UnderPriceLines = ["Per month", "Up to 1,000 booking instances/month"],
            }
        },
        {
            OfferingCode.SpacesContactUsV1, new Offering
            {
                Name = "Spaces Contact Us",
                FeatureSets =
                [
                    FeatureSetCode.OrganizationUnlimitedLocations,
                    FeatureSetCode.OrganizationUnlimitedBookings,
                ],
                UnitPrice = null,
                FixedPrice = null,
                Currency = Currency.Usd,
                MaxUserCount = null,
                MaxLocationCount = null,
                MaxTeamCount = null,
                MaxResourceCount = null,
                MaxBookingInstanceCount = null,
                UnderPriceLines = ["Custom pricing", "Custom booking instance capacity"],
            }
        },
        {
            OfferingCode.HostStandardV1, new Offering
            {
                Name = "Host Standard",
                FeatureSets =
                [
                    FeatureSetCode.OrganizationUnlimitedLocations,
                    FeatureSetCode.OrganizationUnlimitedBookings,
                ],
                UnitPrice = null,
                FixedPrice = null,
                Currency = Currency.Usd,
                MaxUserCount = null,
                MaxLocationCount = null,
                MaxTeamCount = null,
                MaxResourceCount = null,
                MaxBookingInstanceCount = null,
                HostCommissionPercentage = 5m,
                CanCancel = false,
                UnderPriceLines = ["5% commission per booking", "Free to list"],
            }
        },
    };

    public static IReadOnlyList<OfferingCode> ForOrganizationType(OrganizationType organizationType) =>
        organizationType switch
        {
            OrganizationType.Private => TeamsOfferings,
            OrganizationType.Marketplace => SpacesOfferings,
            OrganizationType.Host => HostOfferings,
            _ => throw new ArgumentOutOfRangeException(nameof(organizationType), organizationType, "Unsupported organization type."),
        };

    extension(OfferingCode offeringCode)
    {
        public bool IsFreeOffering() => offeringCode is OfferingCode.FreeTierV1 or OfferingCode.EarlyBirdV1 or OfferingCode.SpacesFreeTierV1;
        public bool IsEarlyBirdOffering() => offeringCode == OfferingCode.EarlyBirdV1;

        public decimal GetEffectiveHostCommissionPercentage(decimal configuredPercentage) =>
            offeringCode.IsEarlyBirdOffering() ? 0m : configuredPercentage;

        public bool IsPayAsYouGoOffering() => offeringCode == OfferingCode.PayAsYouGoV1;
        public bool IsEnterpriseOffering() => offeringCode == OfferingCode.EnterpriseCustomV1;
        public Offering GetOffering() => OfferingSet[offeringCode];
    }

    extension(string code)
    {
        public OfferingCode ToOfferingCode() =>
            code switch
            {
                "EARLY_BIRD_V1" => OfferingCode.EarlyBirdV1,
                "FREE_TIER_V1" => OfferingCode.FreeTierV1,
                "PAY_AS_YOU_GO_V1" => OfferingCode.PayAsYouGoV1,
                "ENTERPRISE_CUSTOM_V1" => OfferingCode.EnterpriseCustomV1,
                "SPACES_FREE_TIER_V1" => OfferingCode.SpacesFreeTierV1,
                "SPACES_GROWTH_V1" => OfferingCode.SpacesGrowthV1,
                "SPACES_BUSINESS_V1" => OfferingCode.SpacesBusinessV1,
                "SPACES_CONTACT_US_V1" => OfferingCode.SpacesContactUsV1,
                "HOST_STANDARD_V1" => OfferingCode.HostStandardV1,
                _ => throw new ArgumentException($"{code} is not valid offering code", nameof(code)),
            };
    }

    extension(OfferingCode code)
    {
        public string ToOfferingCode() =>
            code switch
            {
                OfferingCode.EarlyBirdV1 => "EARLY_BIRD_V1",
                OfferingCode.FreeTierV1 => "FREE_TIER_V1",
                OfferingCode.PayAsYouGoV1 => "PAY_AS_YOU_GO_V1",
                OfferingCode.EnterpriseCustomV1 => "ENTERPRISE_CUSTOM_V1",
                OfferingCode.SpacesFreeTierV1 => "SPACES_FREE_TIER_V1",
                OfferingCode.SpacesGrowthV1 => "SPACES_GROWTH_V1",
                OfferingCode.SpacesBusinessV1 => "SPACES_BUSINESS_V1",
                OfferingCode.SpacesContactUsV1 => "SPACES_CONTACT_US_V1",
                OfferingCode.HostStandardV1 => "HOST_STANDARD_V1",
                _ => throw new ArgumentException($"{code} is not valid offering code", nameof(code)),
            };
    }

    extension(DateTimeOffset date)
    {
        public DateTimeOffset GetOfferingPeriodStart() => new(date.Year, date.Month, 1, 0, 0, 0, date.Offset);
        public DateTimeOffset GetOfferingPeriodEnd() => date.AddMonths(1);
        public DateTimeOffset GetNextOfferingPeriodStart() => date;
    }
}
