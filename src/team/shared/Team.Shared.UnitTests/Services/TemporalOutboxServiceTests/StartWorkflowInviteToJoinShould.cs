using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Microsoft.Extensions.Logging;
using Team.Shared.Services;
using Team.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Team.Shared.UnitTests.Services.TemporalOutboxServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartWorkflowInviteToJoinShould
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
        [Frozen]
        ILogger<TemporalOutboxService> logger,
        TemporalOutboxService sut,
        InviteToJoinTeamInput args,
        IUnitOfWork unitOfWork,
        string expectedId)
    {
        A.CallTo(() => workflowIdService.InviteToJoin(args.JoinInvitationId)).Returns(expectedId);

        sut.StartWorkflowInviteToJoin(args, unitOfWork);

        A.CallTo(() => temporalOutboxWorkflowExecutor.Execute<InviteToJoinTeam, InviteToJoinTeamInput>(
            args,
            A<WorkflowOptions>.That.Matches(options =>
                options.Id == expectedId &&
                options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicate &&
                options.IdConflictPolicy == WorkflowIdConflictPolicy.TerminateExisting),
            unitOfWork)).MustHaveHappenedOnceExactly();

        A.CallTo(logger)
            .Where(call => call.Method.Name == nameof(ILogger.Log))
            .MustHaveHappened();
    }
}
