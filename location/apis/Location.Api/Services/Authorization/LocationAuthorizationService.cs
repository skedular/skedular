using Api.Shared.Models;
using Enterprise.Shared.Exceptions;
using Location.Shared.Models;
using Location.Shared.Repositories;

namespace Location.Api.Services.Authorization;

public interface ILocationAuthorizationService
{
    bool CanView(Shared.Database.Entities.Location location, Customer customer);
    bool CanModify(Shared.Database.Entities.Location location, Customer customer);
    bool CanDelete(Shared.Database.Entities.Location location, Customer customer);
    bool CanInvitePeople(Shared.Database.Entities.Location location, Customer customer);
    bool CanCancelPeopleExistingInvitations(Shared.Database.Entities.Location location, Customer customer);
    bool CanViewAnalytics(Shared.Database.Entities.Location location, Customer customer);
    Task<Permissions> GetPermissionsAsync(string locationId, CancellationToken cancellationToken);
}

public class LocationAuthorizationService(
    ICachedCustomerService cachedCustomerService,
    IRepositoryFactory repositoryFactory,
    IOrganizationAuthorizationService organizationAuthorizationService)
    : ILocationAuthorizationService
{
    public bool CanView(Shared.Database.Entities.Location location, Customer customer)
    {
        if (location.Organization is null)
        {
            return location.LocationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.MembershipType is
                LocationMembershipType.Owner
                or LocationMembershipType.Administrator or LocationMembershipType.Member;
        }

        return organizationAuthorizationService.CanView(location.Organization, customer);
    }

    public bool CanModify(Shared.Database.Entities.Location location, Customer customer)
    {
        if (location.Organization is null)
        {
            return location.LocationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.MembershipType is
                LocationMembershipType.Owner
                or LocationMembershipType.Administrator;
        }

        return organizationAuthorizationService.CanModify(location.Organization, customer);
    }

    public bool CanDelete(Shared.Database.Entities.Location location, Customer customer)
    {
        if (location.Organization is null)
        {
            return location.LocationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.MembershipType is
                LocationMembershipType.Owner;
        }

        return organizationAuthorizationService.CanDelete(location.Organization, customer);
    }

    public bool CanInvitePeople(Shared.Database.Entities.Location location, Customer customer)
    {
        if (location.Organization is null)
        {
            return location.LocationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.MembershipType is
                LocationMembershipType.Owner
                or LocationMembershipType.Administrator;
        }

        return organizationAuthorizationService.CanInvitePeople(location.Organization, customer);
    }

    public bool CanCancelPeopleExistingInvitations(
        Shared.Database.Entities.Location location,
        Customer customer)
    {
        if (location.Organization is null)
        {
            return location.LocationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.MembershipType is
                LocationMembershipType.Owner
                or LocationMembershipType.Administrator;
        }

        return organizationAuthorizationService.CanCancelPeopleExistingInvitations(location.Organization, customer);
    }

    public bool CanViewAnalytics(Shared.Database.Entities.Location location, Customer customer)
    {
        if (location.Organization is null)
        {
            return location.LocationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.MembershipType is
                LocationMembershipType.Owner
                or LocationMembershipType.Administrator;
        }

        return organizationAuthorizationService.CanViewAnalytics(location.Organization, customer);
    }

    public async Task<Permissions> GetPermissionsAsync(string locationId, CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(
            locationId,
            cancellationToken);
        if (location is null)
        {
            throw new OrganizationNotFound();
        }

        return new Permissions
        {
            CanView = CanView(location, customer),
            CanModify = CanModify(location, customer),
            CanDelete = CanDelete(location, customer),
            CanInvitePeople = CanInvitePeople(location, customer),
            CanCancelPeopleExistingInvitations = CanCancelPeopleExistingInvitations(location, customer),
            CanViewAnalytics = CanViewAnalytics(location, customer)
        };
    }
}
