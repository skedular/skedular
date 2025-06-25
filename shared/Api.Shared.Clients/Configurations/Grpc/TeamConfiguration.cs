namespace Api.Shared.Clients.Configurations.Grpc;

public class TeamConfiguration
{
    public const string Key = "Team";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? GrpcUrl { get; set; }
}
