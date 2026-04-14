using Api.Shared.Services;
using Api.Shared.Services.Models;
using Team.Shared.Services.Cache;

namespace Team.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    ValueTask<bool> CanViewAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanModifyAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanDeleteAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanInvitePeopleAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanCancelPeopleExistingInvitationsAsync(string organizationId, string customerId, CancellationToken cancellationToken);
}

public class OrganizationAuthorizationService(
    IOrganizationSsoAuthorizationService organizationSsoAuthorizationService,
    ICachedOrganizationService cachedOrganizationService,
    ILogger<OrganizationAuthorizationService> logger)
    : IOrganizationAuthorizationService
{
    public async ValueTask<bool> CanViewAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        var allowed = organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);

        if (allowed)
        {
            logger.LogInformation(
                "Organisation view permission granted for customer {CustomerId} in organization {OrganizationId}",
                customerId,
                organizationId);
        }
        else
        {
            logger.LogWarning(
                "Organisation view permission denied for customer {CustomerId} in organization {OrganizationId}",
                customerId,
                organizationId);
        }

        return allowed;
    }

    public async ValueTask<bool> CanModifyAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        var allowed = organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);

        if (allowed)
        {
            logger.LogInformation(
                "Organisation modify permission granted for customer {CustomerId} in organization {OrganizationId}",
                customerId,
                organizationId);
        }
        else
        {
            logger.LogWarning(
                "Organisation modify permission denied for customer {CustomerId} in organization {OrganizationId}",
                customerId,
                organizationId);
        }

        return allowed;
    }

    public async ValueTask<bool> CanDeleteAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        var allowed = organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);

        if (allowed)
        {
            logger.LogInformation(
                "Organisation delete permission granted for customer {CustomerId} in organization {OrganizationId}",
                customerId,
                organizationId);
        }
        else
        {
            logger.LogWarning(
                "Organisation delete permission denied for customer {CustomerId} in organization {OrganizationId}",
                customerId,
                organizationId);
        }

        return allowed;
    }

    public async ValueTask<bool> CanInvitePeopleAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        var allowed = organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);

        if (allowed)
        {
            logger.LogInformation(
                "Organisation invite permission granted for customer {CustomerId} in organization {OrganizationId}",
                customerId,
                organizationId);
        }
        else
        {
            logger.LogWarning(
                "Organisation invite permission denied for customer {CustomerId} in organization {OrganizationId}",
                customerId,
                organizationId);
        }

        return allowed;
    }

    public async ValueTask<bool> CanCancelPeopleExistingInvitationsAsync(
        string organizationId,
        string customerId,
        CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        var allowed = organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);

        if (allowed)
        {
            logger.LogInformation(
                "Organisation invitation-cancellation permission granted for customer {CustomerId} in organization {OrganizationId}",
                customerId,
                organizationId);
        }
        else
        {
            logger.LogWarning(
                "Organisation invitation-cancellation permission denied for customer {CustomerId} in organization {OrganizationId}",
                customerId,
                organizationId);
        }

        return allowed;
    }
}
