using Azure.Identity;
using Microsoft.Graph;
using MsTeams.Shared.Configurations;

namespace MsTeams.Shared.Factories;

public interface IGraphServiceClientFactory
{
    GraphServiceClient CreateGraphServiceClient(string tenantId);
}

public class CreateGraphServiceClientFactory(AzureEntraConfiguration azureEntraOptions)
    : IGraphServiceClientFactory
{
    private static readonly string[] s_scopes = ["https://graph.microsoft.com/.default"];

    public GraphServiceClient CreateGraphServiceClient(string tenantId)
    {
        var clientSecretCredential = new ClientSecretCredential(
            tenantId,
            azureEntraOptions.ClientId,
            azureEntraOptions.ClientSecret,
            new ClientSecretCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud });
        return new GraphServiceClient(clientSecretCredential, s_scopes);
    }
}
