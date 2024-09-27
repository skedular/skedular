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

public class DismissSetupPreferredZonesButtonHandler(
    AsyncPageRenderingService asyncPageRenderingService,
    CustomerConfiguration customerConfiguration,
    CustomerService.CustomerServiceClient customerServiceClient,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IMapper mapper,
    IPageNavigator pageNavigator) : IAsyncPageRenderingCallbacks, IBlockActionHandler<ButtonAction>
{
    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
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

        await customerServiceClient.DismissSetupPreferredZonesAsync(
            new DismissSetupPreferredZonesInput(),
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

    public Task Handle(ButtonAction action, BlockActionRequest request)
    {
        asyncPageRenderingService.ButtonActionHandlerStream.OnNext((GetType(), action, request));

        return Task.CompletedTask;
    }
}
