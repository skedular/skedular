namespace Api.Shared.Services.Configurations.Grpc;

public class SlackConfiguration
{
    public const string Key = "Slack";

    public string ApiKey { get; set; } = string.Empty;
}
