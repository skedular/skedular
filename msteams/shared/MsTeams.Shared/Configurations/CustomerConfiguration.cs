namespace MsTeams.Shared.Configurations;

public class CustomerConfiguration
{
    public const string Key = "Customer";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? GrpcUrl { get; set; }
}
