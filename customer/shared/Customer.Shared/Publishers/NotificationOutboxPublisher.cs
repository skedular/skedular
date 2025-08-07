using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Shared.Clients.Events.Skedular.Notification.V1.Key;
using Api.Shared.Clients.Events.Skedular.Notification.V1.Value;
using Api.Shared.Services.Models;
using Customer.Shared.Configurations;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Random;
using Event = Api.Shared.Clients.Events.Skedular.Notification.V1.Value.Event;
using NotificationType = Api.Shared.Clients.Events.Skedular.Notification.V1.Value.NotificationType;
using Type = Api.Shared.Clients.Events.Skedular.Notification.V1.Value.Type;

namespace Customer.Shared.Publishers;

public interface INotificationOutboxPublisher
{
    void PublishNewCustomerJoinedSubmitted(Models.Customer customer, IUnitOfWork unitOfWork);
}

public class NewCustomerFeedbackData
{
    [JsonPropertyName("subject")] public string Subject { get; set; } = string.Empty;
    [JsonPropertyName("feedbackContent")] public string FeedbackContent { get; set; } = string.Empty;
}

public class NewCustomerJoinedData
{
    [JsonPropertyName("subject")] public string Subject { get; set; } = string.Empty;
    [JsonPropertyName("content")] public string Content { get; set; } = string.Empty;
}

public class NotificationOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    EmailConfiguration emailConfiguration,
    IRandomHelper randomHelper,
    IContext context,
    IKafkaOutboxEventPublisher<Key, Event> publisher) : INotificationOutboxPublisher
{
    public void PublishNewCustomerJoinedSubmitted(Models.Customer customer, IUnitOfWork unitOfWork)
    {
        if (!emailConfiguration.EnableNewCustomerJoinedEmail)
        {
            return;
        }

        var emails = customer.Identities.ToStringEmails();
        var data = new NewCustomerJoinedData
        {
            Subject = "New customer has joined Skedular",
            Content = $"New customer with ID {customer.Id} and email(s) {emails} has joined Skedular"
        };

        var templateData = JsonSerializer.Serialize(data);
        var key = new Key { CustomerId = customer.Id };
        var @event = new Event
        {
            Metadata = Event.NewMetadata(
                applicationConfiguration.DomainSource,
                applicationConfiguration.AppSource,
                Type.NotificationUpserted,
                context.GetCorrelationId()),
            Data = new Data
            {
                Notification = new Notification
                {
                    Id = randomHelper.Generate(),
                    NotificationType = NotificationType.Email,
                    Email = new EmailDetails
                    {
                        Id = randomHelper.Generate(),
                        TemplateId = emailConfiguration.NewCustomerJoinedEmailTemplateName,
                        TemplateData = templateData,
                        Sender = emailConfiguration.NewCustomerJoinedEmailSender
                    }
                }
            }
        };

        @event.Data.Notification.Email.ToAddresses.AddRange(emailConfiguration.NewCustomerJoinedEmailReceivers);

        publisher.Publish(key, @event, unitOfWork);
    }
}
