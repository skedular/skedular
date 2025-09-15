using Api.Shared.Services;
using Enterprise.Shared;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace Slack.Api.Handlers.ActionHandlers.Feedback;

public class SendUsFeedbackButtonHandler(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfigurationService slackConfigurationService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IMapper mapper,
    IPageNavigator pageNavigator,
    ICustomerService customerService)
    : IAsyncPageRenderingCallbacks, IBlockActionHandler<ButtonAction>, IViewSubmissionHandler
{
    private const string FeedbackKey = "Feedback";

    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var greetings = new SectionBlock { Text = $"Hi <@{workspaceMemberEntity.Id}>, what feedback would you like to share with us?".ToMarkdown() };

        var feedback = new InputBlock
        {
            BlockId = FeedbackKey,
            Label = "Feedback".ToPlainText(),
            Element = new PlainTextInput { ActionId = FeedbackKey, Multiline = true },
            Optional = false
        };

        var notes = new SectionBlock
        {
            Text =
                "We value your feedback, whether it's big or small. Sometimes, it's the smallest details that distinguish a great product from a mediocre one. If you notice something missing or something that bothers you, please let us know, and we'll address it promptly!"
                    .ToMarkdown()
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            request.TriggerId,
            new ModalViewDefinition
            {
                CallbackId = CommonCallbackTypes.SendUsFeedback,
                Title = "Send us feedback",
                Close = "Cancel",
                Submit = "Send",
                Blocks = [greetings, feedback, notes],
                PrivateMetadata = action.Value
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

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var context = CommonPageContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var values = viewSubmission.View.State.Values;
        string feedback;

        if (values.TryGetValue(FeedbackKey, out var notesBlock))
        {
            if (notesBlock.TryGetValue(FeedbackKey, out var notes))
            {
                if (notes is PlainTextInputValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.Value);
                    feedback = value.Value.ToSafeString();
                }
                else
                {
                    throw new InvalidOperationException("feedback must be PlainTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("feedback block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("feedback block is missing");
        }

        await customerService.SubmitFeedbackAsync(workspaceMember, feedback, cancellationToken);

        await pageNavigator.BackAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            viewSubmission.Hash,
            cancellationToken);

        return ViewSubmissionResponse.Null;
    }

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;
}
