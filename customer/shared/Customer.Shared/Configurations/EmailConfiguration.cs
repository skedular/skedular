namespace Customer.Shared.Configurations;

public class EmailConfiguration
{
    public const string Key = "Email";

    public IReadOnlyCollection<string> NewCustomerFeedbackThroughWebSubmittedEmailReceivers { get; set; } = [];
    public string NewCustomerFeedbackThroughWebSubmittedEmailTemplateName { get; set; } = string.Empty;
    public string NewCustomerFeedbackThroughWebSubmittedEmailSender { get; set; } = string.Empty;
    public IReadOnlyCollection<string> NewCustomerJoinedThroughWebSubmittedEmailReceivers { get; set; } = [];
    public string NewCustomerJoinedThroughWebSubmittedEmailTemplateName { get; set; } = string.Empty;
    public string NewCustomerJoinedThroughWebSubmittedEmailSender { get; set; } = string.Empty;
}
