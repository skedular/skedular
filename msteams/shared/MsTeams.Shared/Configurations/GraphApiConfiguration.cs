namespace MsTeams.Shared.Configurations;

public sealed class GraphApiConfiguration
{
    public const string Key = "GraphAPI";

    public string? Endpoint { get; set; }
    public string? DefaultScope { get; set; }
    public string? Scopes { get; set; }
}
