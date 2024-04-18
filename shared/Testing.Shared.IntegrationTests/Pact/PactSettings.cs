namespace Testing.Shared.IntegrationTests.Pact;

public class PactSettings(string tempPactDirectory)
{
    public string ConsumerName { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public string PactDirectory { get; set; } = string.Empty;
    public string TempPactDirectory { get; } = tempPactDirectory;
    public int Port { get; set; }
}
