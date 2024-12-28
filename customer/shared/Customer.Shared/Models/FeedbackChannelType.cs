namespace Customer.Shared.Models;

public enum FeedbackChannelType
{
    Web = 0,
    Slack = 1,
    MsTeams = 2
}

public static class FeedbackChannelTypeConstants
{
    public const string Web = "WEB";
    public const string Slack = "SLACK";
    public const string MsTeams = "MSTEAMS";
}
