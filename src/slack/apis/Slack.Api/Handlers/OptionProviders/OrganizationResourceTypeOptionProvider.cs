using Api.Shared.Services;
using Slack.Shared.Constants;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace Slack.Api.Handlers.OptionProviders;

public class OrganizationResourceTypeOptionProvider(IRepositoryFactory repositoryFactory, IOrganizationService organizationService)
    : IBlockOptionProvider
{
    public async Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request)
    {
        var cancellationToken = CancellationToken.None;
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var organization = await organizationService.GetAsync(request.User.Id, workspaceEntity.Organization.Id, cancellationToken);

        return new BlockOptionsResponse
        {
            Options = organization.ResourceTypes.Select(item => new Option { Text = item.Name.ToOptionText(), Value = item.Id }).ToList()
        };
    }
}
