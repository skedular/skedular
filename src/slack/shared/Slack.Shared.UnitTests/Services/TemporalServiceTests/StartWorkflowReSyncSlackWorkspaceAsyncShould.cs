using System.Linq.Expressions;
using Enterprise.Shared.Temporal.Configurations;
using Slack.Shared.Services;
using Slack.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Slack.Shared.UnitTests.Services.TemporalServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartWorkflowReSyncSlackWorkspaceAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Start_Workflow_With_Correct_Options(
        [Frozen]
        TemporalConfiguration temporalConfiguration,
        [Frozen]
        ITemporalClient temporalClient,
        [Frozen]
        IWorkflowIdService workflowIdService,
        WorkflowHandle<ReSyncSlackWorkspace> workflowHandle,
        TemporalService sut,
        ReSyncSlackWorkspaceInput args,
        string expectedId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => workflowIdService.ReSyncSlackWorkspace(args.WorkspaceId)).Returns(expectedId);
        A.CallTo(() => temporalClient.StartWorkflowAsync(
                A<Expression<Func<ReSyncSlackWorkspace, Task>>>._,
                A<WorkflowOptions>.That.Matches(options =>
                    options.Id == expectedId &&
                    options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                    options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicate &&
                    options.IdConflictPolicy == WorkflowIdConflictPolicy.TerminateExisting)))
            .Returns(workflowHandle);

        await sut.StartWorkflowReSyncSlackWorkspaceAsync(args, cancellationToken);

        A.CallTo(() => temporalClient.StartWorkflowAsync(
            A<Expression<Func<ReSyncSlackWorkspace, Task>>>._,
            A<WorkflowOptions>.That.Matches(options =>
                options.Id == expectedId &&
                options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicate &&
                options.IdConflictPolicy == WorkflowIdConflictPolicy.TerminateExisting))).MustHaveHappenedOnceExactly();
    }
}
