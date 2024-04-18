using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Shared.Clients.Events.UnityHub.Notification.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Notification.V1.Value;
using Customer.Shared.Configurations;
using Customer.Shared.Models;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Random;
using Event = Api.Shared.Clients.Events.UnityHub.Notification.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.Notification.V1.Value.Type;

namespace Customer.Shared.Publishers;

public interface INotificationOutboxPublisher
{
    Task PublishNewCustomerFeedbackSubmittedAsync(
        CustomerFeedback customerFeedback,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);
}

public class NewCustomerFeedbackData
{
    [JsonPropertyName("subject")] public string Subject { get; set; } = string.Empty;
    [JsonPropertyName("feedbackContent")] public string FeedbackContent { get; set; } = string.Empty;
}

public class NotificationOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    EmailConfiguration emailConfiguration,
    IRandomHelper randomHelper,
    IContext context,
    IOutboxEventPublisher<Key, Event> publisher)
    : INotificationOutboxPublisher
{
    public async Task PublishNewCustomerFeedbackSubmittedAsync(
        CustomerFeedback customerFeedback,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var data = new NewCustomerFeedbackData
        {
            Subject =
                $"You received new feedback from {customerFeedback.Customer.GetCustomerName()} through Web channel",
            FeedbackContent = string.IsNullOrWhiteSpace(customerFeedback.Content)
                ? string.Empty
                : customerFeedback.Content
        };

        var templateData = JsonSerializer.Serialize(data);
        var key = new Key { CustomerId = customerFeedback.Customer.Id };
        var @event = new Event
        {
            Metadata = Event.NewMetadata(
                applicationConfiguration.DomainSource,
                applicationConfiguration.AppSource,
                Type.NotificationUpserted,
                context.PropertyBag.CorrelationId),
            Data = new Data
            {
                AfterState = new Notification
                {
                    Id = randomHelper.Generate(),
                    NotificationType = NotificationType.Email,
                    Email = new EmailDetails
                    {
                        Id = randomHelper.Generate(),
                        TemplateId =
                            emailConfiguration.NewCustomerFeedbackThroughWebSubmittedEmailTemplateName,
                        TemplateData = templateData,
                        Sender = emailConfiguration.NewCustomerFeedbackThroughWebSubmittedEmailSender
                    }
                }
            }
        };

        @event.Data.AfterState.Email.ToAddresses.AddRange(emailConfiguration
            .NewCustomerFeedbackThroughWebSubmittedEmailReceivers);

        await publisher.PublishAsync(key, @event, unitOfWork, cancellationToken);
    }
}
