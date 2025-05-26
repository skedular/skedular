using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Shared.Clients.Events.Skedular.Notification.V1.Key;
using Api.Shared.Clients.Events.Skedular.Notification.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Random;
using Slack.Shared.Configurations;
using Slack.Shared.Models;
using Event = Api.Shared.Clients.Events.Skedular.Notification.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Notification.V1.Value.Type;

namespace Slack.Shared.Publishers;

public interface INotificationOutboxPublisher
{
    void PublishNewSlackWorkspaceJoinedSubmitted(Workspace workspace, IUnitOfWork unitOfWork);
}

public class NewSlackWorkspaceJoinedData
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
    public void PublishNewSlackWorkspaceJoinedSubmitted(Workspace workspace, IUnitOfWork unitOfWork)
    {
        if (!emailConfiguration.EnableNewSlackWorkspaceJoinedEmail)
        {
            return;
        }

        var data = new NewSlackWorkspaceJoinedData
        {
            Subject = "New slack workspace has joined Skedular",
            Content = $"New slack workspace with ID {workspace.Id} and name {workspace.Name} has joined Skedular"
        };

        var templateData = JsonSerializer.Serialize(data);
        var key = new Key { CustomerId = workspace.Id };
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
                        TemplateId = emailConfiguration.NewSlackWorkspaceJoinedEmailTemplateName,
                        TemplateData = templateData,
                        Sender = emailConfiguration.NewSlackWorkspaceJoinedEmailSender
                    }
                }
            }
        };

        @event.Data.Notification.Email.ToAddresses.AddRange(emailConfiguration.NewSlackWorkspaceJoinedEmailReceivers);

        publisher.Publish(key, @event, unitOfWork);
    }
}
