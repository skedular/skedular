using Booking.Shared.Models;
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
    ///     Starts a workflow to pay for a recurring booking cycle via card.
    /// </summary>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task StartWorkflowPayRecurringBookingViaCardAsync(PayRecurringBookingViaCardInput args, CancellationToken cancellationToken);

    /// <summary>
    ///     Starts a workflow to pay for a recurring booking cycle via bank transfer.
    /// </summary>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task StartWorkflowPayRecurringBookingViaBankTransferAsync(PayRecurringBookingViaBankTransferInput args, CancellationToken cancellationToken);

    Task StartWorkflowGenerateInitialArrearsRecurringBookingInvoiceAsync(
        GenerateInitialArrearsRecurringBookingInvoiceInput args,
        CancellationToken cancellationToken);

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

    /// <summary>
    ///     Starts a workflow to run organization-level in-arrears billing.
    /// </summary>
    Task StartWorkflowRunOrganizationArrearsBillingAsync(RunOrganizationArrearsBillingInput args, CancellationToken cancellationToken);

    /// <summary>
    ///     Updates the organization in-arrears billing workflow configuration.
    /// </summary>
    Task SignalRunOrganizationArrearsBillingWorkflowUpdateConfigurationAsync(
        string organizationId,
        OrganizationArrearsBillingConfiguration configuration,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Requests the organization in-arrears billing workflow to generate invoices immediately.
    /// </summary>
    Task SignalRunOrganizationArrearsBillingWorkflowRunNowAsync(
        OrganizationArrearsBillingConfiguration configuration,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Requests the organization in-arrears billing workflow to stop.
    /// </summary>
    Task SignalRunOrganizationArrearsBillingWorkflowStopAsync(string organizationId, CancellationToken cancellationToken);
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
    ///     Starts a workflow to pay for a recurring booking cycle via bank transfer.
    /// </summary>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task StartWorkflowPayRecurringBookingViaBankTransferAsync(
        PayRecurringBookingViaBankTransferInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync(
            (PayRecurringBookingViaBankTransfer workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.PaidRecurringBookingViaBankTransferPrefix}-{args.RecurringBookingId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });

    public async Task StartWorkflowGenerateInitialArrearsRecurringBookingInvoiceAsync(
        GenerateInitialArrearsRecurringBookingInvoiceInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync(
            (GenerateInitialArrearsRecurringBookingInvoice workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.InitialArrearsRecurringBookingInvoicePrefix}-{args.RecurringBookingId}"),
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

    public async Task StartWorkflowRunOrganizationArrearsBillingAsync(RunOrganizationArrearsBillingInput args, CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync(
            (RunOrganizationArrearsBilling workflow) => workflow.ExecuteAsync(args),
            ToOrganizationArrearsBillingWorkflowOptions(args.Configuration.OrganizationId, cancellationToken));

    public async Task SignalRunOrganizationArrearsBillingWorkflowUpdateConfigurationAsync(
        string organizationId,
        OrganizationArrearsBillingConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var workflowId = ToOrganizationArrearsBillingWorkflowId(organizationId);

        if (await temporalHelperService.IsRunningAsync<RunOrganizationArrearsBilling>(workflowId, cancellationToken))
        {
            await temporalClient
                .GetWorkflowHandle<RunOrganizationArrearsBilling>(workflowId)
                .SignalAsync(
                    workflow => workflow.UpdateConfigurationAsync(configuration),
                    new WorkflowSignalOptions { Rpc = new RpcOptions { CancellationToken = cancellationToken } });
            return;
        }

        await StartWorkflowRunOrganizationArrearsBillingAsync(new RunOrganizationArrearsBillingInput(configuration), cancellationToken);
    }

    public async Task SignalRunOrganizationArrearsBillingWorkflowRunNowAsync(
        OrganizationArrearsBillingConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var workflowId = ToOrganizationArrearsBillingWorkflowId(configuration.OrganizationId);

        if (await temporalHelperService.IsRunningAsync<RunOrganizationArrearsBilling>(workflowId, cancellationToken))
        {
            await temporalClient
                .GetWorkflowHandle<RunOrganizationArrearsBilling>(workflowId)
                .SignalAsync(
                    workflow => workflow.RunNowAsync(),
                    new WorkflowSignalOptions { Rpc = new RpcOptions { CancellationToken = cancellationToken } });
            return;
        }

        await StartWorkflowRunOrganizationArrearsBillingAsync(
            new RunOrganizationArrearsBillingInput(configuration, true),
            cancellationToken);
    }

    public async Task SignalRunOrganizationArrearsBillingWorkflowStopAsync(string organizationId, CancellationToken cancellationToken)
    {
        var workflowId = ToOrganizationArrearsBillingWorkflowId(organizationId);
        if (!await temporalHelperService.IsRunningAsync<RunOrganizationArrearsBilling>(workflowId, cancellationToken))
        {
            return;
        }

        await temporalClient
            .GetWorkflowHandle<RunOrganizationArrearsBilling>(workflowId)
            .SignalAsync(
                workflow => workflow.StopAsync(),
                new WorkflowSignalOptions { Rpc = new RpcOptions { CancellationToken = cancellationToken } });
    }

    private string ToOrganizationArrearsBillingWorkflowId(string organizationId) =>
        temporalHelperService.ToId($"{Constants.OrganizationArrearsBillingPrefix}-{organizationId}");

    private WorkflowOptions ToOrganizationArrearsBillingWorkflowOptions(
        string organizationId,
        CancellationToken cancellationToken) =>
        new()
        {
            Id = ToOrganizationArrearsBillingWorkflowId(organizationId),
            TaskQueue = temporalConfiguration.Worker.TaskQueue,
            RetryPolicy = null,
            IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
            IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting,
            Rpc = new RpcOptions { CancellationToken = cancellationToken }
        };
}
