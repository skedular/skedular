namespace Slack.Shared.Configurations;

public class BillingConfiguration
{
    public const string Key = "Billing";

    public string ApiKey { get; set; }
    public Uri? GrpcUrl { get; set; }
}
