namespace Slack.Shared.Configurations;

public class BillingConfiguration
{
    public const string Key = "Billing";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? GrpcUrl { get; set; }
}
