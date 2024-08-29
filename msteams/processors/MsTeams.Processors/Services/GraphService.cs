using Enterprise.Shared.Azure.Graph;
using Microsoft.Graph.Models;

namespace MsTeams.Processors.Services;

public interface IGraphService
{
    Task<List<Team>> GetTeamsAsync(string tenantId, CancellationToken cancellationToken);
    Task<List<Channel>> GetTeamChannelsAsync(string tenantId, string teamId, CancellationToken cancellationToken);
}

public class GraphService(IGraphServiceClientFactory graphServiceClientFactory) : IGraphService
{
    public async Task<List<Team>> GetTeamsAsync(string tenantId, CancellationToken cancellationToken)
    {
        var graphServiceClient = graphServiceClientFactory.CreateGraphServiceClient(tenantId);
        var teams = new List<Team>();
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
                teams.AddRange(response.Value);
            }

            if (response is null || string.IsNullOrWhiteSpace(response.OdataNextLink))
            {
                break;
            }
        } while (true);

        return teams;
    }

    public async Task<List<Channel>> GetTeamChannelsAsync(
        string tenantId,
        string teamId,
        CancellationToken cancellationToken)
    {
        var graphServiceClient = graphServiceClientFactory.CreateGraphServiceClient(tenantId);
        var channels = new List<Channel>();
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

        if (response is not null)
        {
            ArgumentNullException.ThrowIfNull(response.Value);
            channels.AddRange(response.Value.Where(item => !(bool)item.AdditionalData["isArchived"]));
        }

        return channels;
    }
}
