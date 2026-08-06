using System.Linq.Expressions;
using Enterprise.Shared.Temporal.Configurations;
using MsTeams.Shared.Services;
using MsTeams.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace MsTeams.Shared.UnitTests.Services.TemporalServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartWorkflowReSyncMsTeamsAsyncShould
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
        WorkflowHandle<ReSyncMsTeams> workflowHandle,
        TemporalService sut,
        ReSyncMsTeamsInput args,
        string expectedId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => workflowIdService.ReSyncMsTeams(args.TenantId)).Returns(expectedId);
        A.CallTo(() => temporalClient.StartWorkflowAsync(
                A<Expression<Func<ReSyncMsTeams, Task>>>._,
                A<WorkflowOptions>.That.Matches(options =>
                    options.Id == expectedId &&
                    options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                    options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicate &&
                    options.IdConflictPolicy == WorkflowIdConflictPolicy.TerminateExisting)))
            .Returns(workflowHandle);

        await sut.StartWorkflowReSyncMsTeamsAsync(args, cancellationToken);

        A.CallTo(() => temporalClient.StartWorkflowAsync(
            A<Expression<Func<ReSyncMsTeams, Task>>>._,
            A<WorkflowOptions>.That.Matches(options =>
                options.Id == expectedId &&
                options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicate &&
                options.IdConflictPolicy == WorkflowIdConflictPolicy.TerminateExisting))).MustHaveHappenedOnceExactly();
    }
}
