using Enterprise.Shared.Azure.Graph;
using Microsoft.Graph.Models;

namespace Organization.Processors.Services;

public interface IGraphService
{
    Task<List<User>> GetUsersAsync(string tenantId, CancellationToken cancellationToken);
}

public class GraphService(IGraphServiceClientFactory graphServiceClientFactory) : IGraphService
{
    public async Task<List<User>> GetUsersAsync(string tenantId, CancellationToken cancellationToken)
    {
        var graphServiceClient = graphServiceClientFactory.CreateGraphServiceClient(tenantId);
        var users = new List<User>();
        var skipCount = 0;

        do
        {
            var response = await graphServiceClient.Users
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

            if (response is not null)
            {
                ArgumentNullException.ThrowIfNull(response.Value);
                skipCount += response.Value.Count;
                users.AddRange(response.Value);
            }

            if (response is null || string.IsNullOrWhiteSpace(response.OdataNextLink))
            {
                break;
            }
        } while (true);

        return users;
    }
}
