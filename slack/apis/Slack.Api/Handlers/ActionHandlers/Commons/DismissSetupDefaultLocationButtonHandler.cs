using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared.Configurations;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using SlackNet.Blocks;
using SlackNet.Interaction;
using CustomerService = Api.Shared.Services.Grpc.UnityHub.Customer.V1.CustomerService;

namespace Slack.Api.Handlers.ActionHandlers.Commons;

public class DismissSetupDefaultLocationButtonHandler(
    CustomerConfiguration customerConfiguration,
    CustomerService.CustomerServiceClient customerServiceClient,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IMapper mapper,
    IPageNavigator pageNavigator) : IBlockActionHandler<ButtonAction>
{
    public async Task Handle(ButtonAction action, BlockActionRequest request)
    {
        var cancellationToken = CancellationToken.None;

        var workspaceEntity =
            await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, _) =
            await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
                workspaceEntity,
                request.User.Id,
                cancellationToken);

        await customerServiceClient.DismissDefaultLocationOnboardingSetupAsync(
            new DismissDefaultLocationOnboardingSetupInput(),
            customerConfiguration.ApiKey.CreateMetadata(workspaceMemberEntity.Id),
            cancellationToken: cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var context = CommonPageContext.Deserialize(action.Value);

        await pageNavigator.BackAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            request.View.Hash,
            cancellationToken);
    }
}
