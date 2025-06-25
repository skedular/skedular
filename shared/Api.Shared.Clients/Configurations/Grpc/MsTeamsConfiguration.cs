namespace Api.Shared.Clients.Configurations.Grpc;

public class MsTeamsConfiguration
{
    public const string Key = "MsTeams";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? GrpcUrl { get; set; }
}
