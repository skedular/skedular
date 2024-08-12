namespace MsTeams.Shared.Configurations;

public class LocationConfiguration
{
    public const string Key = "Location";

    public string ApiKey { get; set; }
    public Uri? GrpcUrl { get; set; }
}
