using Booking.Api.Services.Authorization;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Permissions;

[QueryType]
public class RootQuery
{
    [UseResolverScope]
    public async Task<OrganizationBookingPermissions> OrganizationBookingPermissionsAsync(
        string? organizationId,
        string? organizationCustomDomain,
        [Service]
        IOrganizationAuthorizationService organizationAuthorizationService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(organizationId) && string.IsNullOrWhiteSpace(organizationCustomDomain))
        {
            return new OrganizationBookingPermissions
            {
                CanAddBooking = false,
                CanUpdateBooking = false,
                CanDeleteBooking = false,
            };
        }

        var permissions = await organizationAuthorizationService.GetPermissionsAsync(
            organizationId,
            organizationCustomDomain,
            cancellationToken);

        return new OrganizationBookingPermissions
        {
            CanViewBookings = permissions.CanViewBookings,
            CanAddBooking = permissions.CanAddBooking,
            CanUpdateBooking = permissions.CanUpdateBooking,
            CanDeleteBooking = permissions.CanDeleteBooking,
            CanModifyPaymentMethod = permissions.CanModifyPaymentMethod,
        };
    }

    [UseResolverScope]
    public async Task<TeamBookingPermissions> TeamBookingPermissionsAsync(
        string teamId,
        [Service]
        ITeamAuthorizationService teamAuthorizationService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(teamId))
        {
            return new TeamBookingPermissions
            {
                CanAddBooking = false,
                CanUpdateBooking = false,
                CanDeleteBooking = false,
            };
        }

        var permissions = await teamAuthorizationService.GetPermissionsAsync(teamId, cancellationToken);
        return new TeamBookingPermissions
        {
            CanAddBooking = permissions.CanAddBooking,
            CanUpdateBooking = permissions.CanUpdateBooking,
            CanDeleteBooking = permissions.CanDeleteBooking,
        };
    }
}
