using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Customer = Booking.Shared.Database.Entities.Customer;
using Identity = Booking.Shared.Database.Entities.Identity;
using Organization = Booking.Shared.Database.Entities.Organization;
using OrganizationMember = Booking.Shared.Database.Entities.OrganizationMember;
using RecurringBooking = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingFailureNotificationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ResolveRecipientsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Resolve_Only_Verified_Customer_And_Active_Stakeholder_Recipients(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IOrganizationRepository organizationRepository,
        MarketplaceBookingFailureNotificationService sut,
        CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Id = "customer-1",
            Identities =
            [
                new Identity { Email = "customer@example.test", EmailVerified = true },
                new Identity { Email = "unverified@example.test", EmailVerified = false }
            ]
        };
        var booking = new Database.Entities.Booking
        {
            Id = "booking-1", CreatedByCustomer = customer, InvolvedOrganizations = [new Organization { Id = "organization-1" }]
        };
        var organization = new Organization
        {
            Id = "organization-1",
            Type = OrganizationTypeConstants.Host,
            OrganizationMembers =
            [
                new OrganizationMember
                {
                    CustomerId = "owner-1",
                    Status = OrganizationMemberStatusConstants.Active,
                    Role = OrganizationMemberRoleConstants.Owner,
                    Customer = new Customer { Id = "owner-1", Identities = [new Identity { Email = "owner@example.test", EmailVerified = true }] }
                },
                new OrganizationMember
                {
                    CustomerId = "former-admin",
                    Status = "INACTIVE",
                    Role = OrganizationMemberRoleConstants.Administrator,
                    Customer = new Customer
                    {
                        Id = "former-admin", Identities = [new Identity { Email = "former@example.test", EmailVerified = true }]
                    }
                }
            ]
        };
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => bookingRepository.GetByIdAsync("booking-1", cancellationToken)).Returns(booking);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("organization-1", null, false, false, cancellationToken))
            .Returns(organization);

        var recipients = await sut.ResolveRecipientsAsync(new MarketplaceBookingFailure { BookingId = "booking-1" }, cancellationToken);

        recipients.Count.ShouldBe(4);
        recipients.ShouldContain(item => item.RecipientEmail == "customer@example.test");
        recipients.ShouldContain(item => item.RecipientEmail == "owner@example.test");
        recipients.ShouldNotContain(item => item.RecipientEmail == "unverified@example.test" || item.RecipientEmail == "former@example.test");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Resolve_Recipients_From_Recurring_Booking_When_No_Concrete_Booking_Exists(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        MarketplaceBookingFailureNotificationService sut,
        CancellationToken cancellationToken)
    {
        var customer = new Customer { Id = "customer-1", Identities = [new Identity { Email = "customer@example.test", EmailVerified = true }] };
        var recurringBooking = new RecurringBooking { Id = "recurring-1", CreatedByCustomer = customer, InvolvedOrganizations = [] };

        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync(recurringBooking.Id, cancellationToken)).Returns(recurringBooking);

        var recipients = await sut.ResolveRecipientsAsync(
            new MarketplaceBookingFailure { RecurringBookingId = recurringBooking.Id },
            cancellationToken);

        recipients.ShouldContain(item =>
            item.RecipientCustomerId == customer.Id &&
            item.Channel == MarketplaceBookingFailureDeliveryChannelConstants.InApplication);
        recipients.ShouldContain(item => item.RecipientEmail == "customer@example.test");
    }
}
