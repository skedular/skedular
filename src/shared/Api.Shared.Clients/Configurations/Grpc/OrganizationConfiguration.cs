namespace Api.Shared.Clients.Configurations.Grpc;

public class OrganizationConfiguration
{
    public const string Key = "Organization";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? GrpcUrl { get; set; }
}
