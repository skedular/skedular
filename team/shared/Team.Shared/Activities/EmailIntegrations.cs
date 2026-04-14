using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Email;
using Enterprise.Shared.Grpc;
using Flurl;
using Microsoft.Extensions.Logging;
using Team.Shared.Configurations;
using Team.Shared.Repositories;
using Temporalio.Activities;

namespace Team.Shared.Activities;

public class EmailIntegrations(
    ApplicationConfiguration applicationConfiguration,
    EmailConfiguration emailConfiguration,
    CustomerConfiguration customerConfiguration,
    IEmailService emailService,
    IRepositoryFactory repositoryFactory,
    CustomerService.CustomerServiceClient customerServiceClient,
    ILogger<EmailIntegrations> logger)
{
    [Activity]
    public async Task SendInviteCustomerToJoinTeamNewCustomerAsync(string joinInvitationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(joinInvitationId, cancellationToken);
        if (joinInvitation is null)
        {
            logger.LogInformation("Send new-customer invite email skipped for invitation {JoinInvitationId}", joinInvitationId);
            return;
        }

        var inviterCustomerId = joinInvitation.CreatedBy.Id;
        var inviteeCustomerEmail = joinInvitation.Email;
        if (string.IsNullOrWhiteSpace(inviteeCustomerEmail))
        {
            logger.LogInformation(
                "Send new-customer invite email skipped because no invitee email exists for invitation {JoinInvitationId}",
                joinInvitationId);
            return;
        }

        var inviterCustomer = await customerServiceClient.Admin_GetAsync(
            new Admin_GetInput { CustomerId = inviterCustomerId },
            customerConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        await using var htmlTemplateStream =
            typeof(EmailIntegrations).Assembly.GetManifestResourceStream("Team.Shared.EmailTemplates.InviteToJoinTeamNewCustomer.template.html");
        ArgumentNullException.ThrowIfNull(htmlTemplateStream);
        using var htmlReader = new StreamReader(htmlTemplateStream);
        var html = await htmlReader.ReadToEndAsync(cancellationToken);

        await using var textTemplateStream =
            typeof(EmailIntegrations).Assembly.GetManifestResourceStream("Team.Shared.EmailTemplates.InviteToJoinTeamNewCustomer.template.txt");
        ArgumentNullException.ThrowIfNull(textTemplateStream);
        using var textReader = new StreamReader(textTemplateStream);
        var text = await textReader.ReadToEndAsync(cancellationToken);

        html = html
            .Replace("{{TEAM_NAME}}", joinInvitation.Team.Name)
            .Replace("{{INVITER_NAME}}", inviterCustomer.DisplayableName)
            .Replace("{{INVITATION_LINK}}", Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "signup"));

        text = text
            .Replace("{{TEAM_NAME}}", joinInvitation.Team.Name)
            .Replace("{{INVITER_NAME}}", inviterCustomer.DisplayableName)
            .Replace("{{INVITATION_LINK}}", Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "signup"));

        await emailService.SendRawEmailAsync(
            "Team Invitation - Skedular",
            text,
            html,
            emailConfiguration.InviteToJoinTeamNewCustomerEmailSender,
            [inviteeCustomerEmail],
            [],
            [],
            [],
            cancellationToken);

        logger.LogInformation("Send new-customer invite email completed for invitation {JoinInvitationId}", joinInvitationId);
    }

    [Activity]
    public async Task SendInviteCustomerToJoinTeamExistingCustomerAsync(string joinInvitationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(joinInvitationId, cancellationToken);
        if (joinInvitation is null)
        {
            logger.LogInformation("Send existing-customer invite email skipped for invitation {JoinInvitationId}", joinInvitationId);
            return;
        }

        var inviterCustomerId = joinInvitation.CreatedBy.Id;
        var inviteeCustomerId = joinInvitation.Invitee?.Id;
        if (string.IsNullOrWhiteSpace(inviteeCustomerId))
        {
            logger.LogInformation(
                "Send existing-customer invite email skipped because no invitee customer is linked for invitation {JoinInvitationId}",
                joinInvitationId);
            return;
        }

        var inviterCustomer = await customerServiceClient.Admin_GetAsync(
            new Admin_GetInput { CustomerId = inviterCustomerId },
            customerConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        var inviteeCustomer = await customerServiceClient.Admin_GetAsync(
            new Admin_GetInput { CustomerId = inviteeCustomerId },
            customerConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        await using var htmlTemplateStream =
            typeof(EmailIntegrations).Assembly.GetManifestResourceStream("Team.Shared.EmailTemplates.InviteToJoinTeamExistingCustomer.template.html");
        ArgumentNullException.ThrowIfNull(htmlTemplateStream);
        using var htmlReader = new StreamReader(htmlTemplateStream);
        var html = await htmlReader.ReadToEndAsync(cancellationToken);

        await using var textTemplateStream =
            typeof(EmailIntegrations).Assembly.GetManifestResourceStream("Team.Shared.EmailTemplates.InviteToJoinTeamExistingCustomer.template.txt");
        ArgumentNullException.ThrowIfNull(textTemplateStream);
        using var textReader = new StreamReader(textTemplateStream);
        var text = await textReader.ReadToEndAsync(cancellationToken);

        html = html
            .Replace("{{TEAM_NAME}}", joinInvitation.Team.Name)
            .Replace("{{INVITER_NAME}}", inviterCustomer.DisplayableName)
            .Replace("{{INVITATION_LINK}}", Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "notifications"));

        text = text
            .Replace("{{TEAM_NAME}}", joinInvitation.Team.Name)
            .Replace("{{INVITER_NAME}}", inviterCustomer.DisplayableName)
            .Replace("{{INVITATION_LINK}}", Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "notifications"));

        await emailService.SendRawEmailAsync(
            "Team Invitation - Skedular",
            text,
            html,
            emailConfiguration.InviteToJoinTeamNewCustomerEmailSender,
            inviteeCustomer.Identities.Select(item => item.Email).ToEmails(),
            [],
            [],
            [],
            cancellationToken);

        logger.LogInformation("Send existing-customer invite email completed for invitation {JoinInvitationId}", joinInvitationId);
    }
}
