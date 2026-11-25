using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Organization = Booking.Shared.Database.Entities.Organization;

namespace Booking.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    ValueTask<bool> CanViewOrganizationDetailsAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanViewBookingsAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanViewOtherCustomersBookingsAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanViewOtherCustomersBookingsAsync(Organization organization, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanAddBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanAddBookingAsync(Organization organization, string customerId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, bool>> CanAddBookingByOrganizationIdsAsync(
        IReadOnlyCollection<string> organizationIds,
        string customerId,
        CancellationToken cancellationToken);

    ValueTask<bool> CanUpdateBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanUpdateBookingAsync(Organization organization, string customerId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, bool>> CanUpdateBookingByOrganizationIdsAsync(
        IReadOnlyCollection<string> organizationIds,
        string customerId,
        CancellationToken cancellationToken);

    ValueTask<bool> CanDeleteBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanOverrideCancellationPolicyAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanOverrideCancellationPolicyAsync(Organization organization, string customerId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, bool>> CanOverrideCancellationPolicyByOrganizationIdsAsync(
        IReadOnlyCollection<string> organizationIds,
        string customerId,
        CancellationToken cancellationToken);

    ValueTask<bool> CanModifyPaymentMethodAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanModifyPaymentMethodAsync(Organization organization, string customerId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, bool>> CanModifyPaymentMethodByOrganizationIdsAsync(
        IReadOnlyCollection<string> organizationIds,
        string customerId,
        CancellationToken cancellationToken);

    ValueTask<OrganizationPermissions> GetPermissionsAsync(
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Organization>> GetOrganizationsAndValidatePermissionsAsync(
        IReadOnlyList<string> ids,
        IReadOnlyList<string> customDomains,
        string customerId,
        bool existing,
        CancellationToken cancellationToken);
}

public class OrganizationAuthorizationService(
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    ICachedOrganizationService cachedOrganizationService,
    IOrganizationSsoAuthorizationService organizationSsoAuthorizationService,
    IOrganizationOfferingService organizationOfferingService)
    : IOrganizationAuthorizationService
{
    public async ValueTask<bool> CanViewOrganizationDetailsAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member,
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanViewBookingsAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        if (organization.Type != OrganizationTypeConstants.Private)
        {
            return await organizationSsoAuthorizationService.IsSsoValidAsync(organization, customerId, cancellationToken);
        }

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member,
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanViewOtherCustomersBookingsAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return await CanViewOtherCustomersBookingsAsync(organization, customerId, cancellationToken);
    }

    public ValueTask<bool> CanViewOtherCustomersBookingsAsync(
        Organization organization,
        string customerId,
        CancellationToken cancellationToken)
    {
        var member = organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId);
        if (member is null || member.Status != OrganizationMemberStatusConstants.Active)
        {
            return ValueTask.FromResult(false);
        }

        return ValueTask.FromResult(organization.Type switch
        {
            OrganizationTypeConstants.Private => member.Role is OrganizationMemberRoleConstants.Owner or
                OrganizationMemberRoleConstants.Administrator or
                OrganizationMemberRoleConstants.Member,
            OrganizationTypeConstants.Marketplace or OrganizationTypeConstants.Host => member.Role is OrganizationMemberRoleConstants.Owner or
                OrganizationMemberRoleConstants.Administrator,
            _ => false,
        });
    }

    public async ValueTask<bool> CanAddBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return await CanAddBookingAsync(organization, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanAddBookingAsync(
        Organization organization,
        string customerId,
        CancellationToken cancellationToken) =>
        await CanAddOrUpdateBookingAsync(organization, customerId, cancellationToken);

    public async Task<IReadOnlyDictionary<string, bool>> CanAddBookingByOrganizationIdsAsync(
        IReadOnlyCollection<string> organizationIds,
        string customerId,
        CancellationToken cancellationToken)
    {
        var organizations = await GetOrganizationsByIdsAsync(organizationIds, customerId, cancellationToken);
        var result = new Dictionary<string, bool>(organizations.Count);
        foreach (var organization in organizations)
        {
            result[organization.Id] = await CanAddBookingAsync(organization, customerId, cancellationToken);
        }

        return result;
    }

    public async ValueTask<bool> CanUpdateBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return await CanUpdateBookingAsync(organization, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanUpdateBookingAsync(
        Organization organization,
        string customerId,
        CancellationToken cancellationToken) =>
        await CanAddOrUpdateBookingAsync(organization, customerId, cancellationToken);

    public async Task<IReadOnlyDictionary<string, bool>> CanUpdateBookingByOrganizationIdsAsync(
        IReadOnlyCollection<string> organizationIds,
        string customerId,
        CancellationToken cancellationToken)
    {
        var organizations = await GetOrganizationsByIdsAsync(organizationIds, customerId, cancellationToken);
        var result = new Dictionary<string, bool>(organizations.Count);
        foreach (var organization in organizations)
        {
            result[organization.Id] = await CanUpdateBookingAsync(organization, customerId, cancellationToken);
        }

        return result;
    }

    public async ValueTask<bool> CanDeleteBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member,
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanOverrideCancellationPolicyAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return await CanOverrideCancellationPolicyAsync(organization, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanOverrideCancellationPolicyAsync(
        Organization organization,
        string customerId,
        CancellationToken cancellationToken) =>
        await CanManageOrganizationBookingAsync(organization, customerId, cancellationToken);

    public async Task<IReadOnlyDictionary<string, bool>> CanOverrideCancellationPolicyByOrganizationIdsAsync(
        IReadOnlyCollection<string> organizationIds,
        string customerId,
        CancellationToken cancellationToken)
    {
        var organizations = await GetOrganizationsByIdsAsync(organizationIds, customerId, cancellationToken);
        var result = new Dictionary<string, bool>(organizations.Count);
        foreach (var organization in organizations)
        {
            result[organization.Id] = await CanOverrideCancellationPolicyAsync(organization, customerId, cancellationToken);
        }

        return result;
    }

    public async ValueTask<bool> CanModifyPaymentMethodAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return await CanModifyPaymentMethodAsync(organization, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanModifyPaymentMethodAsync(
        Organization organization,
        string customerId,
        CancellationToken cancellationToken) =>
        await CanManageOrganizationBookingAsync(organization, customerId, cancellationToken);

    public async Task<IReadOnlyDictionary<string, bool>> CanModifyPaymentMethodByOrganizationIdsAsync(
        IReadOnlyCollection<string> organizationIds,
        string customerId,
        CancellationToken cancellationToken)
    {
        var organizations = await GetOrganizationsByIdsAsync(organizationIds, customerId, cancellationToken);
        var result = new Dictionary<string, bool>(organizations.Count);
        foreach (var organization in organizations)
        {
            result[organization.Id] = await CanModifyPaymentMethodAsync(organization, customerId, cancellationToken);
        }

        return result;
    }

    public async ValueTask<OrganizationPermissions> GetPermissionsAsync(
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(
            organizationId,
            organizationCustomDomain,
            cancellationToken) ?? throw new OrganizationNotFound();

        return new OrganizationPermissions
        {
            CanViewBookings = await CanViewBookingsAsync(organization.Id, customerId, cancellationToken),
            CanAddBooking = await CanAddBookingAsync(organization, customerId, cancellationToken),
            CanUpdateBooking = await CanUpdateBookingAsync(organization, customerId, cancellationToken),
            CanDeleteBooking = await CanDeleteBookingAsync(organization.Id, customerId, cancellationToken),
            CanModifyPaymentMethod = await CanModifyPaymentMethodAsync(organization.Id, customerId, cancellationToken),
        };
    }

    public async Task<IReadOnlyList<Organization>> GetOrganizationsAndValidatePermissionsAsync(
        IReadOnlyList<string> ids,
        IReadOnlyList<string> customDomains,
        string customerId,
        bool existing,
        CancellationToken cancellationToken)
    {
        if (ids.Count == 0 && customDomains.Count == 0)
        {
            return [];
        }

        var result = new List<Organization>();

        if (ids.Count != 0)
        {
            var organizations = await repositoryFactory.OrganizationRepository.GetByIdsOrCustomDomainsAsync(
                ids,
                null,
                false,
                false,
                cancellationToken);
            if (ids.Count + customDomains.Count != organizations.Count)
            {
                throw new OrganizationNotFound();
            }

            foreach (var id in ids)
            {
                var organizationEntity = organizations.First(item => item.Id == id);
                if (existing)
                {
                    if (!await CanUpdateBookingAsync(organizationEntity, customerId, cancellationToken))
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                else
                {
                    if (!await CanAddBookingAsync(organizationEntity, customerId, cancellationToken))
                    {
                        throw new UnauthorizedAccessException();
                    }

                    if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(organizationEntity.Id, customerId, cancellationToken))
                    {
                        throw new NoMoreInteractionAllowed();
                    }
                }

                result.Add(organizationEntity);
            }
        }
        else if (customDomains.Count != 0)
        {
            var organizations = await repositoryFactory.OrganizationRepository.GetByIdsOrCustomDomainsAsync(
                null,
                customDomains,
                false,
                false,
                cancellationToken);
            if (ids.Count + customDomains.Count != organizations.Count)
            {
                throw new OrganizationNotFound();
            }

            foreach (var customDomain in customDomains)
            {
                var organizationEntity = organizations.First(item => item.CustomDomain == customDomain);
                if (existing)
                {
                    if (!await CanUpdateBookingAsync(organizationEntity, customerId, cancellationToken))
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                else
                {
                    if (!await CanAddBookingAsync(organizationEntity, customerId, cancellationToken))
                    {
                        throw new UnauthorizedAccessException();
                    }

                    if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(organizationEntity.Id, customerId, cancellationToken))
                    {
                        throw new NoMoreInteractionAllowed();
                    }
                }

                result.Add(organizationEntity);
            }
        }

        return result;
    }

    private async Task<IReadOnlyList<Organization>> GetOrganizationsByIdsAsync(
        IReadOnlyCollection<string> organizationIds,
        string customerId,
        CancellationToken cancellationToken)
    {
        var distinctIds = organizationIds.Where(item => !string.IsNullOrWhiteSpace(item)).Distinct().ToList();
        if (distinctIds.Count == 0)
        {
            return [];
        }

        var organizations = await repositoryFactory.OrganizationRepository.GetByIdsForAuthorizationAsync(
            distinctIds,
            customerId,
            cancellationToken);
        if (organizations.Count != distinctIds.Count)
        {
            throw new OrganizationNotFound();
        }

        return organizations;
    }

    private async ValueTask<bool> CanManageOrganizationBookingAsync(
        Organization organization,
        string customerId,
        CancellationToken cancellationToken) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator,
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization, customerId, cancellationToken);

    private async ValueTask<bool> CanAddOrUpdateBookingAsync(
        Organization organization,
        string customerId,
        CancellationToken cancellationToken)
    {
        if (organization.Type != OrganizationTypeConstants.Private)
        {
            return await organizationSsoAuthorizationService.IsSsoValidAsync(organization, customerId, cancellationToken);
        }

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member,
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization, customerId, cancellationToken);
    }
}
