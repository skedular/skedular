namespace Enterprise.Shared.Configurations;

public class MsTeamsAzureEntraConfiguration
{
    public const string Key = "MsTeamsAzureEntra";

    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
