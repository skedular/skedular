namespace Api.Shared.Clients.Configurations.Grpc;

public class NotificationConfiguration
{
    public const string Key = "Notification";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? GrpcUrl { get; set; }
}
