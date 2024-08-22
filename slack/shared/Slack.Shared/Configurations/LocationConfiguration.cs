namespace Slack.Shared.Configurations;

public class LocationConfiguration
{
    public const string Key = "Location";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? GrpcUrl { get; set; }
}
