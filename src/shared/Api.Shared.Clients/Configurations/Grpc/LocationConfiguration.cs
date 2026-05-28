namespace Api.Shared.Clients.Configurations.Grpc;

public class LocationConfiguration
{
    public const string Key = "Location";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? GrpcUrl { get; set; }
}
