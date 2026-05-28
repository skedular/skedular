namespace Api.Shared.Clients.Configurations.Grpc;

public class CustomerConfiguration
{
    public const string Key = "Customer";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? GrpcUrl { get; set; }
}
