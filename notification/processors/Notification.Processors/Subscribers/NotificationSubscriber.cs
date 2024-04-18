using Api.Shared.Clients.Events.UnityHub.Notification.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Notification.V1.Value;
using Confluent.Kafka;
using Enterprise.Shared.Kafka.Consume;
using Notification.Processors.Services;
using Type = Api.Shared.Clients.Events.UnityHub.Notification.V1.Value.Type;

namespace Notification.Processors.Subscribers;

public class NotificationSubscriber(IEmailService emailService) : IEventSubscriber<Key, Event>
{
    public async Task HandleAsync(Headers headers, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.NotificationUpserted:
                {
                    if (@event.Data.AfterState.NotificationType != NotificationType.Email)
                    {
                        return;
                    }

                    var email = @event.Data.AfterState.Email;
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

            default:
                return;
        }
    }
}
