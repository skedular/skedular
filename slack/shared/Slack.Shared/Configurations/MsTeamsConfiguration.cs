namespace Slack.Shared.Configurations;

public class MsTeamsConfiguration
{
    public const string Key = "MsTeams";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? GrpcUrl { get; set; }
}
