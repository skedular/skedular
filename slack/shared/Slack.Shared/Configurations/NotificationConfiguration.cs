namespace Slack.Shared.Configurations;

public class NotificationConfiguration
{
    public const string Key = "Notification";

    public string ApiKey { get; set; }
    public Uri? GrpcUrl { get; set; }
}
