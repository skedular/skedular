namespace Customer.Shared.Configurations;

public class EmailConfiguration
{
    public const string Key = "Email";

    public IReadOnlyCollection<string> NewCustomerFeedbackSubmittedEmailReceivers { get; set; } = [];
    public string NewCustomerFeedbackSubmittedEmailTemplateName { get; set; } = string.Empty;
    public string NewCustomerFeedbackSubmittedEmailSender { get; set; } = string.Empty;
    public IReadOnlyCollection<string> NewCustomerJoinedEmailReceivers { get; set; } = [];
    public string NewCustomerJoinedEmailTemplateName { get; set; } = string.Empty;
    public string NewCustomerJoinedEmailSender { get; set; } = string.Empty;
    public bool EnableNewCustomerJoinedEmail { get; set; }
}
