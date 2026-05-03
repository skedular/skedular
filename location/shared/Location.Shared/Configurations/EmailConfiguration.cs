namespace Location.Shared.Configurations;

public class EmailConfiguration
{
    public const string Key = "Email";

    public IReadOnlyList<string> NewLocationJoinedEmailReceivers { get; set; } = [];
    public string NewLocationJoinedEmailSender { get; set; } = string.Empty;
    public bool EnableNewLocationJoinedEmail { get; set; }
}
