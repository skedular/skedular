using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Enterprise.Shared.Grpc;
using Slack.Shared.Constants;
using Slack.Shared.Repositories;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace Slack.Api.Handlers.OptionProviders;

public class OrganizationResourceTypeOptionProvider(
    OrganizationConfiguration organizationConfiguration,
    IRepositoryFactory repositoryFactory,
    OrganizationService.OrganizationServiceClient organizationServiceClient)
    : IBlockOptionProvider
{
    public async Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request)
    {
        var cancellationToken = CancellationToken.None;
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var getInput = new GetInput { Id = workspaceEntity.Organization.Id };

        var organization = await organizationServiceClient.GetAsync(
            getInput,
            organizationConfiguration.ApiKey.CreateMetadata(request.User.Id),
            cancellationToken: cancellationToken);

        return new BlockOptionsResponse
        {
            Options = organization.ResourceTypes.Select(item => new Option { Text = item.Name.ToOptionText(), Value = item.Id }).ToList()
        };
    }
}
