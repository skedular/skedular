using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Slack.Shared.Services;
using Slack.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Slack.Shared.UnitTests.Services.TemporalOutboxServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartWorkflowReSyncSlackWorkspaceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Enqueue_Workflow_With_Correct_Options(
        [Frozen]
        TemporalConfiguration temporalConfiguration,
        [Frozen]
        IWorkflowIdService workflowIdService,
        [Frozen]
        ITemporalOutboxWorkflowExecutor temporalOutboxWorkflowExecutor,
        TemporalOutboxService sut,
        ReSyncSlackWorkspaceInput args,
        IUnitOfWork unitOfWork,
        string expectedId)
    {
        A.CallTo(() => workflowIdService.ReSyncSlackWorkspace(args.WorkspaceId)).Returns(expectedId);

        sut.StartWorkflowReSyncSlackWorkspace(args, unitOfWork);

        A.CallTo(() => temporalOutboxWorkflowExecutor.Execute<ReSyncSlackWorkspace, ReSyncSlackWorkspaceInput>(
            args,
            A<WorkflowOptions>.That.Matches(options =>
                options.Id == expectedId &&
                options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicateFailedOnly),
            unitOfWork)).MustHaveHappenedOnceExactly();
    }
}
