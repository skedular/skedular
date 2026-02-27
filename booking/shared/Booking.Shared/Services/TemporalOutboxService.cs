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

public interface ITemporalOutboxService : ITemporalOutboxExecutor, ITemporalSignalOutboxExecutor
{
    void StartWorkflowPayBookingViaCard(PayBookingViaCardInput args, IUnitOfWork unitOfWork);
    void StartWorkflowPayBookingViaBankTransfer(PayBookingViaBankTransferInput args, IUnitOfWork unitOfWork);
    void StartBookPrivateRecurringResources(BookPrivateRecurringResourcesInput args, IUnitOfWork unitOfWork);
    void StartBookMarketplaceRecurringResources(BookMarketplaceRecurringResourcesInput args, IUnitOfWork unitOfWork);

    void SignalWorkflowPayBookingViaCardDeleteBooking(string bookingId, IUnitOfWork unitOfWork);
    void SignalWorkflowPayBookingViaCardSetPaymentStatus(string bookingId, SetPaymentStatusArgs executionArgs, IUnitOfWork unitOfWork);
    void SignalWorkflowPayBookingViaBankTransferSetPaymentStatus(string bookingId, SetPaymentStatusArgs executionArgs, IUnitOfWork unitOfWork);
    void SignalWorkflowPayBookingViaBankTransferDeleteBooking(string bookingId, IUnitOfWork unitOfWork);
    void SignalWorkflowBookPrivateRecurringResourcesUpdated(string recurringBookingId, IUnitOfWork unitOfWork);
    void SignalWorkflowBookPrivateRecurringResourcesDeleted(string recurringBookingId, IUnitOfWork unitOfWork);
    void SignalWorkflowBookMarketplaceRecurringResourcesDeleted(string recurringBookingId, IUnitOfWork unitOfWork);
}

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
    }
}
