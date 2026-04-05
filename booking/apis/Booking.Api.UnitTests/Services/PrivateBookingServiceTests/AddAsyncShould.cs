using AutoFixture.Xunit3;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Context;
using Enterprise.Shared.Random;
using FakeItEasy;
using CustomerEntity = Booking.Shared.Database.Entities.Customer;
using IPrivateBookingService = Booking.Shared.Services.IPrivateBookingService;
using Organization = Booking.Shared.Database.Entities.Organization;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Api.UnitTests.Services.PrivateBookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_No_Involved_Customers_Are_Provided(
        PrivateBookingService sut,
        CancellationToken cancellationToken) =>
        await Should.ThrowAsync<ArgumentException>(() =>
            sut.AddAsync(new Shared.Models.Booking(), cancellationToken));

    [Theory]
    [AutoFakeItEasyData]
    public async Task Generate_An_Id_And_Call_The_Shared_Service_For_A_New_Booking(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] IContext context,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ITeamAuthorizationService teamAuthorizationService,
        [Frozen] IPrivateBookingService sharedPrivateBookingService,
        PrivateBookingService sut,
        string verifiableToken,
        string generatedBookingId,
        CancellationToken cancellationToken)
    {
        var booking = new Shared.Models.Booking { InvolvedCustomers = [new Customer { Id = "customer-1" }] };
        var customer = new CustomerEntity { Id = "customer-1" };
        var expected = new Shared.Models.Booking { Id = generatedBookingId };
        ICollection<string> emptyIds = [];
        ICollection<Organization> emptyOrganizations = [];
        ICollection<Team> emptyTeams = [];

        A.CallTo(() => context.GetVerifiableToken()).Returns(verifiableToken);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => customerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken)).Returns(customer);
        A.CallTo(() => randomHelper.Generate()).Returns(generatedBookingId);
        A.CallTo(() => organizationAuthorizationService.GetOrganizationsAndValidatePermissionsAsync(emptyIds, emptyIds, customer.Id, false,
                cancellationToken))
            .Returns(emptyOrganizations);
        A.CallTo(() => teamAuthorizationService.GetBookingInvolvedTeamAndValidatePermissionsAsync(emptyIds, customer.Id, false, cancellationToken))
            .Returns(emptyTeams);
        A.CallTo(() => sharedPrivateBookingService.AddAsync(
                A<Shared.Models.Booking>.That.Matches(model => model.Id == generatedBookingId),
                A<CustomerEntity>.That.Matches(model => model.Id == customer.Id),
                A<ICollection<Organization>>.That.Matches(items => items.Count == 0),
                A<ICollection<Team>>.That.Matches(items => items.Count == 0),
                null,
                cancellationToken))
            .Returns(expected);

        var result = await sut.AddAsync(booking, cancellationToken);

        result.ShouldBe(expected);
    }
}
