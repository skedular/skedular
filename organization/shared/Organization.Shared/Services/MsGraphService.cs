using Microsoft.Graph.Models;

namespace Organization.Shared.Services;

public interface IMsGraphService
{
    Task<List<User>> GetUsersAsync(string tenantId, CancellationToken cancellationToken);
}

public class MsGraphService(IMsGraphServiceClientService msGraphServiceClientService) : IMsGraphService
{
    public async Task<List<User>> GetUsersAsync(string tenantId, CancellationToken cancellationToken)
    {
        var graphServiceClient = msGraphServiceClientService.CreateGraphServiceClient(tenantId);
        var users = new List<User>();
        var skipCount = 0;

        do
        {
            var userCollectionResponse = await graphServiceClient.Users
                .GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Count = true;
                        requestConfiguration.QueryParameters.Select =
                        [
                            "id",
                            "mail",
                            "jobTitle",
                            "displayName",
                            "givenName",
                            "surname",
                            "photo",
                            "photos"
                        ];
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
