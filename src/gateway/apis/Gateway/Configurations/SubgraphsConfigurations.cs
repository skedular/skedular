using Enterprise.Shared;

namespace Gateway.Configurations;

public class SubgraphsConfigurations : Dictionary<string, SubgraphConfig>
{
    public const string Key = "Subgraphs";
}

public class SubgraphConfig
{
    public string ClientName { get; set; } = string.Empty;
    public Uri Url { get; set; } = Constants.EmptyUri;
}
