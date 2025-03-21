namespace Slack.Shared.Configurations;

public class EmailConfiguration
{
    public const string Key = "Email";

    public IReadOnlyCollection<string> NewSlackWorkspaceJoinedThroughWebEmailReceivers { get; set; } = [];
    public string NewSlackWorkspaceJoinedThroughWebEmailTemplateName { get; set; } = string.Empty;
    public string NewSlackWorkspaceJoinedThroughWebEmailSender { get; set; } = string.Empty;
    public bool EnableNewSlackWorkspaceJoinedThroughWebEmail { get; set; }
}
