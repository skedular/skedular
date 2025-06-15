using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Team.V1;
using Enterprise.Shared.Grpc;
using Slack.Api.Mappers;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Repositories;
using SlackNet.Blocks;
using SlackNet.Interaction;
using OrderDirection = Api.Shared.Services.Grpc.Skedular.Team.V1.OrderDirection;

namespace Slack.Api.Handlers.OptionProviders;

public class OrganizationTeamOptionProvider(
    TeamConfiguration teamConfiguration,
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    TeamService.TeamServiceClient teamServiceClient)
    : IBlockOptionProvider
{
    public async Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request)
    {
        var cancellationToken = CancellationToken.None;
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var getPaginatedMembersInput = new GetPaginatedTeamsInput
        {
            First = 100,
            After = string.Empty,
            Last = -1,
            Before = string.Empty,
            Where = new TeamWhereInput { OrganizationId = workspaceEntity.Organization.Id, NameContains = request.Value }
        };

        getPaginatedMembersInput.OrderBy.Add(new TeamOrderInput { Direction = OrderDirection.Ascending, Field = TeamOrderField.Name });

        var memberConnection = await teamServiceClient.GetPaginatedTeamsAsync(
            getPaginatedMembersInput,
            teamConfiguration.ApiKey.CreateMetadata(request.User.Id),
            cancellationToken: cancellationToken);

        return new BlockOptionsResponse
        {
            Options = memberConnection.Edges
                .Select(item => mapper.MapTo(item.Node))
                .Select(item => new Option { Text = item.Name.ToOptionText(), Value = item.Id })
                .ToList()
        };
    }
}
