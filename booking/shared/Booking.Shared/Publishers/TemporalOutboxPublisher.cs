using Booking.Shared.Workflows;
using Booking.Shared.Workflows.Payment;
using Booking.Shared.Workflows.Payment.PayViaBankTransfer;
using Booking.Shared.Workflows.Payment.PayViaCard;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal.Configurations;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Booking.Shared.Publishers;

public interface ITemporalOutboxPublisher
{
    void StartWorkflowPayBookingViaCard(Models.Booking booking, IUnitOfWork unitOfWork);
    void SignalWorkflowPayBookingViaCardDeleteBooking(string bookingId, IUnitOfWork unitOfWork);
    void StartWorkflowPayBookingViaBankTransfer(Models.Booking booking, IUnitOfWork unitOfWork);
    void SignalWorkflowPayBookingViaBankTransferSetPaymentStatus(string bookingId, SetPaymentStatusArgs executionArgs, IUnitOfWork unitOfWork);
    void SignalWorkflowPayBookingViaBankTransferDeleteBooking(string bookingId, IUnitOfWork unitOfWork);
}

public class TemporalOutboxPublisher(
    TemporalConfiguration temporalConfiguration,
    ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor,
    ITemporalOutboxWorkflowExecutor<PayBookingViaCard> temporalOutboxPayBookingViaCardWorkflowExecutor,
    ITemporalOutboxWorkflowExecutor<PayBookingViaBankTransfer> temporalOutboxPayBookingViaBankTransferWorkflowExecutor)
    : ITemporalOutboxPublisher
{
    public void StartWorkflowPayBookingViaCard(Models.Booking booking, IUnitOfWork unitOfWork) =>
        temporalOutboxPayBookingViaCardWorkflowExecutor.Execute(
            new PayBookingViaCardInput(
                booking.Id,
                booking.PaymentExpiry,
                Enterprise.Shared.Extensions.ToSafeCollection(booking.InvoiceEmailList)),
            new WorkflowOptions
            {
                Id = $"{Constants.PaidViaCardPrefix}-{booking.Id}",
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);

    public void SignalWorkflowPayBookingViaCardDeleteBooking(string bookingId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            $"{Constants.PaidViaCardPrefix}-{bookingId}",
            typeof(PayBookingViaCard).GetMethod(nameof(PayBookingViaCard.DeleteBookingAsync))!.ToWorkflowSignalType(),
            new WorkflowSignalOptions(),
            unitOfWork);

    public void StartWorkflowPayBookingViaBankTransfer(Models.Booking booking, IUnitOfWork unitOfWork) =>
        temporalOutboxPayBookingViaBankTransferWorkflowExecutor.Execute(
            new PayBookingViaBankTransferInput(
                booking.Id,
                booking.PaymentExpiry,
                Enterprise.Shared.Extensions.ToSafeCollection(booking.InvoiceEmailList)),
            new WorkflowOptions
            {
                Id = $"{Constants.PaidViaBankTransferPrefix}-{booking.Id}",
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);

    public void SignalWorkflowPayBookingViaBankTransferSetPaymentStatus(
        string bookingId,
        SetPaymentStatusArgs executionArgs,
        IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            $"{Constants.PaidViaBankTransferPrefix}-{bookingId}",
            typeof(PayBookingViaBankTransfer).GetMethod(nameof(PayBookingViaBankTransfer.SetPaymentStatusAsync))!.ToWorkflowSignalType(),
            executionArgs,
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowPayBookingViaBankTransferDeleteBooking(string bookingId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            $"{Constants.PaidViaBankTransferPrefix}-{bookingId}",
            typeof(PayBookingViaBankTransfer).GetMethod(nameof(PayBookingViaBankTransfer.DeleteBookingAsync))!.ToWorkflowSignalType(),
            new WorkflowSignalOptions(),
            unitOfWork);
}
