namespace Api.Shared.Services.Offering;

public class Offering
{
    public string Name { get; set; } = string.Empty;
    public IReadOnlyCollection<FeatureSetCode> FeatureSets { get; set; } = [];
    public IReadOnlyCollection<string> UnderPriceLines { get; set; } = [];
    public int UnitPrice { get; set; }
    public int MaxUserCount { get; set; }
    public int MaxLocationCount { get; set; }
    public int MaxTeamCount { get; set; }
}

public enum OfferingCode
{
    EarlyBirdV1 = 0,
    FreeTierV1 = 10000,
    PayAsYouGoV1 = 20000,
    EnterpriseCustomV1 = 1000000
}

public static class Offerings
{
    public static readonly ICollection<OfferingCode> AllOfferings =
    [
        OfferingCode.PayAsYouGoV1,
        OfferingCode.EnterpriseCustomV1
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
                    FeatureSetCode.OrganizationUnlimitedBookings
                ],
                UnitPrice = 0,
                MaxUserCount = -1,
                MaxLocationCount = -1,
                MaxTeamCount = -1,
                UnderPriceLines = ["Completely Free"]
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
                    FeatureSetCode.OrganizationUnlimitedBookings
                ],
                UnitPrice = 0,
                MaxUserCount = 10,
                MaxLocationCount = 1,
                MaxTeamCount = 1,
                UnderPriceLines = ["Completely Free", "Up to 10 Monthly Active Users"]
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
                    FeatureSetCode.OrganizationUnlimitedBookings
                ],
                UnitPrice = 300,
                MaxUserCount = -1,
                MaxLocationCount = -1,
                MaxTeamCount = -1,
                UnderPriceLines = ["Per monthly active user/month", "Best for most users"]
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
                    FeatureSetCode.OrganizationPremiumSupport
                ],
                UnitPrice = -1,
                MaxUserCount = -1,
                MaxLocationCount = -1,
                MaxTeamCount = -1,
                UnderPriceLines = ["Best for large organizations with multiple locations"]
            }
        }
    };

    extension(OfferingCode offeringCode)
    {
        public bool IsFreeOffering() =>
            offeringCode is OfferingCode.FreeTierV1 or OfferingCode.EarlyBirdV1;

        public bool IsEarlyBirdOffering() => offeringCode == OfferingCode.EarlyBirdV1;
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
                _ => throw new ArgumentException($"{code} is not valid offering code", nameof(code))
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
                _ => throw new ArgumentException($"{code} is not valid offering code", nameof(code))
            };
    }

    extension(DateTimeOffset date)
    {
        public DateTimeOffset GetOfferingPeriodStart() => new(date.Year, date.Month, 1, 0, 0, 0, date.Offset);
        public DateTimeOffset GetOfferingPeriodEnd() => date.AddMonths(1);
        public DateTimeOffset GetNextOfferingPeriodStart() => date;
    }
}
