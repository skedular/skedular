using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Temporal;
using Microsoft.Extensions.Logging;
using Team.Shared.Services;
using Temporalio.Client;

namespace Team.Shared.UnitTests.Services.TemporalOutboxServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SignalWorkflowInviteToJoinInvitationStatusChangedShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Enqueue_Signal_With_Correct_Workflow_Id(
        [Frozen] IWorkflowIdService workflowIdService,
        [Frozen] ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor,
        [Frozen] ILogger<TemporalOutboxService> logger,
        TemporalOutboxService sut,
        IUnitOfWork unitOfWork,
        string joinInvitationId,
        string expectedWorkflowId)
    {
        A.CallTo(() => workflowIdService.InviteToJoin(joinInvitationId)).Returns(expectedWorkflowId);

        sut.SignalWorkflowInviteToJoinInvitationStatusChanged(joinInvitationId, unitOfWork);

        A.CallTo(() => temporalSignalOutboxWorkflowExecutor.Signal(
            expectedWorkflowId,
            A<string>._,
            A<WorkflowSignalOptions>._,
            unitOfWork)).MustHaveHappenedOnceExactly();

        A.CallTo(logger)
            .Where(call => call.Method.Name == nameof(ILogger.Log))
            .MustHaveHappened();
    }
}
