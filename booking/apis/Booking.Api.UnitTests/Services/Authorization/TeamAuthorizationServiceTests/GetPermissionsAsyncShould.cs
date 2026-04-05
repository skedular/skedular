using AutoFixture.Xunit3;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Services.Cache;
using FakeItEasy;

namespace Booking.Api.UnitTests.Services.Authorization.TeamAuthorizationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetPermissionsAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_The_Permissions_Built_From_Organization_Authorization(
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] ICachedTeamService cachedTeamService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        TeamAuthorizationService sut,
        string teamId,
        string customerId,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var team = new Team { Id = teamId, Organization = new Organization { Id = organizationId } };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns(customerId);
        A.CallTo(() => cachedTeamService.GetByIdAsync(teamId, cancellationToken)).Returns(team);
        A.CallTo(() => organizationAuthorizationService.CanViewBookingsAsync(organizationId, customerId, cancellationToken)).Returns(true);
        A.CallTo(() => organizationAuthorizationService.CanAddBookingAsync(organizationId, customerId, cancellationToken)).Returns(false);
        A.CallTo(() => organizationAuthorizationService.CanUpdateBookingAsync(organizationId, customerId, cancellationToken)).Returns(true);
        A.CallTo(() => organizationAuthorizationService.CanDeleteBookingAsync(organizationId, customerId, cancellationToken)).Returns(false);

        var result = await sut.GetPermissionsAsync(teamId, cancellationToken);

        result.CanViewBookings.ShouldBeTrue();
        result.CanAddBooking.ShouldBeFalse();
        result.CanUpdateBooking.ShouldBeTrue();
        result.CanDeleteBooking.ShouldBeFalse();
    }
}
