using Microsoft.Graph.Models;
using MsTeams.Shared.Factories;

namespace MsTeams.Shared.Services;

public interface IMsGraphService
{
    Task<List<User>> GetUsersAsync(string tenantId, CancellationToken cancellationToken);
}

public class MsGraphService(IGraphServiceClientFactory graphServiceClientFactory) : IMsGraphService
{
    private static readonly string[] s_userProperties =
    [
        "id",
        "givenName",
        "surname",
        "jobTitle",
        "mail",
        "userPrincipalName"
    ];

    public async Task<List<User>> GetUsersAsync(string tenantId, CancellationToken cancellationToken)
    {
        var users = new List<User>();
        var skipCount = 0;

        var graphServiceClient = graphServiceClientFactory.CreateGraphServiceClient(tenantId);

        do
        {
            var userCollectionResponse = await graphServiceClient.Users
                .GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Count = true;
                        requestConfiguration.QueryParameters.Select = s_userProperties;
                        _ = requestConfiguration.QueryParameters.Select.Skip(skipCount);
                    },
                    cancellationToken);

            if (userCollectionResponse is not null)
            {
                ArgumentNullException.ThrowIfNull(userCollectionResponse.Value);
                skipCount += userCollectionResponse.Value.Count;
                users.AddRange(userCollectionResponse.Value);
            }

            if (userCollectionResponse is null || string.IsNullOrWhiteSpace(userCollectionResponse.OdataNextLink))
            {
                break;
            }
        } while (true);

        return users;
    }
}
