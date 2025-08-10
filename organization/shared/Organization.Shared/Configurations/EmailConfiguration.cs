namespace Organization.Shared.Configurations;

public class EmailConfiguration
{
    public const string Key = "Email";

    public string InviteToJoinOrganizationNewCustomerEmailSender { get; set; } = string.Empty;
    public string InviteToJoinOrganizationExistingCustomerEmailSender { get; set; } = string.Empty;
}
