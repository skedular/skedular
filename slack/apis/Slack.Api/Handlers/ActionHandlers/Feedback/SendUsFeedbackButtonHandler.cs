using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Enterprise.Shared;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;
using CustomerService = Api.Shared.Services.Grpc.UnityHub.Customer.V1.CustomerService;

namespace Slack.Api.Handlers.ActionHandlers.Feedback;

public class SendUsFeedbackButtonHandler(
    AsyncPageRenderingService<SendUsFeedbackButtonHandler> asyncPageRenderingService,
    CustomerConfiguration customerConfiguration,
    CustomerService.CustomerServiceClient customerServiceClient,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IMapper mapper,
    IRandomHelper randomHelper,
    IPageNavigator pageNavigator)
    : IAsyncPageRenderingCallbacks, IBlockActionHandler<ButtonAction>, IViewSubmissionHandler
{
    private const string FeedbackKey = "Feedback";

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

        var workspace = mapper.MapTo(workspaceEntity);
        var greetings = new SectionBlock
        {
            Text = $"Hi <@{workspaceMemberEntity.Id}>, what feedback would you like to share with us?".ToMarkdown()
        };

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
        await slackApiClient.Views.Open(
            request.TriggerId,
            new ModalViewDefinition
            {
                CallbackId = CommonCallbackTypes.SendUsFeedback,
                Title = "Send us feedback",
                Close = "Cancel",
                Submit = "Send",
                Blocks = [greetings, feedback, notes],
                PrivateMetadata = action.Value
            });
    }

    public Task Handle(ButtonAction action, BlockActionRequest request)
    {
        asyncPageRenderingService.ButtonActionHandlerStream.OnNext((action, request));

        return Task.CompletedTask;
    }

    public async Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission)
    {
        var cancellationToken = CancellationToken.None;

        var workspaceEntity =
            await repositoryFactory.WorkspaceRepository.GetByIdAsync(viewSubmission.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, _) =
            await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
                workspaceEntity,
                viewSubmission.User.Id,
                cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var context = CommonPageContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var submitFeedbackInput = new SubmitFeedbackInput { Id = randomHelper.Generate() };
        var values = viewSubmission.View.State.Values;

        if (values.TryGetValue(FeedbackKey, out var notesBlock))
        {
            if (notesBlock.TryGetValue(FeedbackKey, out var notes))
            {
                if (notes is PlainTextInputValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.Value);
                    submitFeedbackInput.Feedback = value.Value.ToSafeString();
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

        await customerServiceClient.SubmitFeedbackAsync(
            submitFeedbackInput,
            customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        await pageNavigator.BackAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            viewSubmission.Hash, cancellationToken);

        return ViewSubmissionResponse.Null;
    }

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;
}
