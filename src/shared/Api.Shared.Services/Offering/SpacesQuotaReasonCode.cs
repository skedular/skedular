namespace Api.Shared.Services.Offering;

public enum SpacesQuotaReasonCode
{
    NotSet = 0,
    WithinQuota = 1,
    FreeTierLimitExceeded = 2,
    PaidTierLimitExceeded = 3,
    CustomCapacityExceeded = 4,
    MissingOfferingState = 5,
    OutOfPeriodExcluded = 6,
    TrialExpired = 7
}

public static class SpacesQuotaReasonCodeExtensions
{
    extension(SpacesQuotaReasonCode src)
    {
        public string ToSpacesQuotaReasonCodeName() =>
            src switch
            {
                SpacesQuotaReasonCode.NotSet => "Not Set",
                SpacesQuotaReasonCode.WithinQuota => "Within Quota",
                SpacesQuotaReasonCode.FreeTierLimitExceeded => "Free Tier Limit Exceeded",
                SpacesQuotaReasonCode.PaidTierLimitExceeded => "Paid Tier Limit Exceeded",
                SpacesQuotaReasonCode.CustomCapacityExceeded => "Custom Capacity Exceeded",
                SpacesQuotaReasonCode.MissingOfferingState => "Missing Offering State",
                SpacesQuotaReasonCode.OutOfPeriodExcluded => "Out Of Period Excluded",
                SpacesQuotaReasonCode.TrialExpired => "Trial Expired",
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
            };
    }
}
