using Customer.Api.Services;
using Customer.Shared.Mappers;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;

namespace Customer.Api.UnitTests.Services.WorkaroundServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RepublishCustomerAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Call_GetByIdAsync(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        WorkaroundService sut,
        ICustomerRepository customerRepository,
        string customerId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);

        await sut.RepublishCustomerAsync(customerId, cancellationToken);

        A.CallTo(() => customerRepository.GetByIdAsync(customerId, cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Not_Publish_Customer_If_Customer_Not_Found(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ICustomerPublisher customerPublisher,
        [Frozen]
        IEntityMapper entityMapper,
        WorkaroundService sut,
        ICustomerRepository customerRepository,
        string customerId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => customerRepository.GetByIdAsync(customerId, cancellationToken)).Returns<Shared.Database.Entities.Customer?>(null);

        await sut.RepublishCustomerAsync(customerId, cancellationToken);

        A.CallTo(() => entityMapper.MapTo(A<Shared.Database.Entities.Customer>._)).MustNotHaveHappened();
        A.CallTo(() => customerPublisher.PublishCustomersAsync(A<IReadOnlyList<Shared.Models.Customer>>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Map_Customer_Entity_To_Model(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntityMapper entityMapper,
        WorkaroundService sut,
        ICustomerRepository customerRepository,
        string customerId,
        Shared.Database.Entities.Customer customerEntity,
        Shared.Models.Customer customer,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => customerRepository.GetByIdAsync(customerId, cancellationToken)).Returns(customerEntity);
        A.CallTo(() => entityMapper.MapTo(customerEntity)).Returns(customer);

        await sut.RepublishCustomerAsync(customerId, cancellationToken);

        A.CallTo(() => entityMapper.MapTo(customerEntity)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Publish_Customer_If_Customer_Found(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ICustomerPublisher customerPublisher,
        [Frozen]
        IEntityMapper entityMapper,
        WorkaroundService sut,
        ICustomerRepository customerRepository,
        string customerId,
        Shared.Database.Entities.Customer customerEntity,
        Shared.Models.Customer customer,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => customerRepository.GetByIdAsync(customerId, cancellationToken)).Returns(customerEntity);
        A.CallTo(() => entityMapper.MapTo(customerEntity)).Returns(customer);

        await sut.RepublishCustomerAsync(customerId, cancellationToken);

        A.CallTo(() => customerPublisher.PublishCustomersAsync(
                A<IReadOnlyList<Shared.Models.Customer>>.That.Matches(items => items.Count == 1 && items.Single() == customer),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
