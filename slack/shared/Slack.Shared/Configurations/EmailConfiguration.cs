namespace Slack.Shared.Configurations;

public class EmailConfiguration
{
    public const string Key = "Email";

    public IReadOnlyCollection<string> NewSlackWorkspaceJoinedThroughWebSubmittedEmailReceivers { get; set; } = [];
    public string NewSlackWorkspaceJoinedThroughWebSubmittedEmailTemplateName { get; set; } = string.Empty;
    public string NewSlackWorkspaceJoinedThroughWebSubmittedEmailSender { get; set; } = string.Empty;
}
