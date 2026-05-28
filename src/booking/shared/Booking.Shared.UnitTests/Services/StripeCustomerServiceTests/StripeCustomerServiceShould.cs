using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Random;
using Stripe;
using Customer = Booking.Shared.Database.Entities.Customer;

namespace Booking.Shared.UnitTests.Services.StripeCustomerServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StripeCustomerServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Existing_Stripe_Customer_For_Organization_When_Already_Exists(
        [Frozen] IRepositoryFactory repositoryFactory,
        StripeCustomerService sut,
        IStripeCustomerRepository stripeCustomerRepository,
        string stripeAccountId,
        CancellationToken cancellationToken)
    {
        var existingCustomer = new StripeCustomer { Id = "stripe-cust-1" };
        var organization = new Organization();

        A.CallTo(() => repositoryFactory.StripeCustomerRepository).Returns(stripeCustomerRepository);
        A.CallTo(() => stripeCustomerRepository.GetByOrganizationIdAsync(stripeAccountId, organization.Id, cancellationToken))
            .Returns(existingCustomer);

        var result = await sut.AddCustomerAsync(organization, stripeAccountId, cancellationToken);

        result.ShouldBe(existingCustomer);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Create_New_Stripe_Customer_For_Organization_When_Not_Exists(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] ICreatable<Stripe.Customer, CustomerCreateOptions> customerCreateService,
        StripeCustomerService sut,
        IStripeCustomerRepository stripeCustomerRepository,
        string stripeAccountId,
        string generatedId,
        CancellationToken cancellationToken)
    {
        var stripeCustomer = new Stripe.Customer { Id = "cus_123" };
        var customerCreateOptions = new CustomerCreateOptions();
        var organization = new Organization();
        StripeCustomer? addedStripeCustomer = null;

        A.CallTo(() => repositoryFactory.StripeCustomerRepository).Returns(stripeCustomerRepository);
        A.CallTo(() => stripeCustomerRepository.GetByOrganizationIdAsync(stripeAccountId, organization.Id, cancellationToken))
            .Returns(Task.FromResult<StripeCustomer?>(null));
        A.CallTo(() => entityMapper.MapToCustomerCreateOption(organization)).Returns(customerCreateOptions);
        A.CallTo(() => customerCreateService.CreateAsync(customerCreateOptions, A<RequestOptions>._, cancellationToken))
            .Returns(stripeCustomer);
        A.CallTo(() => randomHelper.Generate()).Returns(generatedId);
        A.CallTo(() => stripeCustomerRepository.Add(A<StripeCustomer>._))
            .Invokes((StripeCustomer stripeCustomerEntity) => addedStripeCustomer = stripeCustomerEntity)
            .ReturnsLazily(call => call.GetArgument<StripeCustomer>(0)!);

        var result = await sut.AddCustomerAsync(organization, stripeAccountId, cancellationToken);

        addedStripeCustomer.ShouldNotBeNull();
        addedStripeCustomer.Id.ShouldBe(generatedId);
        result.Id.ShouldBe(generatedId);
        result.StripeCustomerId.ShouldBe(stripeCustomer.Id);
        result.StripeAccountId.ShouldBe(stripeAccountId);
        result.Organization.ShouldBe(organization);
        A.CallTo(() => stripeCustomerRepository.Add(A<StripeCustomer>.That.Matches(c =>
            c.Id == generatedId &&
            c.StripeCustomerId == stripeCustomer.Id &&
            c.StripeAccountId == stripeAccountId &&
            c.Organization == organization))).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Existing_Stripe_Customer_For_Customer_When_Already_Exists(
        [Frozen] IRepositoryFactory repositoryFactory,
        StripeCustomerService sut,
        IStripeCustomerRepository stripeCustomerRepository,
        string stripeAccountId,
        CancellationToken cancellationToken)
    {
        var existingCustomer = new StripeCustomer { Id = "stripe-cust-1" };
        var customer = new Customer();

        A.CallTo(() => repositoryFactory.StripeCustomerRepository).Returns(stripeCustomerRepository);
        A.CallTo(() => stripeCustomerRepository.GetByCustomerIdAsync(stripeAccountId, customer.Id, cancellationToken))
            .Returns(existingCustomer);

        var result = await sut.AddCustomerAsync(customer, stripeAccountId, cancellationToken);

        result.ShouldBe(existingCustomer);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Create_New_Stripe_Customer_For_Customer_When_Not_Exists(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] ICreatable<Stripe.Customer, CustomerCreateOptions> customerCreateService,
        StripeCustomerService sut,
        IStripeCustomerRepository stripeCustomerRepository,
        string stripeAccountId,
        string generatedId,
        CancellationToken cancellationToken)
    {
        var stripeCustomer = new Stripe.Customer { Id = "cus_123" };
        var customerCreateOptions = new CustomerCreateOptions();
        var customer = new Customer();
        StripeCustomer? addedStripeCustomer = null;

        A.CallTo(() => repositoryFactory.StripeCustomerRepository).Returns(stripeCustomerRepository);
        A.CallTo(() => stripeCustomerRepository.GetByCustomerIdAsync(stripeAccountId, customer.Id, cancellationToken))
            .Returns(Task.FromResult<StripeCustomer?>(null));
        A.CallTo(() => entityMapper.MapToCustomerCreateOption(customer)).Returns(customerCreateOptions);
        A.CallTo(() => customerCreateService.CreateAsync(customerCreateOptions, A<RequestOptions>._, cancellationToken))
            .Returns(stripeCustomer);
        A.CallTo(() => randomHelper.Generate()).Returns(generatedId);
        A.CallTo(() => stripeCustomerRepository.Add(A<StripeCustomer>._))
            .Invokes((StripeCustomer stripeCustomerEntity) => addedStripeCustomer = stripeCustomerEntity)
            .ReturnsLazily(call => call.GetArgument<StripeCustomer>(0)!);

        var result = await sut.AddCustomerAsync(customer, stripeAccountId, cancellationToken);

        addedStripeCustomer.ShouldNotBeNull();
        addedStripeCustomer.Id.ShouldBe(generatedId);
        result.Id.ShouldBe(generatedId);
        result.StripeCustomerId.ShouldBe(stripeCustomer.Id);
        result.StripeAccountId.ShouldBe(stripeAccountId);
        result.Customer.ShouldBe(customer);
        A.CallTo(() => stripeCustomerRepository.Add(A<StripeCustomer>.That.Matches(c =>
            c.Id == generatedId &&
            c.StripeCustomerId == stripeCustomer.Id &&
            c.StripeAccountId == stripeAccountId &&
            c.Customer == customer))).MustHaveHappenedOnceExactly();
    }
}
