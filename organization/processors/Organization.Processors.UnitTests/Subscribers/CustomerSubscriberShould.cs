using Api.Shared.Clients.Events.Skedular.Customer.V1;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using Organization.Processors.Mappers;
using Organization.Processors.Subscribers;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using ValueMetadata = Api.Shared.Clients.Events.Skedular.Customer.V1.Metadata;
using ValueType = Api.Shared.Clients.Events.Skedular.Customer.V1.Type;
using CustomerModel = Organization.Shared.Models.Customer;
using CustomerEntity = Organization.Shared.Database.Entities.Customer;

namespace Organization.Processors.UnitTests.Subscribers;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CustomerSubscriberShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Publish_Provisioned_After_Fresh_CustomerUpserted(
        [Frozen] IEventMapper eventMapper,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] ICustomerReadinessPublisher customerReadinessPublisher,
        [Frozen] EventContext eventContext,
        CustomerSubscriber sut,
        string customerId,
        string correlationId,
        CancellationToken cancellationToken)
    {
        var model = new CustomerModel { Id = customerId, EventRaisedAt = new DateTimeOffset(2026, 4, 2, 0, 0, 0, TimeSpan.Zero) };
        var existing = new CustomerEntity { Id = customerId, EventRaisedAt = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero) };
        var @event = new Event { Metadata = new ValueMetadata { Type = ValueType.CustomerUpserted, CorrelationId = correlationId } };

        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => eventMapper.MapTo(@event)).Returns(model);
        A.CallTo(() => customerRepository.UpsertNakedAsync(customerId, cancellationToken)).Returns(existing);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        var result = await sut.HandleAsync(eventContext, new Key(), @event, cancellationToken);

        result.ShouldBe(EventSubscriberResults.Success);
        A.CallTo(() => customerReadinessPublisher.PublishProvisionedAsync(customerId, correlationId, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Not_Publish_After_Stale_CustomerUpserted(
        [Frozen] IEventMapper eventMapper,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] ICustomerReadinessPublisher customerReadinessPublisher,
        [Frozen] EventContext eventContext,
        CustomerSubscriber sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        var model = new CustomerModel { Id = customerId, EventRaisedAt = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero) };
        var existing = new CustomerEntity { Id = customerId, EventRaisedAt = new DateTimeOffset(2026, 4, 2, 0, 0, 0, TimeSpan.Zero) };
        var @event = new Event { Metadata = new ValueMetadata { Type = ValueType.CustomerUpserted } };

        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => eventMapper.MapTo(@event)).Returns(model);
        A.CallTo(() => customerRepository.UpsertNakedAsync(customerId, cancellationToken)).Returns(existing);

        var result = await sut.HandleAsync(eventContext, new Key(), @event, cancellationToken);

        result.ShouldBe(EventSubscriberResults.Success);
        A.CallTo(() => customerReadinessPublisher.PublishProvisionedAsync(A<string>._, A<string?>._, cancellationToken))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Not_Publish_After_CustomerDeleted(
        [Frozen] IEventMapper eventMapper,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] ICustomerReadinessPublisher customerReadinessPublisher,
        [Frozen] EventContext eventContext,
        CustomerSubscriber sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        var model = new CustomerModel { Id = customerId, EventRaisedAt = new DateTimeOffset(2026, 4, 2, 0, 0, 0, TimeSpan.Zero) };
        var existing = new CustomerEntity { Id = customerId, EventRaisedAt = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero) };
        var @event = new Event { Metadata = new ValueMetadata { Type = ValueType.CustomerDeleted } };

        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => eventMapper.MapTo(@event)).Returns(model);
        A.CallTo(() => customerRepository.GetByIdAsync(customerId, cancellationToken)).Returns(existing);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        var result = await sut.HandleAsync(eventContext, new Key(), @event, cancellationToken);

        result.ShouldBe(EventSubscriberResults.Success);
        A.CallTo(() => customerReadinessPublisher.PublishProvisionedAsync(A<string>._, A<string?>._, cancellationToken))
            .MustNotHaveHappened();
    }
}
