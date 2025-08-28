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

public record SendInviteCustomerToJoinOrganizationNewCustomerInput(string OrganizationId, string InviterCustomerId, string InviteeCustomerEmail);

public record SendInviteCustomerToJoinOrganizationExistingCustomerInput(string OrganizationId, string InviterCustomerId, string InviteeCustomerId);

public class EmailIntegrations(
    ApplicationConfiguration applicationConfiguration,
    EmailConfiguration emailConfiguration,
    CustomerConfiguration customerConfiguration,
    IRepositoryFactory repositoryFactory,
    IEmailService emailService,
    CustomerService.CustomerServiceClient customerServiceClient)
{
    [Activity]
    public async Task SendInviteCustomerToJoinOrganizationNewCustomerAsync(SendInviteCustomerToJoinOrganizationNewCustomerInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(args.OrganizationId, null, cancellationToken);
        if (organization is null)
        {
            return;
        }

        var inviterCustomer = await customerServiceClient.Admin_GetAsync(
            new Admin_GetInput { CustomerId = args.InviterCustomerId },
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
            .Replace("{{ORGANIZATION_NAME}}", organization.Name)
            .Replace("{{INVITER_NAME}}", inviterCustomer.DisplayableName)
            .Replace("{{INVITATION_LINK}}", Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "signup"));

        text = text
            .Replace("{{ORGANIZATION_NAME}}", organization.Name)
            .Replace("{{INVITER_NAME}}", inviterCustomer.DisplayableName)
            .Replace("{{INVITATION_LINK}}", Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "signup"));

        await emailService.SendRawEmailAsync(
            "Organization Invitation - Skedular",
            text,
            html,
            $"Skedular {emailConfiguration.InviteToJoinOrganizationNewCustomerEmailSender}",
            [args.InviteeCustomerEmail],
            [],
            [],
            [],
            cancellationToken);
    }

    [Activity]
    public async Task SendInviteCustomerToJoinOrganizationExistingCustomerAsync(SendInviteCustomerToJoinOrganizationExistingCustomerInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
            args.OrganizationId,
            null,
            cancellationToken);
        if (organization is null)
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
            .Replace("{{ORGANIZATION_NAME}}", organization.Name)
            .Replace("{{INVITER_NAME}}", inviterCustomer.DisplayableName)
            .Replace("{{INVITATION_LINK}}", Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "notifications"));

        text = text
            .Replace("{{ORGANIZATION_NAME}}", organization.Name)
            .Replace("{{INVITER_NAME}}", inviterCustomer.DisplayableName)
            .Replace("{{INVITATION_LINK}}", Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "notifications"));

        await emailService.SendRawEmailAsync(
            "Organization Invitation - Skedular",
            text,
            html,
            $"Skedular {emailConfiguration.InviteToJoinOrganizationNewCustomerEmailSender}",
            inviteeCustomer.Identities.Select(item => item.Email).ToEmails(),
            [],
            [],
            [],
            cancellationToken);
    }
}
