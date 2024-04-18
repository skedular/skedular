using Microsoft.Graph.Models;
using MsTeams.Shared.Factories;

namespace MsTeams.Shared.Services;

public interface IMsGraphService
{
    Task<List<User>> GetUsersAsync(string tenantId, CancellationToken cancellationToken);
}

public class MsGraphService(IGraphServiceClientFactory graphServiceClientFactory) : IMsGraphService
{
    public async Task<List<User>> GetUsersAsync(string tenantId, CancellationToken cancellationToken)
    {
        var users = new List<User>();
        var skipCount = 0;
        var selectProperties = new List<string>
        {
            "id",
            "givenName",
            "surname",
            "jobTitle",
            "mail",
            "userPrincipalName"
        };

        var graphServiceClient = graphServiceClientFactory.CreateGraphServiceClient(tenantId);
        UserCollectionResponse userCollectionResponse;
        do
        {
            userCollectionResponse = await graphServiceClient.Users
                .GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Count = true;
                        requestConfiguration.QueryParameters.Select = selectProperties.ToArray();
                        requestConfiguration.QueryParameters.Select.Skip(skipCount);
                    },
                    cancellationToken);

            if (userCollectionResponse != null)
            {
                skipCount += userCollectionResponse.Value.Count;
                users.AddRange(userCollectionResponse.Value);
            }
        } while (!string.IsNullOrWhiteSpace(userCollectionResponse?.OdataNextLink));

        return users;
    }
}
