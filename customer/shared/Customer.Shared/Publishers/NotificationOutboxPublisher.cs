using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Shared.Clients.Events.Skedular.Notification.V1.Key;
using Api.Shared.Clients.Events.Skedular.Notification.V1.Value;
using Customer.Shared.Configurations;
using Customer.Shared.Models;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Random;
using Event = Api.Shared.Clients.Events.Skedular.Notification.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Notification.V1.Value.Type;

namespace Customer.Shared.Publishers;

public interface INotificationOutboxPublisher
{
    void PublishNewCustomerFeedbackSubmitted(CustomerFeedback customerFeedback, IUnitOfWork unitOfWork);
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
    IKafkaOutboxEventPublisher<Key, Event> publisher)
    : INotificationOutboxPublisher
{
    public void PublishNewCustomerFeedbackSubmitted(CustomerFeedback customerFeedback, IUnitOfWork unitOfWork)
    {
        var emails = GetEmails(customerFeedback.Customer);
        var channel = customerFeedback.Channel switch
        {
            FeedbackChannelType.Web => "Web",
            FeedbackChannelType.Slack => "Slack",
            FeedbackChannelType.MsTeams => "MsTeams",
            _ => throw new ArgumentOutOfRangeException()
        };

        var data = new NewCustomerFeedbackData
        {
            Subject = $"You received new feedback from {customerFeedback.Customer.GetCustomerName()} through {channel} channel",
            FeedbackContent = string.IsNullOrWhiteSpace(customerFeedback.Content)
                ? $"Email(s): {emails}"
                : $"Email(s): {emails}{Environment.NewLine}{customerFeedback.Content}"
        };

        var templateData = JsonSerializer.Serialize(data);
        var key = new Key { CustomerId = customerFeedback.Customer.Id };
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
                        TemplateId = emailConfiguration.NewCustomerFeedbackThroughWebSubmittedEmailTemplateName,
                        TemplateData = templateData,
                        Sender = emailConfiguration.NewCustomerFeedbackThroughWebSubmittedEmailSender
                    }
                }
            }
        };

        @event.Data.Notification.Email.ToAddresses.AddRange(emailConfiguration.NewCustomerFeedbackThroughWebSubmittedEmailReceivers);

        publisher.Publish(key, @event, unitOfWork);
    }

    public void PublishNewCustomerJoinedSubmitted(Models.Customer customer, IUnitOfWork unitOfWork)
    {
        if (!emailConfiguration.EnableNewCustomerJoinedThroughWebEmail)
        {
            return;
        }

        var emails = GetEmails(customer);
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
                        TemplateId = emailConfiguration.NewCustomerJoinedThroughWebEmailTemplateName,
                        TemplateData = templateData,
                        Sender = emailConfiguration.NewCustomerJoinedThroughWebEmailSender
                    }
                }
            }
        };

        @event.Data.Notification.Email.ToAddresses.AddRange(emailConfiguration.NewCustomerJoinedThroughWebEmailReceivers);

        publisher.Publish(key, @event, unitOfWork);
    }

    private static string GetEmails(Models.Customer customer) =>
        customer.Identities
            .Aggregate(string.Empty, (acc, identity) => $"{acc}, {identity.Email}")
            .Trim(',')
            .Trim();
}
