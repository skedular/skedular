using Enterprise.Shared.Azure.Graph;
using MsTeams.Processors.Mappers;
using MsTeams.Shared.Models;

namespace MsTeams.Processors.Services;

public interface IGraphService
{
    Task<IReadOnlyCollection<AzureTenantTeam>> GetAzureTenantTeamsAsync(
        string tenantId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<AzureTenantTeamChannel>> GetAzureTenantTeamChannelsAsync(
        string tenantId,
        string teamId,
        CancellationToken cancellationToken);
}

public class GraphService(
    IGraphServiceClientFactory graphServiceClientFactory,
    IMapper mapper) : IGraphService
{
    public async Task<IReadOnlyCollection<AzureTenantTeam>> GetAzureTenantTeamsAsync(
        string tenantId,
        CancellationToken cancellationToken)
    {
        var graphServiceClient = graphServiceClientFactory.CreateGraphServiceClient(tenantId);
        var teams = new List<AzureTenantTeam>();
        var skipCount = 0;

        do
        {
            var response = await graphServiceClient.Teams
                .GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Count = true;
                        requestConfiguration.QueryParameters.Select =
                        [
                            "id",
                            "displayName",
                            "description",
                            "photo",
                            "webUrl"
                        ];
                        _ = requestConfiguration.QueryParameters.Select.Skip(skipCount);
                    },
                    cancellationToken);

            if (response is not null)
            {
                ArgumentNullException.ThrowIfNull(response.Value);
                skipCount += response.Value.Count;
                teams.AddRange(response.Value.Select(mapper.MapTo));
            }

            if (response is null || string.IsNullOrWhiteSpace(response.OdataNextLink))
            {
                break;
            }
        } while (true);

        return teams;
    }

    public async Task<IReadOnlyCollection<AzureTenantTeamChannel>> GetAzureTenantTeamChannelsAsync(
        string tenantId,
        string teamId,
        CancellationToken cancellationToken)
    {
        var graphServiceClient = graphServiceClientFactory.CreateGraphServiceClient(tenantId);
        // TODO: 20240829 - Morteza: Need to figure out how to implement pagination  
        var response = await graphServiceClient.Teams[teamId].AllChannels
            .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select =
                    [
                        "id",
                        "displayName",
                        "description",
                        "email",
                        "webUrl",
                        "isArchived"
                    ];
                },
                cancellationToken);

        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(response.Value);

        return response.Value.Where(item => !(bool)item.AdditionalData["isArchived"]).Select(mapper.MapTo).ToList();
    }
}
