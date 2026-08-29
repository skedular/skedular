using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Temporalio.Client;
using Temporalio.Testing;
using Temporalio.Worker;

namespace Booking.Shared.UnitTests.Workflows.MarketplaceBookingCleanupTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceBookingCleanupShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Complete_Idempotently_When_Failure_Is_Already_Missing(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingFailureRepository failureRepository,
        MarketplaceBookingCleanupIntegrations activity,
        MarketplaceBookingCleanupInput input,
        string taskQueue,
        string workflowId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureRepository).Returns(failureRepository);
        A.CallTo(() => failureRepository.GetByIdAsync(input.FailureId, A<CancellationToken>._))
            .Returns((MarketplaceBookingFailure?)null);

        await using var environment = await WorkflowEnvironment.StartTimeSkippingAsync();
        using var worker = new TemporalWorker(
            environment.Client,
            new TemporalWorkerOptions(taskQueue)
                .AddWorkflow<MarketplaceBookingCleanup>()
                .AddAllActivities(activity));

        await worker.ExecuteAsync(async () =>
        {
            var handle = await environment.Client.StartWorkflowAsync(
                (MarketplaceBookingCleanup workflow) => workflow.ExecuteAsync(input),
                new WorkflowOptions(workflowId, worker.Options.TaskQueue!));

            await handle.GetResultAsync();
        }, cancellationToken);

        A.CallTo(() => failureRepository.GetByIdAsync(input.FailureId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}
