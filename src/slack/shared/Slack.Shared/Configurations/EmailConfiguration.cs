namespace Slack.Shared.Configurations;

public class EmailConfiguration
{
    public const string Key = "Email";

    public IReadOnlyList<string> NewSlackWorkspaceJoinedEmailReceivers { get; set; } = [];
    public string NewSlackWorkspaceJoinedEmailSender { get; set; } = string.Empty;
    public bool EnableNewSlackWorkspaceJoinedEmail { get; set; }
}
