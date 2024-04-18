using SlackNet.Interaction;

namespace Slack.Api.Handlers.ActionHandlers.Billing;

public class ViewBillingButtonHandler : IViewSubmissionHandler
{
    public Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission) =>
        Task.FromResult(ViewSubmissionResponse.Null);

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;
}
