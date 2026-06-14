namespace Api.Shared.Services.Offering;

public enum EntitlementReasonCode
{
    NotSet = 0,
    Allowed = 1,
    FreeActiveUserLimitReached = 2,
    FreeTeamLimitReached = 3,
    FreeLocationLimitReached = 4,
    EnterpriseCapacityReached = 5,
    OfferingNotFound = 6,
    OfferingNotEffective = 7,
    ContactUsRequired = 8,
    LegacyOfferingUnchanged = 9
}

public static class EntitlementReasonCodeExtensions
{
    extension(EntitlementReasonCode src)
    {
        public string ToEntitlementReasonCodeName() =>
            src switch
            {
                EntitlementReasonCode.NotSet => "Not Set",
                EntitlementReasonCode.Allowed => "Allowed",
                EntitlementReasonCode.FreeActiveUserLimitReached => "Free Active User Limit Reached",
                EntitlementReasonCode.FreeTeamLimitReached => "Free Team Limit Reached",
                EntitlementReasonCode.FreeLocationLimitReached => "Free Location Limit Reached",
                EntitlementReasonCode.EnterpriseCapacityReached => "Enterprise Capacity Reached",
                EntitlementReasonCode.OfferingNotFound => "Offering Not Found",
                EntitlementReasonCode.OfferingNotEffective => "Offering Not Effective",
                EntitlementReasonCode.ContactUsRequired => "Contact Us Required",
                EntitlementReasonCode.LegacyOfferingUnchanged => "Legacy Offering Unchanged",
                _ => throw new ArgumentOutOfRangeException(nameof(src), src, null)
            };
    }
}
