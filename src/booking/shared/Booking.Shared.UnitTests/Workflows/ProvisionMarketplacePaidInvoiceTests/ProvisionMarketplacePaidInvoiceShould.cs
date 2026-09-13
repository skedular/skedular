using Booking.Shared.Activities;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Temporalio.Client;
using Temporalio.Testing;
using Temporalio.Worker;

namespace Booking.Shared.UnitTests.Workflows.ProvisionMarketplacePaidInvoiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ProvisionMarketplacePaidInvoiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Complete_WhenProvisioningEventuallySucceeds(
        [Frozen]
        IMarketplacePaidInvoiceProvisioner provisioner,
        MarketplacePaidInvoiceIntegrations marketplacePaidInvoiceIntegrations,
        ProvisionMarketplacePaidInvoiceInput input,
        string taskQueue,
        string workflowId,
        CancellationToken cancellationToken)
    {
        input = input with
        {
            StripeAccountId = "acct",
            InvoiceId = "invoice",
        };
        A.CallTo(() => provisioner.ProvisionAsync(
                A<string?>._,
                A<string?>._,
                A<string>._!,
                A<string>._!,
                A<DateTimeOffset>._,
                A<string?>._,
                A<CancellationToken>._))
            .ReturnsNextFromSequence(false, true);

        await using var environment = await WorkflowEnvironment.StartTimeSkippingAsync();
        using var worker = new TemporalWorker(
            environment.Client,
            new TemporalWorkerOptions(taskQueue)
                .AddWorkflow<ProvisionMarketplacePaidInvoice>()
                .AddAllActivities(marketplacePaidInvoiceIntegrations));

        await worker.ExecuteAsync(async () =>
        {
            var handle = await environment.Client.StartWorkflowAsync(
                (ProvisionMarketplacePaidInvoice workflow) => workflow.ExecuteAsync(input),
                new WorkflowOptions(workflowId, worker.Options.TaskQueue!));
            await handle.GetResultAsync();
        }, cancellationToken);

        A.CallTo(() => provisioner.ProvisionAsync(
                A<string?>._,
                A<string?>._,
                A<string>._!,
                A<string>._!,
                A<DateTimeOffset>._,
                A<string?>._,
                A<CancellationToken>._))
            .MustHaveHappenedTwiceExactly();
    }
}
