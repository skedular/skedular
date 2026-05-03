using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Context;
using Enterprise.Shared.Random;
using CustomerEntity = Booking.Shared.Database.Entities.Customer;
using IPrivateRecurringBookingService = Booking.Shared.Services.IPrivateRecurringBookingService;
using Organization = Booking.Shared.Database.Entities.Organization;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Api.UnitTests.Services.PrivateRecurringBookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_No_Involved_Customers_Are_Provided(
        PrivateRecurringBookingService sut,
        CancellationToken cancellationToken) =>
        await Should.ThrowAsync<ArgumentException>(() =>
            sut.AddAsync(new RecurringBooking(), cancellationToken));

    [Theory]
    [AutoFakeItEasyData]
    public async Task Generate_An_Id_And_Call_The_Shared_Service_For_A_New_Recurring_Booking(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] IContext context,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ITeamAuthorizationService teamAuthorizationService,
        [Frozen] IPrivateRecurringBookingService sharedPrivateRecurringBookingService,
        PrivateRecurringBookingService sut,
        string verifiableToken,
        string generatedRecurringBookingId,
        CancellationToken cancellationToken)
    {
        var recurringBooking = new RecurringBooking { InvolvedCustomers = [new Customer { Id = "customer-1" }] };
        var customer = new CustomerEntity { Id = "customer-1" };
        var expected = new RecurringBooking { Id = generatedRecurringBookingId };
        IReadOnlyList<string> emptyIds = [];
        IReadOnlyList<Organization> emptyOrganizations = [];
        IReadOnlyList<Team> emptyTeams = [];

        A.CallTo(() => context.GetVerifiableToken()).Returns(verifiableToken);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => customerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken)).Returns(customer);
        A.CallTo(() => randomHelper.Generate()).Returns(generatedRecurringBookingId);
        A.CallTo(() => organizationAuthorizationService.GetOrganizationsAndValidatePermissionsAsync(emptyIds, emptyIds, customer.Id, false,
                cancellationToken))
            .Returns(emptyOrganizations);
        A.CallTo(() => teamAuthorizationService.GetBookingInvolvedTeamAndValidatePermissionsAsync(emptyIds, customer.Id, false, cancellationToken))
            .Returns(emptyTeams);
        A.CallTo(() => sharedPrivateRecurringBookingService.AddAsync(
                A<RecurringBooking>.That.Matches(model => model.Id == generatedRecurringBookingId),
                A<CustomerEntity>.That.Matches(model => model.Id == customer.Id),
                A<IReadOnlyList<Organization>>.That.Matches(items => items.Count == 0),
                A<IReadOnlyList<Team>>.That.Matches(items => items.Count == 0),
                cancellationToken))
            .Returns(expected);

        var result = await sut.AddAsync(recurringBooking, cancellationToken);

        result.ShouldBe(expected);
    }
}
