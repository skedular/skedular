namespace MsTeams.Shared.Configurations;

public class OrganizationConfiguration
{
    public const string Key = "Organization";

    public string ApiKey { get; set; }
    public Uri? GrpcUrl { get; set; }
}
