namespace Api.Shared.Services.Offering;

public class Offering
{
    public string Name { get; set; } = string.Empty;
    public IReadOnlyCollection<FeatureSetCode> FeatureSets { get; set; } = [];
    public int UnitPrice { get; set; }
    public int MaxUserCount { get; set; }
    public int MaxLocationCount { get; set; }
    public int MaxTeamCount { get; set; }
    public int MaxOrganizationCount { get; set; }
}

public enum OfferingCode
{
    EarlyBirdV1 = 0,
    FreeTierV1 = 1,
    PayAsYouGoV1 = 2
}

public static class Offerings
{
    public static ICollection<OfferingCode> AllOfferings = [OfferingCode.PayAsYouGoV1];

    public static IDictionary<OfferingCode, Offering> OfferingSet { get; } = new Dictionary<OfferingCode, Offering>
    {
        {
            OfferingCode.EarlyBirdV1, new Offering
            {
                Name = "Early bird",
                FeatureSets =
                [
                    FeatureSetCode.OrganizationUnlimitedUsers,
                    FeatureSetCode.OrganizationUnlimitedLocations,
                    FeatureSetCode.OrganizationUnlimitedTeams
                ],
                UnitPrice = 0,
                MaxUserCount = -1,
                MaxLocationCount = -1,
                MaxTeamCount = -1,
                MaxOrganizationCount = -1
            }
        },
        {
            OfferingCode.FreeTierV1, new Offering
            {
                Name = "Free",
                FeatureSets =
                [
                    FeatureSetCode.OrganizationUpToTenUser,
                    FeatureSetCode.OrganizationUpToOneLocation,
                    FeatureSetCode.OrganizationUpToOneTeam
                ],
                UnitPrice = 0,
                MaxUserCount = 10,
                MaxLocationCount = 1,
                MaxTeamCount = 1,
                MaxOrganizationCount = 1
            }
        },
        {
            OfferingCode.PayAsYouGoV1, new Offering
            {
                Name = "Early bird",
                FeatureSets =
                [
                    FeatureSetCode.OrganizationUnlimitedUsers,
                    FeatureSetCode.OrganizationUnlimitedLocations,
                    FeatureSetCode.OrganizationUnlimitedTeams
                ],
                UnitPrice = 300,
                MaxUserCount = -1,
                MaxLocationCount = -1,
                MaxTeamCount = -1,
                MaxOrganizationCount = -1
            }
        }
    };

    public static bool IsFreeOffering(this OfferingCode offeringCode) =>
        offeringCode is OfferingCode.FreeTierV1 or OfferingCode.EarlyBirdV1;

    public static bool IsEarlyBirdOffering(this OfferingCode offeringCode) => offeringCode == OfferingCode.EarlyBirdV1;

    public static Offering GetOffering(this OfferingCode offeringCode) => OfferingSet[offeringCode];

    public static OfferingCode ToOfferingCode(this string code) =>
        code switch
        {
            "EARLY_BIRD_V1" => OfferingCode.EarlyBirdV1,
            "FREE_TIER_V1" => OfferingCode.FreeTierV1,
            "PAY_AS_YOU_GO_V1" => OfferingCode.PayAsYouGoV1,
            _ => throw new ArgumentException($"{code} is not valid offering code", nameof(code))
        };

    public static string ToOfferingCode(this OfferingCode code) =>
        code switch
        {
            OfferingCode.EarlyBirdV1 => "EARLY_BIRD_V1",
            OfferingCode.FreeTierV1 => "FREE_TIER_V1",
            OfferingCode.PayAsYouGoV1 => "PAY_AS_YOU_GO_V1",
            _ => throw new ArgumentException($"{code} is not valid offering code", nameof(code))
        };

    public static DateTimeOffset GetOfferingPeriodStart(this DateTimeOffset date) =>
        new(date.Year, date.Month, 1, 0, 0, 0, date.Offset);

    public static DateTimeOffset GetOfferingPeriodEnd(this DateTimeOffset date) => date.AddMonths(1).AddTicks(-1);
    public static DateTimeOffset GetNextOfferingPeriodStart(this DateTimeOffset end) => end.AddTicks(1);
}
