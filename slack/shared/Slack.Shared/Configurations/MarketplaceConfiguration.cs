namespace Slack.Shared.Configurations;

public class MarketplaceConfiguration
{
    public const string Key = "Marketplace";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? GrpcUrl { get; set; }
}
