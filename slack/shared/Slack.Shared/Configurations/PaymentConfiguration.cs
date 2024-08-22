namespace Slack.Shared.Configurations;

public class PaymentConfiguration
{
    public const string Key = "Payment";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? GrpcUrl { get; set; }
}
