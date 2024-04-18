namespace Notification.Shared.Configurations;

public class NotificationConfiguration
{
    public const string Key = "Notification";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? BaseUri { get; set; }
}
