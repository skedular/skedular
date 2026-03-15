using Booking.Shared.Workflows;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Booking.Shared.Services;

public interface ITemporalService
{
    Task StartWorkflowGenerateLocationResourcesSlotsAsync(GenerateLocationResourcesSlotsInput args, CancellationToken cancellationToken);
    Task StartWorkflowGenerateResourcesSlotsAsync(string locationId, GenerateResourcesSlotsInput args, CancellationToken cancellationToken);

    Task StartWorkflowBookMarketplaceBookingSubscriptionResourcesAsync(
        BookMarketplaceBookingSubscriptionResourcesInput args,
        CancellationToken cancellationToken);

    Task SignalPayBookingViaCardWorkflowAsync(string bookingId, SetPaymentStatusArgs args, CancellationToken cancellationToken);
}

public class TemporalService(
    TemporalConfiguration temporalConfiguration,
    ITemporalClient temporalClient,
    ITemporalHelperService temporalHelperService) : ITemporalService
{
    public async Task StartWorkflowGenerateLocationResourcesSlotsAsync(
        GenerateLocationResourcesSlotsInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((GenerateLocationResourcesSlots workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.GenerateLocationResourcesSlotsPrefix}-{args.LocationId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });

    public async Task StartWorkflowGenerateResourcesSlotsAsync(
        string locationId,
        GenerateResourcesSlotsInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((GenerateResourcesSlots workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.GenerateResourcesSlotsPrefix}-{locationId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });

    public async Task StartWorkflowBookMarketplaceBookingSubscriptionResourcesAsync(
        BookMarketplaceBookingSubscriptionResourcesInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync(
            (BookMarketplaceBookingSubscriptionResources workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId(args.MarketplaceBookingSubscriptionId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });

    public async Task SignalPayBookingViaCardWorkflowAsync(string bookingId, SetPaymentStatusArgs args, CancellationToken cancellationToken) =>
        await temporalClient
            .GetWorkflowHandle<PayBookingViaCard>(temporalHelperService.ToId($"{Constants.PaidViaCardPrefix}-{bookingId}"))
            .SignalAsync(
                workflow => workflow.SetPaymentStatusAsync(args),
                new WorkflowSignalOptions { Rpc = new RpcOptions { CancellationToken = cancellationToken } }
            );
}
