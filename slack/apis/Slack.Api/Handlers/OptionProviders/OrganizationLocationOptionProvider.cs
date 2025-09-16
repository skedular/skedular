using Api.Shared.Services;
using Slack.Shared.Constants;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace Slack.Api.Handlers.OptionProviders;

public class OrganizationLocationOptionProvider(IRepositoryFactory repositoryFactory, ILocationService locationService) : IBlockOptionProvider
{
    public async Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request)
    {
        var cancellationToken = CancellationToken.None;
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (locations, _) = await locationService.GetPaginatedLocationsAsync(
            request.User.Id,
            workspaceEntity.Organization.Id,
            request.Value,
            null,
            100,
            null,
            null,
            cancellationToken);

        return new BlockOptionsResponse
        {
            Options = locations.Select(item => new Option { Text = item.Name.ToOptionText(), Value = item.Id }).ToList()
        };
    }
}
