using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Shared.Clients.Events.UnityHub.Notification.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Notification.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Random;
using Flurl;
using Location.Shared.Configurations;
using Location.Shared.Models;
using Event = Api.Shared.Clients.Events.UnityHub.Notification.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.Notification.V1.Value.Type;

namespace Location.Shared.Publishers;

public interface INotificationOutboxPublisher
{
    Task PublishInviteToJoinLocationNewCustomerAsync(
        Models.Location location,
        Customer inviterCustomer,
        string inviteeCustomerEmail,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);

    Task PublishInviteToJoinLocationExistingCustomerAsync(
        Models.Location location,
        Customer inviterCustomer,
        Customer inviteeCustomer,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);
}

public class InviteToJoinLocationNewCustomerDataData
{
    [JsonPropertyName("subject")] public string Subject { get; set; } = string.Empty;
    [JsonPropertyName("greetings")] public string Greetings { get; set; } = string.Empty;
    [JsonPropertyName("locationName")] public string LocationName { get; set; } = string.Empty;
    [JsonPropertyName("locationLink")] public string LocationLink { get; set; } = string.Empty;
    [JsonPropertyName("customerName")] public string CustomerName { get; set; } = string.Empty;
}

public class InviteToJoinLocationExistingCustomerData
{
    [JsonPropertyName("subject")] public string Subject { get; set; } = string.Empty;
    [JsonPropertyName("greetings")] public string Greetings { get; set; } = string.Empty;
    [JsonPropertyName("locationName")] public string LocationName { get; set; } = string.Empty;
    [JsonPropertyName("locationLink")] public string LocationLink { get; set; } = string.Empty;
    [JsonPropertyName("customerName")] public string CustomerName { get; set; } = string.Empty;
}

public class NotificationOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    EmailConfiguration emailConfiguration,
    IContext context,
    IOutboxEventPublisher<Key, Event> publisher,
    IRandomHelper randomHelper)
    : INotificationOutboxPublisher
{
    public async Task PublishInviteToJoinLocationNewCustomerAsync(
        Models.Location location,
        Customer inviterCustomer,
        string inviteeCustomerEmail,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var data = new InviteToJoinLocationExistingCustomerData
        {
            Subject = $"You are invited to join location {location.Name}",
            Greetings = "Hi",
            LocationName = string.IsNullOrWhiteSpace(location.Name) ? string.Empty : location.Name,
            LocationLink = Url.Combine(applicationConfiguration.WebAppBaseDomain, "notifications"),
            CustomerName = inviterCustomer.GetCustomerName()
        };
        var templateData = JsonSerializer.Serialize(data);
        var key = new Key { Email = inviteeCustomerEmail };

        var @event = new Event
        {
            Metadata = Event.NewMetadata(
                applicationConfiguration.DomainSource,
                applicationConfiguration.AppSource,
                Type.NotificationUpserted,
                context.GetCorrelationId()),
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
                            emailConfiguration.InviteToJoinLocationNewCustomerEmailTemplateName,
                        TemplateData = templateData,
                        Sender = emailConfiguration.InviteToJoinLocationNewCustomerEmailSender
                    }
                }
            }
        };

        @event.Data.AfterState.Email.ToAddresses.Add(inviteeCustomerEmail);

        await publisher.PublishAsync(key, @event, unitOfWork, cancellationToken);
    }

    public async Task PublishInviteToJoinLocationExistingCustomerAsync(
        Models.Location location,
        Customer inviterCustomer,
        Customer inviteeCustomer,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var data = new InviteToJoinLocationNewCustomerDataData
        {
            Subject = $"You are invited to join location {location.Name}",
            Greetings = $"Hi {inviteeCustomer.GetCustomerName()}",
            LocationName = string.IsNullOrWhiteSpace(location.Name) ? string.Empty : location.Name,
            LocationLink = Url.Combine(applicationConfiguration.WebAppBaseDomain, "notifications"),
            CustomerName = inviterCustomer.GetCustomerName()
        };
        var templateData = JsonSerializer.Serialize(data);
        var key = new Key { CustomerId = inviteeCustomer.Id };

        var @event = new Event
        {
            Metadata = Event.NewMetadata(
                applicationConfiguration.DomainSource,
                applicationConfiguration.AppSource,
                Type.NotificationUpserted,
                context.GetCorrelationId()),
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
                            emailConfiguration.InviteToJoinLocationExistingCustomerEmailTemplateName,
                        TemplateData = templateData,
                        Sender = emailConfiguration.InviteToJoinLocationExistingCustomerEmailSender
                    }
                }
            }
        };

        @event.Data.AfterState.Email.ToAddresses.AddRange(inviteeCustomer.Identities.Select(item => item.Email));

        await publisher.PublishAsync(key, @event, unitOfWork, cancellationToken);
    }
}
