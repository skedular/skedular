using AutoFixture.Xunit3;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Context;
using FakeItEasy;
using CustomerEntity = Booking.Shared.Database.Entities.Customer;

namespace Booking.Api.UnitTests.Services.PrivateRecurringBookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DeleteAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_An_Involved_Team_Cannot_Delete_The_Recurring_Booking(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] ITeamRepository teamRepository,
        [Frozen] IContext context,
        [Frozen] ITeamAuthorizationService teamAuthorizationService,
        PrivateRecurringBookingService sut,
        string recurringBookingId,
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var customer = new CustomerEntity { Id = "customer-1" };
        var recurringBooking = new RecurringBooking { Id = recurringBookingId, InvolvedTeams = [new Team { Id = "team-1" }] };
        var team = new Team { Id = "team-1" };

        A.CallTo(() => context.GetVerifiableToken()).Returns(verifiableToken);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.TeamRepository).Returns(teamRepository);
        A.CallTo(() => customerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken)).Returns(customer);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync(recurringBookingId, cancellationToken)).Returns(recurringBooking);
        A.CallTo(() => teamRepository.GetByIdsAsync(A<ICollection<string>>._, false, cancellationToken)).Returns([team]);
        A.CallTo(() => teamAuthorizationService.CanDeleteBookingAsync(team, customer.Id, cancellationToken)).Returns(false);

        await Should.ThrowAsync<UnauthorizedAccessException>(() => sut.DeleteAsync(recurringBookingId, cancellationToken));
    }
}
