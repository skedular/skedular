using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Organization.Shared.Services;
using Organization.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Organization.Shared.UnitTests.Services.TemporalOutboxServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartWorkflowInviteToJoinShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Enqueue_Workflow_With_Correct_Options(
        [Frozen] TemporalConfiguration temporalConfiguration,
        [Frozen] IWorkflowIdService workflowIdService,
        [Frozen] ITemporalOutboxWorkflowExecutor temporalOutboxWorkflowExecutor,
        TemporalOutboxService sut,
        InviteToJoinOrganizationInput args,
        IUnitOfWork unitOfWork,
        string expectedId)
    {
        A.CallTo(() => workflowIdService.InviteToJoin(args.JoinInvitationId)).Returns(expectedId);

        sut.StartWorkflowInviteToJoin(args, unitOfWork);

        A.CallTo(() => temporalOutboxWorkflowExecutor.Execute<InviteToJoinOrganization, InviteToJoinOrganizationInput>(
            args,
            A<WorkflowOptions>.That.Matches(options =>
                options.Id == expectedId &&
                options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicate &&
                options.IdConflictPolicy == WorkflowIdConflictPolicy.TerminateExisting),
            unitOfWork)).MustHaveHappenedOnceExactly();
    }
}
