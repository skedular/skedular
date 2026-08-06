namespace Organization.Shared.Models.PricingCatalog;

public enum PricingCatalogProductOfferingCode
{
    NotSet = 0,
    Teams = 1,
    Spaces = 2,
    Host = 3,
}

public enum PricingCatalogVersionStatus
{
    Active = 0,
}

public enum PricingCatalogVisibility
{
    Public = 0,
}

public enum PricingCatalogSubscriptionPlanCode
{
    NotSet = 0,
    Free = 1,
    PayAsYouGo = 2,
    EnterpriseCapacity = 3,
    LegacyEarlyBird = 4,
    Growth = 5,
    Business = 6,
    ContactUs = 7,
}

public enum PricingCatalogCommercialModel
{
    Free = 0,
    UsageBased = 1,
    CapacityBased = 2,
}

public enum PricingCatalogPlanAvailability
{
    NotSet = 0,
    SelfService = 1,
    ContactUs = 2,
    Hidden = 3,
    Deprecated = 4,
    Unavailable = 5,
    ExistingCustomersOnly = 6,
}

public enum OrganizationOfferingPlanStatus
{
    NotSet = 0,
    Pending = 1,
    Active = 2,
    ScheduledChange = 3,
    Canceled = 4,
    Expired = 5,
    Legacy = 6,
}

public static class PricingCatalogConstants
{
    public const string CurrentTeamsCatalogVersion = CatalogVersionConstants.TeamsV1;
    public const string CurrentSpacesCatalogVersion = CatalogVersionConstants.SpacesV1;
    public const string CurrentHostCatalogVersion = CatalogVersionConstants.HostV1;
}

public static class PricingCatalogNameExtensions
{
    extension(PricingCatalogProductOfferingCode code)
    {
        public string ToPricingCatalogProductOfferingCodeName() =>
            code switch
            {
                PricingCatalogProductOfferingCode.Teams => "Teams",
                PricingCatalogProductOfferingCode.Spaces => "Spaces",
                PricingCatalogProductOfferingCode.Host => "Host",
                PricingCatalogProductOfferingCode.NotSet => "Not set",
                _ => throw new ArgumentOutOfRangeException(nameof(code), code,
                    $"Unexpected value for {nameof(code)}: {code}. Update enum mapping or caller input."),
            };
    }

    extension(PricingCatalogSubscriptionPlanCode code)
    {
        public string ToPricingCatalogSubscriptionPlanCodeName() =>
            code switch
            {
                PricingCatalogSubscriptionPlanCode.Free => "Free",
                PricingCatalogSubscriptionPlanCode.PayAsYouGo => "Pay As You Go",
                PricingCatalogSubscriptionPlanCode.EnterpriseCapacity => "Enterprise Capacity",
                PricingCatalogSubscriptionPlanCode.LegacyEarlyBird => "Early Bird",
                PricingCatalogSubscriptionPlanCode.Growth => "Growth",
                PricingCatalogSubscriptionPlanCode.Business => "Business",
                PricingCatalogSubscriptionPlanCode.ContactUs => "Contact Us",
                PricingCatalogSubscriptionPlanCode.NotSet => "Not set",
                _ => throw new ArgumentOutOfRangeException(nameof(code), code,
                    $"Unexpected value for {nameof(code)}: {code}. Update enum mapping or caller input."),
            };
    }

    extension(PricingCatalogPlanAvailability availability)
    {
        public string ToPricingCatalogPlanAvailabilityName() =>
            availability switch
            {
                PricingCatalogPlanAvailability.SelfService => "Self-service",
                PricingCatalogPlanAvailability.ContactUs => "Contact Us",
                PricingCatalogPlanAvailability.Hidden => "Hidden",
                PricingCatalogPlanAvailability.Deprecated => "Deprecated",
                PricingCatalogPlanAvailability.Unavailable => "Unavailable",
                PricingCatalogPlanAvailability.ExistingCustomersOnly => "Existing customers only",
                PricingCatalogPlanAvailability.NotSet => "Not set",
                _ => throw new ArgumentOutOfRangeException(nameof(availability), availability,
                    $"Unexpected value for {nameof(availability)}: {availability}. Update enum mapping or caller input."),
            };
    }

    extension(OrganizationOfferingPlanStatus status)
    {
        public string ToOrganizationOfferingPlanStatusName() =>
            status switch
            {
                OrganizationOfferingPlanStatus.Pending => "Pending",
                OrganizationOfferingPlanStatus.Active => "Active",
                OrganizationOfferingPlanStatus.ScheduledChange => "Scheduled change",
                OrganizationOfferingPlanStatus.Canceled => "Canceled",
                OrganizationOfferingPlanStatus.Expired => "Expired",
                OrganizationOfferingPlanStatus.Legacy => "Legacy",
                OrganizationOfferingPlanStatus.NotSet => "Not set",
                _ => throw new ArgumentOutOfRangeException(nameof(status), status,
                    $"Unexpected value for {nameof(status)}: {status}. Update enum mapping or caller input."),
            };
    }
}
