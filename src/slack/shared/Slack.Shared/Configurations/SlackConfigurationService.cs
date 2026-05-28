namespace Slack.Shared.Configurations;

public class SlackConfigurationService
{
    public const string Key = "Slack";

    public string AppId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string SigningSecret { get; set; } = string.Empty;
    public Uri? RedirectUrl { get; set; }
    public Uri? SuccessInstallUrl { get; set; }
    public bool EnableAsyncMode { get; set; }
}
