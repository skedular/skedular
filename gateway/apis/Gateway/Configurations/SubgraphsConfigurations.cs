namespace Gateway.Configurations;

public class SubgraphsConfigurations
{
    public const string Key = "Subgraphs";

    public UrlConfig Billing { get; set; } = new();
    public UrlConfig Booking { get; set; } = new();
    public UrlConfig Customer { get; set; } = new();
    public UrlConfig Location { get; set; } = new();
    public UrlConfig Marketplace { get; set; } = new();
    public UrlConfig MsTeams { get; set; } = new();
    public UrlConfig Notification { get; set; } = new();
    public UrlConfig Organization { get; set; } = new();
    public UrlConfig Payment { get; set; } = new();
    public UrlConfig Slack { get; set; } = new();
    public UrlConfig Team { get; set; } = new();
    public UrlConfig Core { get; set; } = new();
}

public class UrlConfig
{
    public Uri? Uri { get; set; }
}
