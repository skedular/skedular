namespace Api.Shared.Services.Models;

public enum MembershipTerm
{
    NotSet = 0,
    Daily = 1,
    Weekly = 2,
    Fortnightly = 3,
    Monthly = 4,
    TwoMonths = 5,
    Quarterly = 6,
    FourMonths = 7,
    FiveMonths = 8,
    SixMonths = 9,
    Yearly = 10,
}

public static class MembershipTermConstants
{
    public const string NotSet = "NOT_SET";
    public const string Daily = "DAILY";
    public const string Weekly = "WEEKLY";
    public const string Fortnightly = "FORTNIGHTLY";
    public const string Monthly = "MONTHLY";
    public const string TwoMonths = "TWO_MONTHS";
    public const string Quarterly = "QUARTERLY";
    public const string FourMonths = "FOUR_MONTHS";
    public const string FiveMonths = "FIVE_MONTHS";
    public const string SixMonths = "SIX_MONTHS";
    public const string Yearly = "YEARLY";
}

public static class MembershipTermExtensions
{
    extension(MembershipTerm src)
    {
        public string ToMembershipTerms() =>
            src switch
            {
                MembershipTerm.NotSet => MembershipTermConstants.NotSet,
                MembershipTerm.Daily => MembershipTermConstants.Daily,
                MembershipTerm.Weekly => MembershipTermConstants.Weekly,
                MembershipTerm.Monthly => MembershipTermConstants.Monthly,
                MembershipTerm.TwoMonths => MembershipTermConstants.TwoMonths,
                MembershipTerm.Quarterly => MembershipTermConstants.Quarterly,
                MembershipTerm.FourMonths => MembershipTermConstants.FourMonths,
                MembershipTerm.FiveMonths => MembershipTermConstants.FiveMonths,
                MembershipTerm.SixMonths => MembershipTermConstants.SixMonths,
                MembershipTerm.Yearly => MembershipTermConstants.Yearly,
                MembershipTerm.Fortnightly => MembershipTermConstants.Fortnightly,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };

        public string ToMembershipTermName() =>
            src switch
            {
                MembershipTerm.NotSet => "Not Set",
                MembershipTerm.Daily => "Daily",
                MembershipTerm.Weekly => "Weekly",
                MembershipTerm.Monthly => "Monthly",
                MembershipTerm.TwoMonths => "2-Monthly",
                MembershipTerm.Quarterly => "Quarterly",
                MembershipTerm.FourMonths => "4-Monthly",
                MembershipTerm.FiveMonths => "5-Monthly",
                MembershipTerm.SixMonths => "6-Monthly",
                MembershipTerm.Yearly => "Yearly",
                MembershipTerm.Fortnightly => "Fortnightly",
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };

        public string ToInvoicePriceUnitName() =>
            src switch
            {
                MembershipTerm.NotSet => "not-set",
                MembershipTerm.Daily => "daily",
                MembershipTerm.Weekly => "weekly",
                MembershipTerm.Monthly => "monthly",
                MembershipTerm.TwoMonths => "2-monthly",
                MembershipTerm.Quarterly => "quarterly",
                MembershipTerm.FourMonths => "4-monthly",
                MembershipTerm.FiveMonths => "5-monthly",
                MembershipTerm.SixMonths => "6-monthly",
                MembershipTerm.Yearly => "yearly",
                MembershipTerm.Fortnightly => "fortnightly",
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };

        public string ToStripePriceUnitName() =>
            src switch
            {
                MembershipTerm.NotSet => "Not-Set",
                MembershipTerm.Daily => "Daily",
                MembershipTerm.Weekly => "Weekly",
                MembershipTerm.Monthly => "Monthly",
                MembershipTerm.TwoMonths => "2-Monthly",
                MembershipTerm.Quarterly => "Quarterly",
                MembershipTerm.FourMonths => "4-Monthly",
                MembershipTerm.FiveMonths => "5-Monthly",
                MembershipTerm.SixMonths => "6-Monthly",
                MembershipTerm.Yearly => "Yearly",
                MembershipTerm.Fortnightly => "Fortnightly",
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };
    }
}
