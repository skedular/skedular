using Api.Shared.Clients.Events.Skedular.Booking.V1.Key;
using Api.Shared.Clients.Events.Skedular.Booking.V1.Value;
using Booking.Shared.Mappers;
using Booking.Shared.Workflows;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
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
    void ExecuteWorkflowBookingPaidThroughStripe(IEnumerable<Models.Booking> bookings, IUnitOfWork unitOfWork);
}

public class BookingOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaOutboxEventPublisher<Key, Event> publisher,
    TemporalConfiguration temporalConfiguration,
    ITemporalOutboxWorkflowExecutor<BookingPaidThroughStripe, BookingPaidThroughStripeInput> temporalOutboxBookingPaidThroughStripeWorkflowExecutor)
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
                        booking.IsNotDeleted() ? Type.BookingUpserted : Type.BookingDeleted,
                        context.GetCorrelationId()),
                    Data = new Data { Booking = mapper.MapTo(booking) }
                },
                unitOfWork);
        }
    }

    public void ExecuteWorkflowBookingPaidThroughStripe(IEnumerable<Models.Booking> bookings, IUnitOfWork unitOfWork)
    {
        foreach (var booking in bookings)
        {
            temporalOutboxBookingPaidThroughStripeWorkflowExecutor.Execute(
                new BookingPaidThroughStripeInput(booking.Id),
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
}
