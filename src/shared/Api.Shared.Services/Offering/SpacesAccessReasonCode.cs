namespace Api.Shared.Services.Offering;

public enum SpacesAccessReasonCode
{
    NotSet = 0,
    AllowedTrial = 1,
    AllowedPaid = 2,
    AllowedComplimentaryBridge = 3,
    AllowedProtectiveAction = 4,
    AllowedReadOrRecovery = 5,
    TrialExpired = 6,
    PaidInactive = 7,
    MissingTrialState = 8,
    MissingOfferingState = 9,
    ActionNotAllowed = 10,
}

public static class SpacesAccessReasonCodeExtensions
{
    extension(SpacesAccessReasonCode reasonCode)
    {
        public string ToSpacesAccessReasonCodeName() =>
            reasonCode switch
            {
                SpacesAccessReasonCode.NotSet => "Not Set",
                SpacesAccessReasonCode.AllowedTrial => "Allowed Trial",
                SpacesAccessReasonCode.AllowedPaid => "Allowed Paid",
                SpacesAccessReasonCode.AllowedComplimentaryBridge => "Allowed Complimentary Bridge",
                SpacesAccessReasonCode.AllowedProtectiveAction => "Allowed Protective Action",
                SpacesAccessReasonCode.AllowedReadOrRecovery => "Allowed Read or Recovery",
                SpacesAccessReasonCode.TrialExpired => "Trial Expired",
                SpacesAccessReasonCode.PaidInactive => "Paid Inactive",
                SpacesAccessReasonCode.MissingTrialState => "Missing Trial State",
                SpacesAccessReasonCode.MissingOfferingState => "Missing Offering State",
                SpacesAccessReasonCode.ActionNotAllowed => "Action Not Allowed",
                _ => throw new ArgumentOutOfRangeException(nameof(reasonCode), reasonCode,
                    $"Unexpected value for {nameof(reasonCode)}: {reasonCode}. Update enum mapping or caller input."),
            };
    }
}
