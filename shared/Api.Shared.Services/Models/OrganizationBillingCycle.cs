namespace Api.Shared.Services.Models;

public enum OrganizationBillingCycle
{
    Weekly,
    Fortnightly,
    Monthly
}

public static class OrganizationBillingCycleConstants
{
    public const string Weekly = "WEEKLY";
    public const string Fortnightly = "FORTNIGHTLY";
    public const string Monthly = "MONTHLY";
}

public static class OrganizationBillingCycleExtensions
{
    extension(OrganizationBillingCycle src)
    {
        public string ToOrganizationBillingCycle() =>
            src switch
            {
                OrganizationBillingCycle.Weekly => OrganizationBillingCycleConstants.Weekly,
                OrganizationBillingCycle.Fortnightly => OrganizationBillingCycleConstants.Fortnightly,
                OrganizationBillingCycle.Monthly => OrganizationBillingCycleConstants.Monthly,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToOrganizationBillingCycleName() =>
            src switch
            {
                OrganizationBillingCycle.Weekly => "Weekly",
                OrganizationBillingCycle.Fortnightly => "Fortnightly",
                OrganizationBillingCycle.Monthly => "Monthly",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string src)
    {
        public OrganizationBillingCycle ToOrganizationBillingCycle() =>
            src switch
            {
                OrganizationBillingCycleConstants.Weekly => OrganizationBillingCycle.Weekly,
                OrganizationBillingCycleConstants.Fortnightly => OrganizationBillingCycle.Fortnightly,
                OrganizationBillingCycleConstants.Monthly => OrganizationBillingCycle.Monthly,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
