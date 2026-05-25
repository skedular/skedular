namespace Customer.Shared.Models;

public class CustomerReadinessState
{
    public static readonly IReadOnlySet<string> RequiredDomains =
        new HashSet<string>(StringComparer.Ordinal)
        {
            Domains.Booking,
            Domains.Organization,
            Domains.Team,
            Domains.Marketplace,
            Domains.Location,
            Domains.Core,
            Domains.Slack,
            Domains.MsTeams
        };

    public required string CustomerId { get; init; }
    public IReadOnlyList<string> ProvisionedDomains { get; init; } = [];
    public bool IsReadyEverywhere => RequiredDomains.All(ProvisionedDomains.Contains);

    public static class Domains
    {
        public const string Booking = "Booking";
        public const string Organization = "Organization";
        public const string Team = "Team";
        public const string Marketplace = "Marketplace";
        public const string Location = "Location";
        public const string Core = "Core";
        public const string Slack = "Slack";
        public const string MsTeams = "MsTeams";
    }
}
