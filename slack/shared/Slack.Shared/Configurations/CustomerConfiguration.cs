namespace Slack.Shared.Configurations;

public class CustomerConfiguration
{
    public const string Key = "Customer";

    public string ApiKey { get; set; }
    public Uri? GrpcUrl { get; set; }
}
