using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Email;
using Enterprise.Shared.Grpc;
using Flurl;
using Organization.Shared.Configurations;
using Organization.Shared.Repositories;
using Temporalio.Activities;

namespace Organization.Shared.Activities;

public class EmailIntegrations(
    ApplicationConfiguration applicationConfiguration,
    EmailConfiguration emailConfiguration,
    CustomerConfiguration customerConfiguration,
    IRepositoryFactory repositoryFactory,
    IEmailService emailService,
    CustomerService.CustomerServiceClient customerServiceClient)
{
    [Activity]
    public async Task SendInviteCustomerToJoinOrganizationNewCustomerAsync(string joinInvitationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(joinInvitationId, cancellationToken);
        if (joinInvitation is null)
        {
            return;
        }

        var inviterCustomerId = joinInvitation.CreatedBy.Id;
        var inviteeCustomerEmail = joinInvitation.Email;
        if (string.IsNullOrWhiteSpace(inviteeCustomerEmail))
        {
            return;
        }

        var inviterCustomer = await customerServiceClient.Admin_GetAsync(
            new Admin_GetInput { CustomerId = inviterCustomerId },
            customerConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        await using var htmlTemplateStream =
            typeof(EmailIntegrations).Assembly.GetManifestResourceStream(
                "Organization.Shared.EmailTemplates.InviteToJoinOrganizationNewCustomer.template.html");
        ArgumentNullException.ThrowIfNull(htmlTemplateStream);
        using var htmlReader = new StreamReader(htmlTemplateStream);
        var html = await htmlReader.ReadToEndAsync(cancellationToken);

        await using var textTemplateStream =
            typeof(EmailIntegrations).Assembly.GetManifestResourceStream(
                "Organization.Shared.EmailTemplates.InviteToJoinOrganizationNewCustomer.template.txt");
        ArgumentNullException.ThrowIfNull(textTemplateStream);
        using var textReader = new StreamReader(textTemplateStream);
        var text = await textReader.ReadToEndAsync(cancellationToken);

        html = html
            .Replace("{{ORGANIZATION_NAME}}", joinInvitation.Organization.Name)
            .Replace("{{INVITER_NAME}}", inviterCustomer.DisplayableName)
            .Replace("{{INVITATION_LINK}}", Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "signup"));

        text = text
            .Replace("{{ORGANIZATION_NAME}}", joinInvitation.Organization.Name)
            .Replace("{{INVITER_NAME}}", inviterCustomer.DisplayableName)
            .Replace("{{INVITATION_LINK}}", Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "signup"));

        await emailService.SendRawEmailAsync(
            "Organization Invitation - Skedular",
            text,
            html,
            emailConfiguration.InviteToJoinOrganizationNewCustomerEmailSender,
            [inviteeCustomerEmail],
            [],
            [],
            [],
            cancellationToken);
    }

    [Activity]
    public async Task SendInviteCustomerToJoinOrganizationExistingCustomerAsync(string joinInvitationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(joinInvitationId, cancellationToken);
        if (joinInvitation is null)
        {
            return;
        }

        var inviterCustomerId = joinInvitation.CreatedBy.Id;
        var inviteeCustomerId = joinInvitation.Invitee?.Id;
        if (string.IsNullOrWhiteSpace(inviteeCustomerId))
        {
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
            typeof(EmailIntegrations).Assembly.GetManifestResourceStream(
                "Organization.Shared.EmailTemplates.InviteToJoinOrganizationExistingCustomer.template.html");
        ArgumentNullException.ThrowIfNull(htmlTemplateStream);
        using var htmlReader = new StreamReader(htmlTemplateStream);
        var html = await htmlReader.ReadToEndAsync(cancellationToken);

        await using var textTemplateStream =
            typeof(EmailIntegrations).Assembly.GetManifestResourceStream(
                "Organization.Shared.EmailTemplates.InviteToJoinOrganizationExistingCustomer.template.txt");
        ArgumentNullException.ThrowIfNull(textTemplateStream);
        using var textReader = new StreamReader(textTemplateStream);
        var text = await textReader.ReadToEndAsync(cancellationToken);

        html = html
            .Replace("{{ORGANIZATION_NAME}}", joinInvitation.Organization.Name)
            .Replace("{{INVITER_NAME}}", inviterCustomer.DisplayableName)
            .Replace("{{INVITATION_LINK}}", Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "notifications"));

        text = text
            .Replace("{{ORGANIZATION_NAME}}", joinInvitation.Organization.Name)
            .Replace("{{INVITER_NAME}}", inviterCustomer.DisplayableName)
            .Replace("{{INVITATION_LINK}}", Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "notifications"));

        await emailService.SendRawEmailAsync(
            "Organization Invitation - Skedular",
            text,
            html,
            emailConfiguration.InviteToJoinOrganizationNewCustomerEmailSender,
            inviteeCustomer.Identities.Select(item => item.Email).ToEmails(),
            [],
            [],
            [],
            cancellationToken);
    }

    [Activity]
    public async Task SendNewOrganizationJoinedEmailAsync(string? organizationId, string? organizationUniqueAlphanumericName)
    {
        if (!emailConfiguration.EnableNewOrganizationJoinedEmail)
        {
            return;
        }

        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
            organizationId,
            organizationUniqueAlphanumericName,
            cancellationToken);
        if (organization is null)
        {
            return;
        }

        await using var htmlTemplateStream =
            typeof(EmailIntegrations).Assembly.GetManifestResourceStream("Organization.Shared.EmailTemplates.NewOrganizationJoined.template.html");
        ArgumentNullException.ThrowIfNull(htmlTemplateStream);
        using var htmlReader = new StreamReader(htmlTemplateStream);
        var html = await htmlReader.ReadToEndAsync(cancellationToken);

        await using var textTemplateStream =
            typeof(EmailIntegrations).Assembly.GetManifestResourceStream("Organization.Shared.EmailTemplates.NewOrganizationJoined.template.txt");
        ArgumentNullException.ThrowIfNull(textTemplateStream);
        using var textReader = new StreamReader(textTemplateStream);
        var text = await textReader.ReadToEndAsync(cancellationToken);

        html = html
            .Replace("{{ORGANIZATION_ID}}", organization.Id)
            .Replace("{{ORGANIZATION_NAME}}", organization.Name);

        text = text
            .Replace("{{ORGANIZATION_ID}}", organization.Id)
            .Replace("{{ORGANIZATION_NAME}}", organization.Name);

        await emailService.SendRawEmailAsync(
            "New organization has joined Skedular",
            text,
            html,
            emailConfiguration.NewOrganizationJoinedEmailSender,
            emailConfiguration.NewOrganizationJoinedEmailReceivers,
            [],
            [],
            [],
            cancellationToken);
    }
}
