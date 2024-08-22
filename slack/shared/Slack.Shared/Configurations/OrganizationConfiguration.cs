namespace Slack.Shared.Configurations;

public class OrganizationConfiguration
{
    public const string Key = "Organization";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? GrpcUrl { get; set; }
}
