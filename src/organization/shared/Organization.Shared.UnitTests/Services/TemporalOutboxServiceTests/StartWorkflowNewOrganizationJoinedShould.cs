using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Organization.Shared.Services;
using Organization.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Organization.Shared.UnitTests.Services.TemporalOutboxServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartWorkflowNewOrganizationJoinedShould
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
        NewOrganizationJoinedInput args,
        IUnitOfWork unitOfWork,
        string expectedId)
    {
        A.CallTo(() => workflowIdService.NewOrganizationJoined(args.OrganizationId, args.OrganizationCustomDomain)).Returns(expectedId);

        sut.StartWorkflowNewOrganizationJoined(args, unitOfWork);

        A.CallTo(() => temporalOutboxWorkflowExecutor.Execute<NewOrganizationJoined, NewOrganizationJoinedInput>(
            args,
            A<WorkflowOptions>.That.Matches(options =>
                options.Id == expectedId &&
                options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicateFailedOnly),
            unitOfWork)).MustHaveHappenedOnceExactly();
    }
}
