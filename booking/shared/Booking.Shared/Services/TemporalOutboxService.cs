using System.Text.Json;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
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

    /// <summary>
    ///     Starts a workflow to pay for a booking via bank transfer.
    /// </summary>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void StartWorkflowPayBookingViaBankTransfer(PayBookingViaBankTransferInput args, IUnitOfWork unitOfWork);

    /// <summary>
    ///     Starts a workflow to book private recurring resources.
    /// </summary>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void StartBookPrivateRecurringResources(BookPrivateRecurringResourcesInput args, IUnitOfWork unitOfWork);

    /// <summary>
    ///     Starts a workflow to book marketplace recurring resources.
    /// </summary>
    /// <param name="args">The input arguments for the workflow.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void StartBookMarketplaceRecurringResources(BookMarketplaceRecurringResourcesInput args, IUnitOfWork unitOfWork);

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
    ///     Signals the BookMarketplaceRecurringResources workflow that a recurring booking was deleted.
    /// </summary>
    /// <param name="recurringBookingId">The ID of the recurring booking.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void SignalWorkflowBookMarketplaceRecurringResourcesDeleted(string recurringBookingId, IUnitOfWork unitOfWork);

    /// <summary>
    ///     Signals the BookMarketplaceBookingSubscriptionResources workflow that a marketplace booking subscription was deleted.
    /// </summary>
    /// <param name="marketplaceBookingSubscriptionId">The ID of the marketplace booking subscription.</param>
    /// <param name="unitOfWork">The unit of work for the operation.</param>
    void SignalWorkflowBookMarketplaceBookingSubscriptionResourcesDeleted(string marketplaceBookingSubscriptionId, IUnitOfWork unitOfWork);
}

/// <summary>
///     Implementation of the Temporal outbox service.
/// </summary>
public class TemporalOutboxService(
    ITemporalClient temporalClient,
    ITemporalHelperService temporalHelperService,
    TemporalConfiguration temporalConfiguration,
    ITemporalOutboxWorkflowExecutor temporalOutboxWorkflowExecutor,
    ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor) : ITemporalOutboxService
{
    private static readonly string s_payBookingViaCard = typeof(PayBookingViaCard).ToWorkflowType();
    private static readonly string s_payBookingViaBankTransfer = typeof(PayBookingViaBankTransfer).ToWorkflowType();
    private static readonly string s_bookPrivateRecurringResources = typeof(BookPrivateRecurringResources).ToWorkflowType();

    private static readonly string s_bookMarketplaceRecurringResources = typeof(BookMarketplaceRecurringResources).ToWorkflowType();

    private static readonly string s_bookMarketplaceBookingSubscriptionResources =
        typeof(BookMarketplaceBookingSubscriptionResources).ToWorkflowType();

    private static readonly string s_payBookingViaCardSetPaymentStatusAsync =
        typeof(PayBookingViaCard).GetMethod(nameof(PayBookingViaCard.SetPaymentStatusAsync))!.ToWorkflowSignalType();

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

    private static readonly string s_bookMarketplaceRecurringResourcesRecurringBookingDeletedAsync =
        typeof(BookMarketplaceRecurringResources).GetMethod(nameof(BookMarketplaceRecurringResources.RecurringBookingDeletedAsync))!
            .ToWorkflowSignalType();

    private static readonly string s_bookMarketplaceBookingSubscriptionResourcesMarketplaceBookingSubscriptionDeletedAsync =
        typeof(BookMarketplaceBookingSubscriptionResources)
            .GetMethod(nameof(BookMarketplaceBookingSubscriptionResources.MarketplaceBookingSubscriptionDeletedAsync))!
            .ToWorkflowSignalType();

    public void StartWorkflowPayBookingViaCard(PayBookingViaCardInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<PayBookingViaCard, PayBookingViaCardInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.PaidViaCardPrefix}-{args.BookingId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);

    public void StartWorkflowPayBookingViaBankTransfer(PayBookingViaBankTransferInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<PayBookingViaBankTransfer, PayBookingViaBankTransferInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.PaidViaBankTransferPrefix}-{args.BookingId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);

    public void StartBookPrivateRecurringResources(BookPrivateRecurringResourcesInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<BookPrivateRecurringResources, BookPrivateRecurringResourcesInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId(args.RecurringBookingId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting
            },
            unitOfWork);

    public void StartBookMarketplaceRecurringResources(BookMarketplaceRecurringResourcesInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<BookMarketplaceRecurringResources, BookMarketplaceRecurringResourcesInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId(args.RecurringBookingId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting
            },
            unitOfWork);

    public void StartBookMarketplaceBookingSubscriptionResources(
        BookMarketplaceBookingSubscriptionResourcesInput args,
        IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<BookMarketplaceBookingSubscriptionResources, BookMarketplaceBookingSubscriptionResourcesInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId(args.MarketplaceBookingSubscriptionId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting
            },
            unitOfWork);

    public void SignalWorkflowPayBookingViaCardDeleteBooking(string bookingId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            temporalHelperService.ToId($"{Constants.PaidViaCardPrefix}-{bookingId}"),
            s_payBookingViaCardDeleteBookingAsync,
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowPayBookingViaCardSetPaymentStatus(
        string bookingId,
        SetPaymentStatusArgs executionArgs,
        IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            temporalHelperService.ToId($"{Constants.PaidViaCardPrefix}-{bookingId}"),
            s_payBookingViaCardSetPaymentStatusAsync,
            executionArgs,
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowPayBookingViaBankTransferSetPaymentStatus(
        string bookingId,
        SetPaymentStatusArgs executionArgs,
        IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            temporalHelperService.ToId($"{Constants.PaidViaBankTransferPrefix}-{bookingId}"),
            s_payBookingViaBankTransferSetPaymentStatusAsync,
            executionArgs,
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowPayBookingViaBankTransferDeleteBooking(string bookingId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            temporalHelperService.ToId($"{Constants.PaidViaBankTransferPrefix}-{bookingId}"),
            s_payBookingViaBankTransferDeleteBookingAsync,
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowBookPrivateRecurringResourcesUpdated(string recurringBookingId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            temporalHelperService.ToId(recurringBookingId),
            s_bookPrivateRecurringResourcesRecurringBookingUpdatedAsync,
            new PrivateRecurringBookingUpdatedArgs(recurringBookingId),
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowBookPrivateRecurringResourcesDeleted(string recurringBookingId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            temporalHelperService.ToId(recurringBookingId),
            s_bookPrivateRecurringResourcesRecurringBookingDeletedAsync,
            new PrivateRecurringBookingDeletedArgs(recurringBookingId),
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowBookMarketplaceRecurringResourcesDeleted(string recurringBookingId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            temporalHelperService.ToId(recurringBookingId),
            s_bookMarketplaceRecurringResourcesRecurringBookingDeletedAsync,
            new MarketplaceRecurringBookingDeletedArgs(recurringBookingId),
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowBookMarketplaceBookingSubscriptionResourcesDeleted(
        string marketplaceBookingSubscriptionId,
        IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            temporalHelperService.ToId(marketplaceBookingSubscriptionId),
            s_bookMarketplaceBookingSubscriptionResourcesMarketplaceBookingSubscriptionDeletedAsync,
            new MarketplaceBookingSubscriptionDeletedArgs(marketplaceBookingSubscriptionId),
            new WorkflowSignalOptions(),
            unitOfWork);

    public async Task StartWorkflowAsync(
        string workflowType,
        string? executionArgs,
        WorkflowOptions workflowOptions,
        CancellationToken cancellationToken)
    {
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
        else if (workflowType == s_bookMarketplaceRecurringResources)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<BookMarketplaceRecurringResourcesInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (BookMarketplaceRecurringResources workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
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
                _ = await temporalClient.StartWorkflowAsync(
                    (BookPrivateRecurringResources workflow) =>
                        workflow.ExecuteAsync(new BookPrivateRecurringResourcesInput(input.RecurringBookingId)),
                    new WorkflowOptions
                    {
                        Id = temporalHelperService.ToId(input.RecurringBookingId),
                        TaskQueue = temporalConfiguration.Worker.TaskQueue,
                        RetryPolicy = null,
                        IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                        IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting
                    });
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
                _ = await temporalClient.StartWorkflowAsync(
                    (BookPrivateRecurringResources workflow) =>
                        workflow.ExecuteAsync(new BookPrivateRecurringResourcesInput(input.RecurringBookingId)),
                    new WorkflowOptions
                    {
                        Id = temporalHelperService.ToId(input.RecurringBookingId),
                        TaskQueue = temporalConfiguration.Worker.TaskQueue,
                        RetryPolicy = null,
                        IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                        IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting
                    });
            }
        }
        else if (signalType == s_bookMarketplaceRecurringResourcesRecurringBookingDeletedAsync)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
            var input = JsonSerializer.Deserialize<MarketplaceRecurringBookingDeletedArgs>(executionArgs);
            ArgumentNullException.ThrowIfNull(input);

            if (await temporalHelperService.IsRunningAsync<BookMarketplaceRecurringResources>(workflowId, cancellationToken))
            {
                await temporalClient
                    .GetWorkflowHandle<BookMarketplaceRecurringResources>(workflowId)
                    .SignalAsync(workflow => workflow.RecurringBookingDeletedAsync(input), workflowSignalOptions);
            }
            else
            {
                _ = await temporalClient.StartWorkflowAsync(
                    (BookMarketplaceRecurringResources workflow) =>
                        workflow.ExecuteAsync(new BookMarketplaceRecurringResourcesInput(input.RecurringBookingId)),
                    new WorkflowOptions
                    {
                        Id = temporalHelperService.ToId(input.RecurringBookingId),
                        TaskQueue = temporalConfiguration.Worker.TaskQueue,
                        RetryPolicy = null,
                        IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                        IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting
                    });
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
                _ = await temporalClient.StartWorkflowAsync(
                    (BookMarketplaceBookingSubscriptionResources workflow) =>
                        workflow.ExecuteAsync(new BookMarketplaceBookingSubscriptionResourcesInput(input.MarketplaceBookingSubscriptionId)),
                    new WorkflowOptions
                    {
                        Id = temporalHelperService.ToId(input.MarketplaceBookingSubscriptionId),
                        TaskQueue = temporalConfiguration.Worker.TaskQueue,
                        RetryPolicy = null,
                        IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                        IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting
                    });
            }
        }
    }
}
