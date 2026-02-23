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
    ValueTask<bool> CanAddBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanUpdateBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanDeleteBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanModifyPaymentMethodAsync(string organizationId, string customerId, CancellationToken cancellationToken);

    ValueTask<OrganizationPermissions> GetPermissionsAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken);

    Task<ICollection<Organization>> GetOrganizationsAndValidatePermissionsAsync(
        ICollection<string> ids,
        ICollection<string> uniqueAlphanumericNames,
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
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanViewBookingsAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanAddBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanUpdateBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanDeleteBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanModifyPaymentMethodAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }

    public async ValueTask<OrganizationPermissions> GetPermissionsAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(
            organizationId,
            organizationUniqueAlphanumericName,
            cancellationToken) ?? throw new OrganizationNotFound();

        return new OrganizationPermissions
        {
            CanViewBookings = await CanViewBookingsAsync(organization.Id, customer.Id, cancellationToken),
            CanAddBooking = await CanAddBookingAsync(organization.Id, customer.Id, cancellationToken),
            CanUpdateBooking = await CanUpdateBookingAsync(organization.Id, customer.Id, cancellationToken),
            CanDeleteBooking = await CanDeleteBookingAsync(organization.Id, customer.Id, cancellationToken),
            CanModifyPaymentMethod = await CanModifyPaymentMethodAsync(organization.Id, customer.Id, cancellationToken)
        };
    }

    public async Task<ICollection<Organization>> GetOrganizationsAndValidatePermissionsAsync(
        ICollection<string> ids,
        ICollection<string> uniqueAlphanumericNames,
        string customerId,
        bool existing,
        CancellationToken cancellationToken)
    {
        if (ids.Count == 0 && uniqueAlphanumericNames.Count == 0)
        {
            return [];
        }

        var result = new List<Organization>();

        if (ids.Count != 0)
        {
            var organizations = await repositoryFactory.OrganizationRepository.GetByIdsOrUniqueAlphanumericNamesAsync(
                ids,
                null,
                false,
                false,
                cancellationToken);
            if (ids.Count + uniqueAlphanumericNames.Count != organizations.Count)
            {
                throw new OrganizationNotFound();
            }

            foreach (var id in ids)
            {
                var organizationEntity = organizations.First(item => item.Id == id);
                if (existing)
                {
                    if (!await CanUpdateBookingAsync(organizationEntity.Id, customerId, cancellationToken))
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                else
                {
                    if (!await CanAddBookingAsync(organizationEntity.Id, customerId, cancellationToken))
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
        else if (uniqueAlphanumericNames.Count != 0)
        {
            var organizations = await repositoryFactory.OrganizationRepository.GetByIdsOrUniqueAlphanumericNamesAsync(
                null,
                uniqueAlphanumericNames,
                false,
                false,
                cancellationToken);
            if (ids.Count + uniqueAlphanumericNames.Count != organizations.Count)
            {
                throw new OrganizationNotFound();
            }

            foreach (var uniqueAlphanumericName in uniqueAlphanumericNames)
            {
                var organizationEntity = organizations.First(item => item.UniqueAlphanumericName == uniqueAlphanumericName);
                if (existing)
                {
                    if (!await CanUpdateBookingAsync(organizationEntity.Id, customerId, cancellationToken))
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                else
                {
                    if (!await CanAddBookingAsync(organizationEntity.Id, customerId, cancellationToken))
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
}
