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
using Team.Shared.Configurations;
using Team.Shared.Models;
using Event = Api.Shared.Clients.Events.Skedular.Notification.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Notification.V1.Value.Type;

namespace Team.Shared.Publishers;

public interface INotificationOutboxPublisher
{
    Task PublishInviteToJoinTeamNewCustomerAsync(
        Models.Team team,
        Customer inviterCustomer,
        string inviteeCustomerEmail,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);

    Task PublishInviteToJoinTeamExistingCustomerAsync(
        Models.Team team,
        Customer inviterCustomer,
        Customer inviteeCustomer,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);
}

public class InviteToJoinTeamNewCustomerDataData
{
    [JsonPropertyName("subject")] public string Subject { get; set; } = string.Empty;
    [JsonPropertyName("greetings")] public string Greetings { get; set; } = string.Empty;
    [JsonPropertyName("teamName")] public string TeamName { get; set; } = string.Empty;
    [JsonPropertyName("teamLink")] public string TeamLink { get; set; } = string.Empty;
    [JsonPropertyName("customerName")] public string CustomerName { get; set; } = string.Empty;
}

public class InviteToJoinTeamExistingCustomerData
{
    [JsonPropertyName("subject")] public string Subject { get; set; } = string.Empty;
    [JsonPropertyName("greetings")] public string Greetings { get; set; } = string.Empty;
    [JsonPropertyName("teamName")] public string TeamName { get; set; } = string.Empty;
    [JsonPropertyName("teamLink")] public string TeamLink { get; set; } = string.Empty;
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
    public async Task PublishInviteToJoinTeamNewCustomerAsync(
        Models.Team team,
        Customer inviterCustomer,
        string inviteeCustomerEmail,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var data = new InviteToJoinTeamExistingCustomerData
        {
            Subject = $"You are invited to join team {team.Name}",
            Greetings = "Hi",
            TeamName = string.IsNullOrWhiteSpace(team.Name) ? string.Empty : team.Name,
            TeamLink = Url.Combine(applicationConfiguration.WebAppBaseDomain, "notifications"),
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
                            emailConfiguration.InviteToJoinTeamNewCustomerEmailTemplateName,
                        TemplateData = templateData,
                        Sender = emailConfiguration.InviteToJoinTeamNewCustomerEmailSender
                    }
                }
            }
        };

        @event.Data.AfterState.Email.ToAddresses.Add(inviteeCustomerEmail);

        await publisher.PublishAsync(key, @event, unitOfWork, cancellationToken);
    }

    public async Task PublishInviteToJoinTeamExistingCustomerAsync(
        Models.Team team,
        Customer inviterCustomer,
        Customer inviteeCustomer,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var data = new InviteToJoinTeamNewCustomerDataData
        {
            Subject = $"You are invited to join team {team.Name}",
            Greetings = $"Hi {inviteeCustomer.GetCustomerName()}",
            TeamName = string.IsNullOrWhiteSpace(team.Name) ? string.Empty : team.Name,
            TeamLink = Url.Combine(applicationConfiguration.WebAppBaseDomain, "notifications"),
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
                            emailConfiguration.InviteToJoinTeamExistingCustomerEmailTemplateName,
                        TemplateData = templateData,
                        Sender = emailConfiguration.InviteToJoinTeamExistingCustomerEmailSender
                    }
                }
            }
        };

        @event.Data.AfterState.Email.ToAddresses.AddRange(inviteeCustomer.Identities.Select(item => item.Email));

        await publisher.PublishAsync(key, @event, unitOfWork, cancellationToken);
    }
}
