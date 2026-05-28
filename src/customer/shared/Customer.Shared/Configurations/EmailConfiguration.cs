namespace Customer.Shared.Configurations;

public class EmailConfiguration
{
    public const string Key = "Email";

    public IReadOnlyList<string> NewCustomerFeedbackSubmittedEmailReceivers { get; set; } = [];
    public string NewCustomerFeedbackSubmittedEmailSender { get; set; } = string.Empty;
    public IReadOnlyList<string> NewCustomerJoinedEmailReceivers { get; set; } = [];
    public string NewCustomerJoinedEmailSender { get; set; } = string.Empty;
    public bool EnableNewCustomerJoinedEmail { get; set; }
}
