using Api.Shared.Services;
using Slack.Shared.Constants;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace Slack.Api.Handlers.OptionProviders;

public class OrganizationMemberAndCustomerPairOptionProvider(
    IRepositoryFactory repositoryFactory,
    IOrganizationMemberService organizationMemberService)
    : IBlockOptionProvider
{
    public async Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request)
    {
        var cancellationToken = CancellationToken.None;
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var connection = await organizationMemberService.GetPaginatedMembersAsync(
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
            Options = connection.Edges
                .Select(item => item.Node)
                .Select(item => new Option
                {
                    Text = string.IsNullOrWhiteSpace(item.Customer.DisplayableName) ? "???" : item.Customer.DisplayableName.ToOptionText(),
                    Value = $"{item.Id}{Global.OptionLoaderValueSeparator}{item.Customer.Id}"
                })
                .ToList()
        };
    }
}
