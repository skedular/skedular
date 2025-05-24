namespace Enterprise.Shared.Azure.Configurations;

public class AzureEntraConfiguration
{
    public const string Key = "AzureEntra";

    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
