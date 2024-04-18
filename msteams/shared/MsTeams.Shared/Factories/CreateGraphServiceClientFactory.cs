using Azure.Identity;
using Microsoft.Graph;
using MsTeams.Shared.Configurations;

namespace MsTeams.Shared.Factories;

public interface IGraphServiceClientFactory
{
    GraphServiceClient CreateGraphServiceClient(string tenantId);
}

public class CreateGraphServiceClientFactory(AzureAdConfiguration azureAdOptions, GraphApiConfiguration graphApiOptions)
    : IGraphServiceClientFactory
{
    public GraphServiceClient CreateGraphServiceClient(string tenantId)
    {
        var scopes = new[] { graphApiOptions.DefaultScope };
        var clientSecretCredential = new ClientSecretCredential(
            tenantId,
            azureAdOptions.ClientId,
            azureAdOptions.ClientSecret,
            new ClientSecretCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud });
        return new GraphServiceClient(clientSecretCredential, scopes);
    }
}
