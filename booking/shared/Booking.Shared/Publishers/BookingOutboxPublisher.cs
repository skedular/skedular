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
    void ExecuteWorkflowPayBookingUsingStripeCheckoutSession(IEnumerable<Models.Booking> bookings, IUnitOfWork unitOfWork);

    void SignalWorkflowPayBookingUsingStripeCheckoutSessionSetPaymentStatus(
        string bookingId,
        SetPaymentStatusArgs executionArgs,
        IUnitOfWork unitOfWork);

    void SignalWorkflowPayBookingUsingStripeCheckoutSessionDeleteBooking(string bookingId, IUnitOfWork unitOfWork);
}

public class BookingOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaOutboxEventPublisher<Key, Event> publisher,
    TemporalConfiguration temporalConfiguration,
    ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor,
    ITemporalOutboxWorkflowExecutor<PayBookingUsingStripeCheckoutSession> temporalOutboxPayBookingUsingStripeCheckoutSessionWorkflowExecutor)
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

    public void ExecuteWorkflowPayBookingUsingStripeCheckoutSession(IEnumerable<Models.Booking> bookings, IUnitOfWork unitOfWork)
    {
        foreach (var booking in bookings)
        {
            temporalOutboxPayBookingUsingStripeCheckoutSessionWorkflowExecutor.Execute(
                new BookingPaidThroughStripeInput(booking.Id, booking.BookingCheckoutSessionExpiry),
                new WorkflowOptions
                {
                    Id = booking.Id,
                    TaskQueue = temporalConfiguration.Worker.TaskQueue,
                    RetryPolicy = null,
                    IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
                },
                unitOfWork);
        }
    }

    public void SignalWorkflowPayBookingUsingStripeCheckoutSessionSetPaymentStatus(
        string bookingId,
        SetPaymentStatusArgs executionArgs,
        IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            bookingId,
            typeof(PayBookingUsingStripeCheckoutSession).GetMethod(nameof(PayBookingUsingStripeCheckoutSession.SetPaymentStatusAsync))!
                .ToWorkflowSignalType(),
            executionArgs,
            new WorkflowSignalOptions(),
            unitOfWork);

    public void SignalWorkflowPayBookingUsingStripeCheckoutSessionDeleteBooking(string bookingId, IUnitOfWork unitOfWork) =>
        temporalSignalOutboxWorkflowExecutor.Signal(
            bookingId,
            typeof(PayBookingUsingStripeCheckoutSession).GetMethod(nameof(PayBookingUsingStripeCheckoutSession.DeleteBookingAsync))!
                .ToWorkflowSignalType(),
            new WorkflowSignalOptions(),
            unitOfWork);
}
