using Api.Shared.Clients.Events.Skedular.Booking.V1.Key;
using Api.Shared.Clients.Events.Skedular.Booking.V1.Value;
using Booking.Shared.Mappers;
using Booking.Shared.Workflows;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal.Configurations;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;
using Event = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Type;

namespace Booking.Shared.Publishers;

public interface IBookingOutboxPublisher
{
    void PublishBookings(IEnumerable<Models.Booking> bookings, IUnitOfWork unitOfWork);
    void StartWorkflowPayBookingViaCard(Models.Booking booking, IUnitOfWork unitOfWork);
    void SignalWorkflowPayBookingViaCardSetPaymentStatus(string bookingId, SetPaymentStatusArgs executionArgs, IUnitOfWork unitOfWork);
    void SignalWorkflowPayBookingViaCardDeleteBooking(string bookingId, IUnitOfWork unitOfWork);
    void StartWorkflowPayBookingViaBankTransfer(Models.Booking booking, IUnitOfWork unitOfWork);
    void SignalWorkflowPayBookingViaBankTransferSetPaymentStatus(string bookingId, SetPaymentStatusArgs executionArgs, IUnitOfWork unitOfWork);
    void SignalWorkflowPayBookingViaBankTransferDeleteBooking(string bookingId, IUnitOfWork unitOfWork);
}

public class BookingOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaOutboxEventPublisher<Key, Event> publisher,
    TemporalConfiguration temporalConfiguration,
    ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor,
    ITemporalOutboxWorkflowExecutor<PayBookingViaCard> temporalOutboxPayBookingViaCardWorkflowExecutor,
    ITemporalOutboxWorkflowExecutor<PayBookingViaBankTransfer> temporalOutboxPayBookingViaBankTransferWorkflowExecutor)
    : IBookingOutboxPublisher
{
    public void PublishBookings(IEnumerable<Models.Booking> bookings, IUnitOfWork unitOfWork)
    {
        foreach (var booking in bookings)
        {
            publisher.Publish(
                new Key { BookingId = booking.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        booking.IsDeleted() ? Type.BookingDeleted : Type.BookingUpserted,
                        context.GetCorrelationId()),
                    Data = new Data { Booking = mapper.MapTo(booking) }
                },
                unitOfWork);
        }
    }

    public void StartWorkflowPayBookingViaCard(Models.Booking booking, IUnitOfWork unitOfWork) =>
        temporalOutboxPayBookingViaCardWorkflowExecutor.Execute(
            new PayBookingViaCardInput(
                booking.Id,
                booking.PaymentExpiry,
                booking.SendInvoice ?? false,
                Enterprise.Shared.Extensions.ToSafeCollection(booking.InvoiceEmailList)),
            new WorkflowOptions
            {
                Id = $"{Constants.PaidViaCardPrefix}-{booking.Id}",
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);

    public void SignalWorkflowPayBookingViaCardSetPaymentStatus(
        string bookingId,
        SetPaymentStatusArgs executionArgs,
        IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            $"{Constants.PaidViaCardPrefix}-{bookingId}",
            typeof(PayBookingViaCard).GetMethod(nameof(PayBookingViaCard.SetPaymentStatusAsync))!.ToWorkflowSignalType(),
            executionArgs,
            new WorkflowSignalOptions(),
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
                booking.SendInvoice ?? false,
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
