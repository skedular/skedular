using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Email;
using Enterprise.Shared.Grpc;
using Flurl;
using Team.Shared.Configurations;
using Team.Shared.Repositories;
using Temporalio.Activities;

namespace Team.Shared.Activities;

public record SendInviteCustomerToJoinTeamNewCustomerInput(string TeamId, string InviterCustomerId, string InviteeCustomerEmail);

public record SendInviteCustomerToJoinTeamExistingCustomerInput(string TeamId, string InviterCustomerId, string InviteeCustomerId);

public class EmailIntegrations(
    ApplicationConfiguration applicationConfiguration,
    EmailConfiguration emailConfiguration,
    CustomerConfiguration customerConfiguration,
    IRepositoryFactory repositoryFactory,
    IEmailService emailService,
    CustomerService.CustomerServiceClient customerServiceClient)
{
    [Activity]
    public async Task SendInviteCustomerToJoinTeamNewCustomerAsync(SendInviteCustomerToJoinTeamNewCustomerInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var team = await repositoryFactory.TeamRepository.GetByIdAsync(args.TeamId, cancellationToken);
        if (team is null)
        {
            return;
        }

        var inviterCustomer = await customerServiceClient.Admin_GetAsync(
            new Admin_GetInput { CustomerId = args.InviterCustomerId },
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
            .Replace("{{TEAM_NAME}}", team.Name)
            .Replace("{{INVITER_NAME}}", inviterCustomer.DisplayableName)
            .Replace("{{INVITATION_LINK}}", Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "signup"));

        text = text
            .Replace("{{TEAM_NAME}}", team.Name)
            .Replace("{{INVITER_NAME}}", inviterCustomer.DisplayableName)
            .Replace("{{INVITATION_LINK}}", Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "signup"));

        await emailService.SendRawEmailAsync(
            "Team Invitation - Skedular",
            text,
            html,
            $"Skedular {emailConfiguration.InviteToJoinTeamNewCustomerEmailSender}",
            [args.InviteeCustomerEmail],
            [],
            [],
            [],
            cancellationToken);
    }

    [Activity]
    public async Task SendInviteCustomerToJoinTeamExistingCustomerAsync(SendInviteCustomerToJoinTeamExistingCustomerInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var team = await repositoryFactory.TeamRepository.GetByIdAsync(args.TeamId, cancellationToken);
        if (team is null)
        {
            return;
        }

        var inviterCustomer = await customerServiceClient.Admin_GetAsync(
            new Admin_GetInput { CustomerId = args.InviterCustomerId },
            customerConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        var inviteeCustomer = await customerServiceClient.Admin_GetAsync(
            new Admin_GetInput { CustomerId = args.InviteeCustomerId },
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
            .Replace("{{TEAM_NAME}}", team.Name)
            .Replace("{{INVITER_NAME}}", inviterCustomer.DisplayableName)
            .Replace("{{INVITATION_LINK}}", Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "notifications"));

        text = text
            .Replace("{{TEAM_NAME}}", team.Name)
            .Replace("{{INVITER_NAME}}", inviterCustomer.DisplayableName)
            .Replace("{{INVITATION_LINK}}", Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "notifications"));

        await emailService.SendRawEmailAsync(
            "Team Invitation - Skedular",
            text,
            html,
            $"Skedular {emailConfiguration.InviteToJoinTeamNewCustomerEmailSender}",
            inviteeCustomer.Identities.Select(item => item.Email).ToEmails(),
            [],
            [],
            [],
            cancellationToken);
    }
}
