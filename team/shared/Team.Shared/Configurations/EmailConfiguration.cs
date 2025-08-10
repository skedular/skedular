namespace Team.Shared.Configurations;

public class EmailConfiguration
{
    public const string Key = "Email";

    public string InviteToJoinTeamNewCustomerEmailSender { get; set; } = string.Empty;
    public string InviteToJoinTeamExistingCustomerEmailSender { get; set; } = string.Empty;
}
