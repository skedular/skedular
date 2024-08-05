using Azure.Identity;
using Enterprise.Shared.Configurations;
using Microsoft.Graph;

namespace MsTeams.Shared.Factories;

public interface IGraphServiceClientFactory
{
    GraphServiceClient CreateGraphServiceClient(string tenantId);
}

public class CreateGraphServiceClientFactory(MsTeamsAzureEntraConfiguration msTeamsAzureEntraOptions)
    : IGraphServiceClientFactory
{
    private static readonly string[] s_scopes = ["https://graph.microsoft.com/.default"];

    public GraphServiceClient CreateGraphServiceClient(string tenantId)
    {
        var clientSecretCredential = new ClientSecretCredential(
            tenantId,
            msTeamsAzureEntraOptions.ClientId,
            msTeamsAzureEntraOptions.ClientSecret,
            new ClientSecretCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud });
        return new GraphServiceClient(clientSecretCredential, s_scopes);
    }
}
