using Azure.Identity;
using Enterprise.Shared.Configurations;
using Microsoft.Graph;

namespace MsTeams.Shared.Services;

public interface IMsGraphServiceClientService
{
    GraphServiceClient CreateGraphServiceClient(string tenantId);
}

public class MsGraphServiceClientService(MsTeamsAzureEntraConfiguration msTeamsAzureEntraOptions)
    : IMsGraphServiceClientService
{
    public GraphServiceClient CreateGraphServiceClient(string tenantId)
    {
        var clientSecretCredential = new ClientSecretCredential(
            tenantId,
            msTeamsAzureEntraOptions.ClientId,
            msTeamsAzureEntraOptions.ClientSecret,
            new ClientSecretCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud });
        return new GraphServiceClient(clientSecretCredential, ["https://graph.microsoft.com/.default"]);
    }
}
