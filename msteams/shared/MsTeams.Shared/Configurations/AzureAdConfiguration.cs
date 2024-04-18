namespace MsTeams.Shared.Configurations;

public class AzureAdConfiguration
{
    public const string Key = "AzureAd";

    public string? Instance { get; set; }
    public string? Domain { get; set; }
    public string? TenantId { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? CallbackPath { get; set; }
    public string? SignedOutCallbackPath { get; set; }
    public List<string>? ClientCapabilities { get; set; }
}
