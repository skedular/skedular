using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Booking.Shared.UnitTests.Services.TemporalOutboxServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartBookPrivateRecurringResourcesShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Enqueue_Workflow_With_Correct_Options(
        [Frozen] TemporalConfiguration temporalConfiguration,
        [Frozen] IWorkflowIdService workflowIdService,
        [Frozen] ITemporalOutboxWorkflowExecutor temporalOutboxWorkflowExecutor,
        TemporalOutboxService sut,
        BookPrivateRecurringResourcesInput args,
        IUnitOfWork unitOfWork,
        string expectedId)
    {
        A.CallTo(() => workflowIdService.BookPrivateRecurringResources(args.RecurringBookingId)).Returns(expectedId);

        sut.StartBookPrivateRecurringResources(args, unitOfWork);

        A.CallTo(() => temporalOutboxWorkflowExecutor.Execute<BookPrivateRecurringResources, BookPrivateRecurringResourcesInput>(
            args,
            A<WorkflowOptions>.That.Matches(options =>
                options.Id == expectedId &&
                options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicate &&
                options.IdConflictPolicy == WorkflowIdConflictPolicy.TerminateExisting),
            unitOfWork)).MustHaveHappenedOnceExactly();
    }
}
