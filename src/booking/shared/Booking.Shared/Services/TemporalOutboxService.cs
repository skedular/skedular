using System.Text.Json;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Temporal;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Microsoft.Extensions.Logging;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace Booking.Shared.Services;

/// <summary>
///     Service for managing Temporal workflow operations through an outbox pattern.
///     Provides methods to start workflows and send signals asynchronously.
/// </summary>
public interface ITemporalOutboxService : ITemporalOutboxExecutor, ITemporalSignalOutboxExecutor
{
    /// <summary>
    ///     Starts a workflow to pay for a booking via card.
    /// </summary>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void StartWorkflowPayBookingViaCard(PayBookingViaCardInput args, IUnitOfWork unitOfWork);

    void StartWorkflowGenerateInitialArrearsBookingInvoice(GenerateInitialArrearsBookingInvoiceInput args, IUnitOfWork unitOfWork);

    /// <summary>
    ///     Starts a workflow to pay for a booking via bank transfer.
    /// </summary>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void StartWorkflowPayBookingViaBankTransfer(PayBookingViaBankTransferInput args, IUnitOfWork unitOfWork);

    void StartWorkflowNotifyMarketplaceBookingFailure(NotifyMarketplaceBookingFailureInput args, IUnitOfWork unitOfWork);
    void StartWorkflowNotifyMarketplaceBookingModification(NotifyMarketplaceBookingModificationInput args, IUnitOfWork unitOfWork);
    void StartWorkflowResolvePartialMarketplaceBooking(ResolvePartialMarketplaceBookingInput args, IUnitOfWork unitOfWork);
    void StartWorkflowProcessMarketplaceRefund(ProcessMarketplaceRefundInput args, IUnitOfWork unitOfWork);
    void StartWorkflowMarketplaceBookingCleanup(MarketplaceBookingCleanupInput args, IUnitOfWork unitOfWork);

    /// <summary>
    ///     Starts a workflow to book private recurring resources.
    /// </summary>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void StartBookPrivateRecurringResources(BookPrivateRecurringResourcesInput args, IUnitOfWork unitOfWork);

    /// <summary>
    ///     Starts a workflow to book marketplace booking subscription resources.
    /// </summary>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void StartBookMarketplaceBookingSubscriptionResources(BookMarketplaceBookingSubscriptionResourcesInput args, IUnitOfWork unitOfWork);

    /// <summary>
    ///     Signals the PayBookingViaCard workflow to delete a booking.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to delete.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void SignalWorkflowPayBookingViaCardDeleteBooking(string bookingId, IUnitOfWork unitOfWork);

    /// <summary>
    ///     Signals the PayBookingViaCard workflow to set payment status.
    /// </summary>
    /// <param name="bookingId">The ID of the booking.</param>
    /// <param name="executionArgs">The payment status arguments.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void SignalWorkflowPayBookingViaCardSetPaymentStatus(string bookingId, SetPaymentStatusArgs executionArgs, IUnitOfWork unitOfWork);

    /// <summary>
    ///     Signals the recurring-booking card-payment workflow to set payment status.
    /// </summary>
    /// <param name="recurringBookingId">The ID of the recurring booking.</param>
    /// <param name="executionArgs">The payment status arguments.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void SignalWorkflowPayRecurringBookingViaCardSetPaymentStatus(
        string recurringBookingId,
        SetPaymentStatusArgs executionArgs,
        IUnitOfWork unitOfWork);

    /// <summary>
    ///     Signals the recurring-booking bank-transfer workflow to set payment status.
    /// </summary>
    /// <param name="recurringBookingId">The ID of the recurring booking.</param>
    /// <param name="executionArgs">The payment status arguments.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void SignalWorkflowPayRecurringBookingViaBankTransferSetPaymentStatus(
        string recurringBookingId,
        SetPaymentStatusArgs executionArgs,
        IUnitOfWork unitOfWork);

    /// <summary>
    ///     Signals the PayBookingViaBankTransfer workflow to set payment status.
    /// </summary>
    /// <param name="bookingId">The ID of the booking.</param>
    /// <param name="executionArgs">The payment status arguments.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void SignalWorkflowPayBookingViaBankTransferSetPaymentStatus(string bookingId, SetPaymentStatusArgs executionArgs, IUnitOfWork unitOfWork);

    /// <summary>
    ///     Signals the PayBookingViaBankTransfer workflow to delete a booking.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to delete.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void SignalWorkflowPayBookingViaBankTransferDeleteBooking(string bookingId, IUnitOfWork unitOfWork);

    /// <summary>
    ///     Signals the BookPrivateRecurringResources workflow that a recurring booking was updated.
    /// </summary>
    /// <param name="recurringBookingId">The ID of the recurring booking.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void SignalWorkflowBookPrivateRecurringResourcesUpdated(string recurringBookingId, IUnitOfWork unitOfWork);

    /// <summary>
    ///     Signals the BookPrivateRecurringResources workflow that a recurring booking was deleted.
    /// </summary>
    /// <param name="recurringBookingId">The ID of the recurring booking.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void SignalWorkflowBookPrivateRecurringResourcesDeleted(string recurringBookingId, IUnitOfWork unitOfWork);

    /// <summary>
    ///     Signals the BookMarketplaceBookingSubscriptionResources workflow that a marketplace booking subscription was deleted.
    /// </summary>
    /// <param name="marketplaceBookingSubscriptionId">The ID of the marketplace booking subscription.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void SignalWorkflowBookMarketplaceBookingSubscriptionResourcesDeleted(
        string marketplaceBookingSubscriptionId,
        TimeOnly from,
        TimeOnly until,
        IUnitOfWork unitOfWork);
}

/// <summary>
///     Implementation of the Temporal outbox service.
/// </summary>
public class TemporalOutboxService(
    ITemporalClient temporalClient,
    IWorkflowIdService workflowIdService,
    ITemporalHelperService temporalHelperService,
    TemporalConfiguration temporalConfiguration,
    ITemporalOutboxWorkflowExecutor temporalOutboxWorkflowExecutor,
    ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor,
    ILogger<TemporalOutboxService> logger) : ITemporalOutboxService
{
    private static readonly string s_payBookingViaCard = typeof(PayBookingViaCard).ToWorkflowType();
    private static readonly string s_payBookingViaBankTransfer = typeof(PayBookingViaBankTransfer).ToWorkflowType();
    private static readonly string s_bookPrivateRecurringResources = typeof(BookPrivateRecurringResources).ToWorkflowType();

    private static readonly string s_bookMarketplaceBookingSubscriptionResources =
        typeof(BookMarketplaceBookingSubscriptionResources).ToWorkflowType();

    private static readonly string s_generateInitialArrearsBookingInvoice = typeof(GenerateInitialArrearsBookingInvoice).ToWorkflowType();

    private static readonly string s_notifyMarketplaceBookingFailure = typeof(NotifyMarketplaceBookingFailure).ToWorkflowType();
    private static readonly string s_notifyMarketplaceBookingModification = typeof(NotifyMarketplaceBookingModification).ToWorkflowType();
    private static readonly string s_resolvePartialMarketplaceBooking = typeof(ResolvePartialMarketplaceBooking).ToWorkflowType();
    private static readonly string s_processMarketplaceRefund = typeof(ProcessMarketplaceRefund).ToWorkflowType();
    private static readonly string s_marketplaceBookingCleanup = typeof(MarketplaceBookingCleanup).ToWorkflowType();

    private static readonly string s_payBookingViaCardSetPaymentStatusAsync =
        typeof(PayBookingViaCard).GetMethod(nameof(PayBookingViaCard.SetPaymentStatusAsync))!.ToWorkflowSignalType();

    private static readonly string s_payRecurringBookingViaCardSetPaymentStatusAsync =
        typeof(PayRecurringBookingViaCard).GetMethod(nameof(PayRecurringBookingViaCard.SetPaymentStatusAsync))!.ToWorkflowSignalType();

    private static readonly string s_payRecurringBookingViaBankTransferSetPaymentStatusAsync =
        typeof(PayRecurringBookingViaBankTransfer).GetMethod(nameof(PayRecurringBookingViaBankTransfer.SetPaymentStatusAsync))!
            .ToWorkflowSignalType();

    private static readonly string s_payBookingViaCardDeleteBookingAsync =
        typeof(PayBookingViaCard).GetMethod(nameof(PayBookingViaCard.DeleteBookingAsync))!.ToWorkflowSignalType();

    private static readonly string s_payBookingViaBankTransferSetPaymentStatusAsync =
        typeof(PayBookingViaBankTransfer).GetMethod(nameof(PayBookingViaBankTransfer.SetPaymentStatusAsync))!.ToWorkflowSignalType();

    private static readonly string s_payBookingViaBankTransferDeleteBookingAsync =
        typeof(PayBookingViaBankTransfer).GetMethod(nameof(PayBookingViaBankTransfer.DeleteBookingAsync))!.ToWorkflowSignalType();

    private static readonly string s_bookPrivateRecurringResourcesRecurringBookingUpdatedAsync =
        typeof(BookPrivateRecurringResources).GetMethod(nameof(BookPrivateRecurringResources.RecurringBookingUpdatedAsync))!.ToWorkflowSignalType();

    private static readonly string s_bookPrivateRecurringResourcesRecurringBookingDeletedAsync =
        typeof(BookPrivateRecurringResources).GetMethod(nameof(BookPrivateRecurringResources.RecurringBookingDeletedAsync))!.ToWorkflowSignalType();

    private static readonly string s_bookMarketplaceBookingSubscriptionResourcesMarketplaceBookingSubscriptionDeletedAsync =
        typeof(BookMarketplaceBookingSubscriptionResources)
            .GetMethod(nameof(BookMarketplaceBookingSubscriptionResources.MarketplaceBookingSubscriptionDeletedAsync))!
            .ToWorkflowSignalType();

    public void StartWorkflowPayBookingViaCard(PayBookingViaCardInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<PayBookingViaCard, PayBookingViaCardInput>(
            args,
            new WorkflowOptions
            {
                Id = workflowIdService.PayBookingViaCard(args.BookingId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly,
            },
            unitOfWork);

    public void StartWorkflowGenerateInitialArrearsBookingInvoice(GenerateInitialArrearsBookingInvoiceInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<GenerateInitialArrearsBookingInvoice, GenerateInitialArrearsBookingInvoiceInput>(
            args,
            new WorkflowOptions
            {
                Id = workflowIdService.GenerateInitialArrearsBookingInvoice(args.BookingId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly,
            },
            unitOfWork);

    public void StartWorkflowPayBookingViaBankTransfer(PayBookingViaBankTransferInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<PayBookingViaBankTransfer, PayBookingViaBankTransferInput>(
            args,
            new WorkflowOptions
            {
                Id = workflowIdService.PayBookingViaBankTransfer(args.BookingId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly,
            },
            unitOfWork);

    public void StartWorkflowNotifyMarketplaceBookingFailure(NotifyMarketplaceBookingFailureInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<NotifyMarketplaceBookingFailure, NotifyMarketplaceBookingFailureInput>(
            args,
            new WorkflowOptions
            {
                Id = workflowIdService.NotifyMarketplaceBookingFailure(args.FailureId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly,
            },
            unitOfWork);

    public void StartWorkflowNotifyMarketplaceBookingModification(NotifyMarketplaceBookingModificationInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<NotifyMarketplaceBookingModification, NotifyMarketplaceBookingModificationInput>(
            args,
            new WorkflowOptions
            {
                Id = workflowIdService.NotifyMarketplaceBookingModification(args.ModificationId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly,
            },
            unitOfWork);

    public void StartWorkflowResolvePartialMarketplaceBooking(ResolvePartialMarketplaceBookingInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<ResolvePartialMarketplaceBooking, ResolvePartialMarketplaceBookingInput>(args,
            new WorkflowOptions
            {
                Id = workflowIdService.ResolvePartialMarketplaceBooking(args.FailureId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly,
            }, unitOfWork);

    public void StartWorkflowProcessMarketplaceRefund(ProcessMarketplaceRefundInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<ProcessMarketplaceRefund, ProcessMarketplaceRefundInput>(args,
            new WorkflowOptions
            {
                Id = workflowIdService.ProcessMarketplaceRefund(args.RefundId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
            }, unitOfWork);

    public void StartWorkflowMarketplaceBookingCleanup(MarketplaceBookingCleanupInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<MarketplaceBookingCleanup, MarketplaceBookingCleanupInput>(args,
            new WorkflowOptions
            {
                Id = workflowIdService.MarketplaceBookingCleanup(args.FailureId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
            }, unitOfWork);

    public void StartBookPrivateRecurringResources(BookPrivateRecurringResourcesInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<BookPrivateRecurringResources, BookPrivateRecurringResourcesInput>(
            args,
            new WorkflowOptions
            {
                Id = workflowIdService.BookPrivateRecurringResources(args.RecurringBookingId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting,
            },
            unitOfWork);

    public void StartBookMarketplaceBookingSubscriptionResources(
        BookMarketplaceBookingSubscriptionResourcesInput args,
        IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<BookMarketplaceBookingSubscriptionResources, BookMarketplaceBookingSubscriptionResourcesInput>(
            args,
            new WorkflowOptions
            {
                Id = workflowIdService.BookMarketplaceBookingSubscriptionResources(args.MarketplaceBookingSubscriptionId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting,
            },
            unitOfWork);

    public void SignalWorkflowPayBookingViaCardDeleteBooking(string bookingId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            workflowIdService.PayBookingViaCard(bookingId),
            s_payBookingViaCardDeleteBookingAsync,
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowPayBookingViaCardSetPaymentStatus(
        string bookingId,
        SetPaymentStatusArgs executionArgs,
        IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            workflowIdService.PayBookingViaCard(bookingId),
            s_payBookingViaCardSetPaymentStatusAsync,
            executionArgs,
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowPayRecurringBookingViaCardSetPaymentStatus(
        string recurringBookingId,
        SetPaymentStatusArgs executionArgs,
        IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            workflowIdService.PayRecurringBookingViaCard(recurringBookingId),
            s_payRecurringBookingViaCardSetPaymentStatusAsync,
            executionArgs,
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowPayRecurringBookingViaBankTransferSetPaymentStatus(
        string recurringBookingId,
        SetPaymentStatusArgs executionArgs,
        IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            workflowIdService.PayRecurringBookingViaBankTransfer(recurringBookingId),
            s_payRecurringBookingViaBankTransferSetPaymentStatusAsync,
            executionArgs,
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowPayBookingViaBankTransferSetPaymentStatus(
        string bookingId,
        SetPaymentStatusArgs executionArgs,
        IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            workflowIdService.PayBookingViaBankTransfer(bookingId),
            s_payBookingViaBankTransferSetPaymentStatusAsync,
            executionArgs,
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowPayBookingViaBankTransferDeleteBooking(string bookingId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            workflowIdService.PayBookingViaBankTransfer(bookingId),
            s_payBookingViaBankTransferDeleteBookingAsync,
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowBookPrivateRecurringResourcesUpdated(string recurringBookingId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            workflowIdService.BookPrivateRecurringResources(recurringBookingId),
            s_bookPrivateRecurringResourcesRecurringBookingUpdatedAsync,
            new PrivateRecurringBookingUpdatedArgs(recurringBookingId),
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowBookPrivateRecurringResourcesDeleted(string recurringBookingId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            workflowIdService.BookPrivateRecurringResources(recurringBookingId),
            s_bookPrivateRecurringResourcesRecurringBookingDeletedAsync,
            new PrivateRecurringBookingDeletedArgs(recurringBookingId),
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowBookMarketplaceBookingSubscriptionResourcesDeleted(
        string marketplaceBookingSubscriptionId,
        TimeOnly from,
        TimeOnly until,
        IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            workflowIdService.BookMarketplaceBookingSubscriptionResources(marketplaceBookingSubscriptionId),
            s_bookMarketplaceBookingSubscriptionResourcesMarketplaceBookingSubscriptionDeletedAsync,
            new MarketplaceBookingSubscriptionDeletedArgs(marketplaceBookingSubscriptionId, from, until),
            new WorkflowSignalOptions(),
            unitOfWork);

    public async Task StartWorkflowAsync(
        string workflowType,
        string? executionArgs,
        WorkflowOptions workflowOptions,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Dispatching Temporal outbox workflow {WorkflowType} with workflow ID {WorkflowId} to task queue {TaskQueue}",
            workflowType,
            workflowOptions.Id,
            workflowOptions.TaskQueue);

        await temporalClient.Connection.ConnectAsync();

        if (workflowType == s_payBookingViaCard)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<PayBookingViaCardInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((PayBookingViaCard workflow) => workflow.ExecuteAsync(input), workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_payBookingViaBankTransfer)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<PayBookingViaBankTransferInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((PayBookingViaBankTransfer workflow) => workflow.ExecuteAsync(input), workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_bookPrivateRecurringResources)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<BookPrivateRecurringResourcesInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (BookPrivateRecurringResources workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_generateInitialArrearsBookingInvoice)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<GenerateInitialArrearsBookingInvoiceInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (GenerateInitialArrearsBookingInvoice workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_notifyMarketplaceBookingFailure)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<NotifyMarketplaceBookingFailureInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (NotifyMarketplaceBookingFailure workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_notifyMarketplaceBookingModification)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<NotifyMarketplaceBookingModificationInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (NotifyMarketplaceBookingModification workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_processMarketplaceRefund)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<ProcessMarketplaceRefundInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((ProcessMarketplaceRefund workflow) => workflow.ExecuteAsync(input), workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_resolvePartialMarketplaceBooking)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<ResolvePartialMarketplaceBookingInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (ResolvePartialMarketplaceBooking workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_marketplaceBookingCleanup)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<MarketplaceBookingCleanupInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);
                _ = await temporalClient.StartWorkflowAsync(
                    (MarketplaceBookingCleanup workflow) => workflow.ExecuteAsync(input), workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_bookMarketplaceBookingSubscriptionResources)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<BookMarketplaceBookingSubscriptionResourcesInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (BookMarketplaceBookingSubscriptionResources workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else
        {
            throw new InvalidOperationException($"Unsupported Temporal outbox workflow type '{workflowType}'.");
        }
    }

    public async Task SignalAsync(
        string workflowId,
        string signalType,
        string? executionArgs,
        WorkflowSignalOptions workflowSignalOptions,
        CancellationToken cancellationToken)
    {
        await temporalClient.Connection.ConnectAsync();

        if (signalType == s_payBookingViaCardSetPaymentStatusAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<PayBookingViaCard>(workflowId, cancellationToken))
            {
                return;
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
            var input = JsonSerializer.Deserialize<SetPaymentStatusArgs>(executionArgs);
            ArgumentNullException.ThrowIfNull(input);

            await temporalClient
                .GetWorkflowHandle<PayBookingViaCard>(workflowId)
                .SignalAsync(workflow => workflow.SetPaymentStatusAsync(input), workflowSignalOptions);
        }
        else if (signalType == s_payRecurringBookingViaCardSetPaymentStatusAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<PayRecurringBookingViaCard>(workflowId, cancellationToken))
            {
                return;
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
            var input = JsonSerializer.Deserialize<SetPaymentStatusArgs>(executionArgs);
            ArgumentNullException.ThrowIfNull(input);

            await temporalClient
                .GetWorkflowHandle<PayRecurringBookingViaCard>(workflowId)
                .SignalAsync(workflow => workflow.SetPaymentStatusAsync(input), workflowSignalOptions);
        }
        else if (signalType == s_payRecurringBookingViaBankTransferSetPaymentStatusAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<PayRecurringBookingViaBankTransfer>(workflowId, cancellationToken))
            {
                return;
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
            var input = JsonSerializer.Deserialize<SetPaymentStatusArgs>(executionArgs);
            ArgumentNullException.ThrowIfNull(input);

            await temporalClient
                .GetWorkflowHandle<PayRecurringBookingViaBankTransfer>(workflowId)
                .SignalAsync(workflow => workflow.SetPaymentStatusAsync(input), workflowSignalOptions);
        }
        else if (signalType == s_payBookingViaCardDeleteBookingAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<PayBookingViaCard>(workflowId, cancellationToken))
            {
                return;
            }

            await temporalClient
                .GetWorkflowHandle<PayBookingViaCard>(workflowId)
                .SignalAsync(workflow => workflow.DeleteBookingAsync(), workflowSignalOptions);
        }
        else if (signalType == s_payBookingViaBankTransferSetPaymentStatusAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<PayBookingViaBankTransfer>(workflowId, cancellationToken))
            {
                return;
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
            var input = JsonSerializer.Deserialize<SetPaymentStatusArgs>(executionArgs);
            ArgumentNullException.ThrowIfNull(input);

            await temporalClient
                .GetWorkflowHandle<PayBookingViaBankTransfer>(workflowId)
                .SignalAsync(workflow => workflow.SetPaymentStatusAsync(input), workflowSignalOptions);
        }
        else if (signalType == s_payBookingViaBankTransferDeleteBookingAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<PayBookingViaBankTransfer>(workflowId, cancellationToken))
            {
                return;
            }

            await temporalClient
                .GetWorkflowHandle<PayBookingViaBankTransfer>(workflowId)
                .SignalAsync(workflow => workflow.DeleteBookingAsync(), workflowSignalOptions);
        }
        else if (signalType == s_bookPrivateRecurringResourcesRecurringBookingUpdatedAsync)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
            var input = JsonSerializer.Deserialize<PrivateRecurringBookingUpdatedArgs>(executionArgs);
            ArgumentNullException.ThrowIfNull(input);

            if (await temporalHelperService.IsRunningAsync<BookPrivateRecurringResources>(workflowId, cancellationToken))
            {
                await temporalClient
                    .GetWorkflowHandle<BookPrivateRecurringResources>(workflowId)
                    .SignalAsync(workflow => workflow.RecurringBookingUpdatedAsync(input), workflowSignalOptions);
            }
            else
            {
                var workflowHandle = await temporalClient.StartWorkflowAsync(
                    (BookPrivateRecurringResources workflow) =>
                        workflow.ExecuteAsync(new BookPrivateRecurringResourcesInput(input.RecurringBookingId)),
                    new WorkflowOptions
                    {
                        Id = workflowIdService.BookPrivateRecurringResources(input.RecurringBookingId),
                        TaskQueue = temporalConfiguration.Worker.TaskQueue,
                        RetryPolicy = null,
                        IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                        IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting,
                    });

                await workflowHandle.SignalAsync(
                    workflow => workflow.RecurringBookingUpdatedAsync(input),
                    workflowSignalOptions);
            }
        }
        else if (signalType == s_bookPrivateRecurringResourcesRecurringBookingDeletedAsync)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
            var input = JsonSerializer.Deserialize<PrivateRecurringBookingDeletedArgs>(executionArgs);
            ArgumentNullException.ThrowIfNull(input);

            if (await temporalHelperService.IsRunningAsync<BookPrivateRecurringResources>(workflowId, cancellationToken))
            {
                await temporalClient
                    .GetWorkflowHandle<BookPrivateRecurringResources>(workflowId)
                    .SignalAsync(workflow => workflow.RecurringBookingDeletedAsync(input), workflowSignalOptions);
            }
            else
            {
                var workflowHandle = await temporalClient.StartWorkflowAsync(
                    (BookPrivateRecurringResources workflow) =>
                        workflow.ExecuteAsync(new BookPrivateRecurringResourcesInput(input.RecurringBookingId)),
                    new WorkflowOptions
                    {
                        Id = workflowIdService.BookPrivateRecurringResources(input.RecurringBookingId),
                        TaskQueue = temporalConfiguration.Worker.TaskQueue,
                        RetryPolicy = null,
                        IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                        IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting,
                    });

                await workflowHandle.SignalAsync(
                    workflow => workflow.RecurringBookingDeletedAsync(input),
                    workflowSignalOptions);
            }
        }
        else if (signalType == s_bookMarketplaceBookingSubscriptionResourcesMarketplaceBookingSubscriptionDeletedAsync)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
            var input = JsonSerializer.Deserialize<MarketplaceBookingSubscriptionDeletedArgs>(executionArgs);
            ArgumentNullException.ThrowIfNull(input);

            if (await temporalHelperService.IsRunningAsync<BookMarketplaceBookingSubscriptionResources>(workflowId, cancellationToken))
            {
                await temporalClient
                    .GetWorkflowHandle<BookMarketplaceBookingSubscriptionResources>(workflowId)
                    .SignalAsync(
                        workflow => workflow.MarketplaceBookingSubscriptionDeletedAsync(input),
                        workflowSignalOptions);
            }
            else
            {
                var workflowHandle = await temporalClient.StartWorkflowAsync(
                    (BookMarketplaceBookingSubscriptionResources workflow) =>
                        workflow.ExecuteAsync(new BookMarketplaceBookingSubscriptionResourcesInput(
                            input.MarketplaceBookingSubscriptionId,
                            input.From,
                            input.Until)),
                    new WorkflowOptions
                    {
                        Id = workflowIdService.BookMarketplaceBookingSubscriptionResources(input.MarketplaceBookingSubscriptionId),
                        TaskQueue = temporalConfiguration.Worker.TaskQueue,
                        RetryPolicy = null,
                        IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                        IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting,
                    });

                await workflowHandle.SignalAsync(
                    workflow => workflow.MarketplaceBookingSubscriptionDeletedAsync(input),
                    workflowSignalOptions);
            }
        }
    }
}
