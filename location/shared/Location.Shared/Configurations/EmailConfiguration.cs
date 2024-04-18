namespace Location.Shared.Configurations;

public class EmailConfiguration
{
    public const string Key = "Email";

    public string InviteToJoinLocationNewCustomerEmailTemplateName { get; set; } = string.Empty;
    public string InviteToJoinLocationNewCustomerEmailSender { get; set; } = string.Empty;
    public string InviteToJoinLocationExistingCustomerEmailTemplateName { get; set; } = string.Empty;
    public string InviteToJoinLocationExistingCustomerEmailSender { get; set; } = string.Empty;
}
