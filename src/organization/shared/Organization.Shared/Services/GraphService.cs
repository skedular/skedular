using Enterprise.Shared.Azure.Graph;
using Organization.Shared.Mappers;
using Organization.Shared.Models;

namespace Organization.Shared.Services;

public interface IGraphService
{
    Task<IReadOnlyCollection<AzureTenantMember>> GetAzureTenantMembersAsync(string tenantId, CancellationToken cancellationToken);
}

public class GraphService(IGraphServiceClientFactory graphServiceClientFactory, IEntityMapper entityMapper) : IGraphService
{
    public async Task<IReadOnlyCollection<AzureTenantMember>> GetAzureTenantMembersAsync(string tenantId, CancellationToken cancellationToken)
    {
        var graphServiceClient = graphServiceClientFactory.CreateGraphServiceClient(tenantId);
        var users = new List<AzureTenantMember>();
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
                            "photos",
                        ];
                        _ = requestConfiguration.QueryParameters.Select.Skip(skipCount);
                    },
                    cancellationToken);

            if (response is not null)
            {
                ArgumentNullException.ThrowIfNull(response.Value);
                skipCount += response.Value.Count;
                users.AddRange(response.Value.Select(entityMapper.MapTo));
            }

            if (response is null || string.IsNullOrWhiteSpace(response.OdataNextLink))
            {
                break;
            }
        } while (true);

        return users;
    }
}
