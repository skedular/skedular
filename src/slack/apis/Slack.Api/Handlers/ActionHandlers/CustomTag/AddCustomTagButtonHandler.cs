using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Random;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace Slack.Api.Handlers.ActionHandlers.CustomTag;

public class AddCustomTagButtonHandler(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfigurationService slackConfigurationService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IEntityMapper entityMapper,
    IRandomHelper randomHelper,
    IPageNavigator pageNavigator,
    IOrganizationCustomTagService organizationCustomTagService)
    : IAsyncPageRenderingCallbacks, IBlockActionHandler<ButtonAction>, IViewSubmissionHandler
{
    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        _ = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = entityMapper.MapTo(workspaceEntity);
        var name = new InputBlock
        {
            BlockId = CustomTagActionTypes.Name,
            Label = "Name".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = CustomTagActionTypes.Name,
            },
            Optional = false,
        };

        var description = new InputBlock
        {
            BlockId = CustomTagActionTypes.Description,
            Label = "Description".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = CustomTagActionTypes.Description,
                Multiline = true,
            },
            Optional = true,
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            request.TriggerId,
            new ModalViewDefinition
            {
                CallbackId = CustomTagCallbackTypes.AddCustomTag,
                Title = "Add Tag",
                Close = "Cancel",
                Submit = "Add",
                Blocks = [name, description],
                PrivateMetadata = action.Value,
            },
            cancellationToken);
    }

    public async Task Handle(ButtonAction action, BlockActionRequest request)
    {
        if (slackConfigurationService.EnableAsyncMode)
        {
            asyncPageRenderingService.ButtonActionHandlerStream.OnNext((GetType(), action, request));
        }
        else
        {
            await HandleAsync(action, request, CancellationToken.None);
        }
    }

    public async Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission)
    {
        var cancellationToken = CancellationToken.None;
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(viewSubmission.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            viewSubmission.User.Id,
            cancellationToken);

        var workspace = entityMapper.MapTo(workspaceEntity);
        var workspaceMember = entityMapper.MapTo(workspaceMemberEntity, workspace);
        var context = AddCustomTagContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var values = viewSubmission.View.State.Values;
        var organizationCustomTag = new OrganizationCustomTag
        {
            Id = randomHelper.Generate(),
            Organization = new Organization
            {
                Id = workspace.Organization.Id,
            },
        };

        if (values.TryGetValue(CustomTagActionTypes.Name, out var nameBlock))
        {
            if (nameBlock.TryGetValue(CustomTagActionTypes.Name, out var block))
            {
                if (block is PlainTextInputValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.Value);
                    organizationCustomTag.Name = value.Value.ToSafeString();
                }
                else
                {
                    throw new InvalidOperationException("name must be PlainTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("name block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("name block is missing");
        }

        if (values.TryGetValue(CustomTagActionTypes.Description, out var descriptionBlock))
        {
            if (descriptionBlock.TryGetValue(CustomTagActionTypes.Description, out var block))
            {
                if (block is PlainTextInputValue value)
                {
                    organizationCustomTag.Description = value.Value.ToSafeString();
                }
                else
                {
                    throw new InvalidOperationException("description must be PlainTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("description block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("description block is missing");
        }

        await organizationCustomTagService.AddAsync(workspaceMember.Id, organizationCustomTag, cancellationToken);

        await pageNavigator.BackAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            viewSubmission.Hash, cancellationToken);

        return ViewSubmissionResponse.Null;
    }

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;
}
