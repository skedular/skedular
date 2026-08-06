using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Location.Shared.Services;
using Location.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Location.Shared.UnitTests.Services.TemporalOutboxServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartComputeOrganizationLocationsAndProductsRelationshipsShould
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
        ComputeOrganizationLocationsAndProductsRelationshipsInput args,
        IUnitOfWork unitOfWork,
        string expectedId)
    {
        A.CallTo(() => workflowIdService.ComputeOrganizationLocationsAndProductsRelationships(args.OrganizationId)).Returns(expectedId);

        sut.StartComputeOrganizationLocationsAndProductsRelationships(args, unitOfWork);

        A.CallTo(() => temporalOutboxWorkflowExecutor
            .Execute<ComputeOrganizationLocationsAndProductsRelationships, ComputeOrganizationLocationsAndProductsRelationshipsInput>(
                args,
                A<WorkflowOptions>.That.Matches(options =>
                    options.Id == expectedId &&
                    options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                    options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicate &&
                    options.IdConflictPolicy == WorkflowIdConflictPolicy.TerminateExisting),
                unitOfWork)).MustHaveHappenedOnceExactly();
    }
}
