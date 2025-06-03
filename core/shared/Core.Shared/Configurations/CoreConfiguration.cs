namespace Core.Shared.Configurations;

public class CoreConfiguration
{
    public const string Key = "Core";

    public string ApiKey { get; set; } = string.Empty;
    public Uri? GrpcUrl { get; set; }
}
