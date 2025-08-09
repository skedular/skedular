namespace Customer.Shared.Configurations;

public class EmailConfiguration
{
    public const string Key = "Email";

    public ICollection<string> NewCustomerFeedbackSubmittedEmailReceivers { get; set; } = [];
    public string NewCustomerFeedbackSubmittedEmailSender { get; set; } = string.Empty;
    public ICollection<string> NewCustomerJoinedEmailReceivers { get; set; } = [];
    public string NewCustomerJoinedEmailSender { get; set; } = string.Empty;
    public bool EnableNewCustomerJoinedEmail { get; set; }
}
