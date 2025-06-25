namespace Api.Shared.Clients.Configurations.Grpc;

public class SlackConfiguration
{
    public const string Key = "Slack";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? GrpcUrl { get; set; }
}
