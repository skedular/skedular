namespace Slack.Shared.Configurations;

public class EmailConfiguration
{
    public const string Key = "Email";

    public IReadOnlyCollection<string> NewSlackWorkspaceJoinedEmailReceivers { get; set; } = [];
    public string NewSlackWorkspaceJoinedEmailTemplateName { get; set; } = string.Empty;
    public string NewSlackWorkspaceJoinedEmailSender { get; set; } = string.Empty;
    public bool EnableNewSlackWorkspaceJoinedEmail { get; set; }
}
