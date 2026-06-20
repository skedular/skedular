namespace Api.Shared.Services.Offering;

public enum SpacesSubscriptionStatus
{
    NotSet = 0,
    TrialActive = 1,
    TrialExpiring = 2,
    TrialExpired = 3,
    ComplimentaryBridge = 4,
    PaidActive = 5,
    PaidInactive = 6,
    LegacyActive = 7,
    MissingState = 8
}

public static class SpacesSubscriptionStatusExtensions
{
    extension(SpacesSubscriptionStatus status)
    {
        public string ToSpacesSubscriptionStatusName() =>
            status switch
            {
                SpacesSubscriptionStatus.NotSet => "Not Set",
                SpacesSubscriptionStatus.TrialActive => "Trial Active",
                SpacesSubscriptionStatus.TrialExpiring => "Trial Expiring",
                SpacesSubscriptionStatus.TrialExpired => "Trial Expired",
                SpacesSubscriptionStatus.ComplimentaryBridge => "Complimentary Bridge",
                SpacesSubscriptionStatus.PaidActive => "Paid Active",
                SpacesSubscriptionStatus.PaidInactive => "Paid Inactive",
                SpacesSubscriptionStatus.LegacyActive => "Legacy Active",
                SpacesSubscriptionStatus.MissingState => "Missing State",
                _ => throw new ArgumentOutOfRangeException(nameof(status), status,
                    $"Unexpected value for {nameof(status)}: {status}. Update enum mapping or caller input.")
            };
    }
}
