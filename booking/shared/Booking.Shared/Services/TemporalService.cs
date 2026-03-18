using Booking.Shared.Workflows;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Booking.Shared.Services;

/// <summary>
///     Service for managing Temporal workflow operations.
/// </summary>
public interface ITemporalService
{
    /// <summary>
    ///     Starts a workflow to generate location resource slots.
    /// </summary>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task StartWorkflowGenerateLocationResourcesSlotsAsync(GenerateLocationResourcesSlotsInput args, CancellationToken cancellationToken);

    /// <summary>
    ///     Starts a workflow to generate resource slots for a specific location.
    /// </summary>
    /// <param name="locationId">The ID of the location.</param>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task StartWorkflowGenerateResourcesSlotsAsync(string locationId, GenerateResourcesSlotsInput args, CancellationToken cancellationToken);

    /// <summary>
    ///     Starts a workflow to book marketplace booking subscription resources.
    /// </summary>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task StartWorkflowBookMarketplaceBookingSubscriptionResourcesAsync(
        BookMarketplaceBookingSubscriptionResourcesInput args,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Starts a workflow to pay for a recurring booking cycle via card.
    /// </summary>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task StartWorkflowPayRecurringBookingViaCardAsync(PayRecurringBookingViaCardInput args, CancellationToken cancellationToken);

    /// <summary>
    ///     Signals a payment status update to the recurring-booking card-payment workflow.
    /// </summary>
    /// <param name="recurringBookingId">The ID of the recurring booking.</param>
    /// <param name="args">The payment status arguments.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task SignalPayRecurringBookingViaCardWorkflowAsync(string recurringBookingId, SetPaymentStatusArgs args, CancellationToken cancellationToken);

    /// <summary>
    ///     Signals a payment status update to the PayBookingViaCard workflow.
    /// </summary>
    /// <param name="bookingId">The ID of the booking.</param>
    /// <param name="args">The payment status arguments.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task SignalPayBookingViaCardWorkflowAsync(string bookingId, SetPaymentStatusArgs args, CancellationToken cancellationToken);
}

/// <summary>
///     Implementation of the Temporal service.
/// </summary>
public class TemporalService(
    TemporalConfiguration temporalConfiguration,
    ITemporalClient temporalClient,
    ITemporalHelperService temporalHelperService) : ITemporalService
{
    /// <summary>
    ///     Starts a workflow to generate location resource slots.
    /// </summary>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
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

    /// <summary>
    ///     Starts a workflow to generate resource slots for a specific location.
    /// </summary>
    /// <param name="locationId">The ID of the location.</param>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
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

    /// <summary>
    ///     Starts a workflow to book marketplace booking subscription resources.
    /// </summary>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
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

    /// <summary>
    ///     Starts a workflow to pay for a recurring booking cycle via card.
    /// </summary>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task StartWorkflowPayRecurringBookingViaCardAsync(PayRecurringBookingViaCardInput args, CancellationToken cancellationToken) => 
        await temporalClient.StartWorkflowAsync(
            (PayRecurringBookingViaCard workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.PaidRecurringBookingViaCardPrefix}-{args.RecurringBookingId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });

    /// <summary>
    ///     Signals a payment status update to the recurring-booking card-payment workflow.
    /// </summary>
    /// <param name="recurringBookingId">The ID of the recurring booking.</param>
    /// <param name="args">The payment status arguments.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task SignalPayRecurringBookingViaCardWorkflowAsync(
        string recurringBookingId,
        SetPaymentStatusArgs args,
        CancellationToken cancellationToken) =>
        await temporalClient
            .GetWorkflowHandle<PayRecurringBookingViaCard>(
                temporalHelperService.ToId($"{Constants.PaidRecurringBookingViaCardPrefix}-{recurringBookingId}"))
            .SignalAsync(
                workflow => workflow.SetPaymentStatusAsync(args),
                new WorkflowSignalOptions { Rpc = new RpcOptions { CancellationToken = cancellationToken } }
            );

    /// <summary>
    ///     Signals a payment status update to the PayBookingViaCard workflow.
    /// </summary>
    /// <param name="bookingId">The ID of the booking.</param>
    /// <param name="args">The payment status arguments.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task SignalPayBookingViaCardWorkflowAsync(string bookingId, SetPaymentStatusArgs args, CancellationToken cancellationToken) =>
        await temporalClient
            .GetWorkflowHandle<PayBookingViaCard>(temporalHelperService.ToId($"{Constants.PaidViaCardPrefix}-{bookingId}"))
            .SignalAsync(
                workflow => workflow.SetPaymentStatusAsync(args),
                new WorkflowSignalOptions { Rpc = new RpcOptions { CancellationToken = cancellationToken } }
            );
}
