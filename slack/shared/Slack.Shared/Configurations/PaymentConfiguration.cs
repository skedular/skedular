namespace Slack.Shared.Configurations;

public class PaymentConfiguration
{
    public const string Key = "Payment";

    public string ApiKey { get; set; }
    public Uri? GrpcUrl { get; set; }
}
