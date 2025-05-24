using Azure.Identity;
using Enterprise.Shared.Configurations;
using Microsoft.Graph;

namespace Enterprise.Shared.Azure.Graph;

public interface IGraphServiceClientFactory
{
    GraphServiceClient CreateGraphServiceClient(string tenantId);
}

public class GraphServiceClientFactory(AzureEntraConfiguration azureEntraOptions) : IGraphServiceClientFactory
{
    public GraphServiceClient CreateGraphServiceClient(string tenantId) =>
        new(new ClientSecretCredential(
                tenantId,
                azureEntraOptions.ClientId,
                azureEntraOptions.ClientSecret,
                new ClientSecretCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud }),
            ["https://graph.microsoft.com/.default"]);
}
