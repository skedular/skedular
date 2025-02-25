using Api.Shared.Clients.Events.Skedular.Notification.V1.Key;
using Api.Shared.Clients.Events.Skedular.Notification.V1.Value;
using Enterprise.Shared.Kafka.Consume;
using Notification.Processors.Services;
using Type = Api.Shared.Clients.Events.Skedular.Notification.V1.Value.Type;

namespace Notification.Processors.Subscribers;

public class NotificationSubscriber(IEmailService emailService) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.NotificationUpserted:
                {
                    if (@event.Data.Notification.NotificationType != NotificationType.Email)
                    {
                        return EventSubscriberResults.Success;
                    }

                    var email = @event.Data.Notification.Email;
                    await emailService.SendEmailAsync(
                        email.TemplateId,
                        email.TemplateData,
                        email.Sender,
                        email.ToAddresses.ToList(),
                        email.CcAddresses.ToList(),
                        email.BccAddresses.ToList(),
                        cancellationToken);
                }

                break;
        }

        return EventSubscriberResults.Success;
    }
}
