using Customer.Api.Services;
using Customer.Shared.Mappers;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Enterprise.Shared;

namespace Customer.Api.UnitTests.Services.WorkaroundServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RepublishAllCustomersAsyncTests
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Call_GetAllUntrackedAsync(
        [Frozen] IRepositoryFactory repositoryFactory,
        WorkaroundService sut,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => customerRepository.GetAllUntrackedAsync(cancellationToken)).Returns([]);

        await sut.RepublishAllCustomersAsync(cancellationToken);

        A.CallTo(() => customerRepository.GetAllUntrackedAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Map_Customer_Entities_To_Models(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IEntityMapper entityMapper,
        WorkaroundService sut,
        ICustomerRepository customerRepository,
        IReadOnlyList<Shared.Database.Entities.Customer> customerEntities,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => customerRepository.GetAllUntrackedAsync(cancellationToken)).Returns(customerEntities);

        await sut.RepublishAllCustomersAsync(cancellationToken);

        Enumerable.Range(0, customerEntities.Count).ForEach(idx =>
            A.CallTo(() => entityMapper.MapTo(customerEntities.Skip(idx).First())).MustHaveHappenedOnceExactly());
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Publish_Customers(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICustomerPublisher customerPublisher,
        [Frozen] IEntityMapper entityMapper,
        WorkaroundService sut,
        ICustomerRepository customerRepository,
        IReadOnlyList<Shared.Database.Entities.Customer> customerEntities,
        IReadOnlyList<Shared.Models.Customer> customers,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => customerRepository.GetAllUntrackedAsync(cancellationToken)).Returns(customerEntities);

        Enumerable.Range(0, customerEntities.Count).ForEach(idx =>
            A.CallTo(() => entityMapper.MapTo(customerEntities.Skip(idx).First())).Returns(customers.Skip(idx).First()));

        await sut.RepublishAllCustomersAsync(cancellationToken);

        A.CallTo(() => customerPublisher.PublishCustomersAsync(
                A<IReadOnlyList<Shared.Models.Customer>>.That.Matches(items =>
                    items.Count == customerEntities.Count && customers.Any(items.Contains)),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
