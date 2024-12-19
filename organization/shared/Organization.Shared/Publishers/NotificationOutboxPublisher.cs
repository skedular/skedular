using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Shared.Clients.Events.Skedular.Notification.V1.Key;
using Api.Shared.Clients.Events.Skedular.Notification.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Random;
using Flurl;
using Organization.Shared.Configurations;
using Organization.Shared.Models;
using Event = Api.Shared.Clients.Events.Skedular.Notification.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Notification.V1.Value.Type;

namespace Organization.Shared.Publishers;

public interface INotificationOutboxPublisher
{
    Task PublishInviteToJoinOrganizationNewCustomerAsync(
        Models.Organization organization,
        Customer inviterCustomer,
        string inviteeCustomerEmail,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);

    Task PublishInviteToJoinOrganizationExistingCustomerAsync(
        Models.Organization organization,
        Customer inviterCustomer,
        Customer inviteeCustomer,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);
}

public class InviteToJoinOrganizationNewCustomerDataData
{
    [JsonPropertyName("subject")] public string Subject { get; set; } = string.Empty;
    [JsonPropertyName("greetings")] public string Greetings { get; set; } = string.Empty;
    [JsonPropertyName("organizationName")] public string OrganizationName { get; set; } = string.Empty;
    [JsonPropertyName("organizationLink")] public string OrganizationLink { get; set; } = string.Empty;
    [JsonPropertyName("customerName")] public string CustomerName { get; set; } = string.Empty;
}

public class InviteToJoinOrganizationExistingCustomerData
{
    [JsonPropertyName("subject")] public string Subject { get; set; } = string.Empty;
    [JsonPropertyName("greetings")] public string Greetings { get; set; } = string.Empty;
    [JsonPropertyName("organizationName")] public string OrganizationName { get; set; } = string.Empty;
    [JsonPropertyName("organizationLink")] public string OrganizationLink { get; set; } = string.Empty;
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
    public async Task PublishInviteToJoinOrganizationNewCustomerAsync(
        Models.Organization organization,
        Customer inviterCustomer,
        string inviteeCustomerEmail,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var data = new InviteToJoinOrganizationExistingCustomerData
        {
            Subject = $"You are invited to join organization {organization.Name}",
            Greetings = "Hi",
            OrganizationName = string.IsNullOrWhiteSpace(organization.Name) ? string.Empty : organization.Name,
            OrganizationLink = Url.Combine(applicationConfiguration.WebAppBaseDomain, "notifications"),
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
                            emailConfiguration.InviteToJoinOrganizationNewCustomerEmailTemplateName,
                        TemplateData = templateData,
                        Sender = emailConfiguration.InviteToJoinOrganizationNewCustomerEmailSender
                    }
                }
            }
        };

        @event.Data.AfterState.Email.ToAddresses.Add(inviteeCustomerEmail);

        await publisher.PublishAsync(key, @event, unitOfWork, cancellationToken);
    }

    public async Task PublishInviteToJoinOrganizationExistingCustomerAsync(
        Models.Organization organization,
        Customer inviterCustomer,
        Customer inviteeCustomer,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var data = new InviteToJoinOrganizationNewCustomerDataData
        {
            Subject = $"You are invited to join organization {organization.Name}",
            Greetings = $"Hi {inviteeCustomer.GetCustomerName()}",
            OrganizationName = string.IsNullOrWhiteSpace(organization.Name) ? string.Empty : organization.Name,
            OrganizationLink = Url.Combine(applicationConfiguration.WebAppBaseDomain, "notifications"),
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
                            emailConfiguration.InviteToJoinOrganizationExistingCustomerEmailTemplateName,
                        TemplateData = templateData,
                        Sender = emailConfiguration.InviteToJoinOrganizationExistingCustomerEmailSender
                    }
                }
            }
        };

        @event.Data.AfterState.Email.ToAddresses.AddRange(inviteeCustomer.Identities.Select(item => item.Email));

        await publisher.PublishAsync(key, @event, unitOfWork, cancellationToken);
    }
}
