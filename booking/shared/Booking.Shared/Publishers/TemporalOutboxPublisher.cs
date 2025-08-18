using Booking.Shared.Workflows;
using Booking.Shared.Workflows.Payment;
using Booking.Shared.Workflows.Payment.PayViaBankTransfer;
using Booking.Shared.Workflows.Payment.PayViaCard;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Booking.Shared.Publishers;

public interface ITemporalOutboxPublisher
{
    void StartWorkflowPayBookingViaCard(PayBookingViaCardInput args, IUnitOfWork unitOfWork);
    void StartWorkflowPayBookingViaBankTransfer(PayBookingViaBankTransferInput args, IUnitOfWork unitOfWork);
    void SignalWorkflowPayBookingViaCardDeleteBooking(string bookingId, IUnitOfWork unitOfWork);
    void SignalWorkflowPayBookingViaBankTransferSetPaymentStatus(string bookingId, SetPaymentStatusArgs executionArgs, IUnitOfWork unitOfWork);
    void SignalWorkflowPayBookingViaBankTransferDeleteBooking(string bookingId, IUnitOfWork unitOfWork);
}

public class TemporalOutboxPublisher(
    TemporalConfiguration temporalConfiguration,
    ITemporalHelperService temporalHelperService,
    ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor,
    ITemporalOutboxWorkflowExecutor<PayBookingViaCard> temporalOutboxPayBookingViaCardWorkflowExecutor,
    ITemporalOutboxWorkflowExecutor<PayBookingViaBankTransfer> temporalOutboxPayBookingViaBankTransferWorkflowExecutor)
    : ITemporalOutboxPublisher
{
    public void StartWorkflowPayBookingViaCard(PayBookingViaCardInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxPayBookingViaCardWorkflowExecutor.Execute(
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
        temporalOutboxPayBookingViaBankTransferWorkflowExecutor.Execute(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.PaidViaBankTransferPrefix}-{args.BookingId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);

    public void SignalWorkflowPayBookingViaCardDeleteBooking(string bookingId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            temporalHelperService.ToId($"{Constants.PaidViaCardPrefix}-{bookingId}"),
            typeof(PayBookingViaCard).GetMethod(nameof(PayBookingViaCard.DeleteBookingAsync))!.ToWorkflowSignalType(),
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowPayBookingViaBankTransferSetPaymentStatus(
        string bookingId,
        SetPaymentStatusArgs executionArgs,
        IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            temporalHelperService.ToId($"{Constants.PaidViaBankTransferPrefix}-{bookingId}"),
            typeof(PayBookingViaBankTransfer).GetMethod(nameof(PayBookingViaBankTransfer.SetPaymentStatusAsync))!.ToWorkflowSignalType(),
            executionArgs,
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowPayBookingViaBankTransferDeleteBooking(string bookingId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            temporalHelperService.ToId($"{Constants.PaidViaBankTransferPrefix}-{bookingId}"),
            typeof(PayBookingViaBankTransfer).GetMethod(nameof(PayBookingViaBankTransfer.DeleteBookingAsync))!.ToWorkflowSignalType(),
            new WorkflowSignalOptions(),
            unitOfWork);
}
