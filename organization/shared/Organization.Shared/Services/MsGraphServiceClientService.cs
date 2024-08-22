using Azure.Identity;
using Enterprise.Shared.Configurations;
using Microsoft.Graph;

namespace Organization.Shared.Services;

public interface IMsGraphServiceClientService
{
    GraphServiceClient CreateGraphServiceClient(string tenantId);
}

public class MsGraphServiceClientService(AzureEntraConfiguration azureEntraOptions)
    : IMsGraphServiceClientService
{
    public GraphServiceClient CreateGraphServiceClient(string tenantId)
    {
        var clientSecretCredential = new ClientSecretCredential(
            tenantId,
            azureEntraOptions.ClientId,
            azureEntraOptions.ClientSecret,
            new ClientSecretCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud });
        return new GraphServiceClient(clientSecretCredential, ["https://graph.microsoft.com/.default"]);
    }
}
