using Api.Shared.Clients.Events.Skedular.Customer.V1;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using Microsoft.Extensions.Logging;
using Team.Processors.Mappers;
using Team.Processors.Subscribers;
using Team.Shared.Repositories;
using Team.Shared.Services.Cache;
using Customer = Team.Shared.Models.Customer;
using ValueMetadata = Api.Shared.Clients.Events.Skedular.Customer.V1.Metadata;
using ValueType = Api.Shared.Clients.Events.Skedular.Customer.V1.Type;

namespace Team.Processors.UnitTests.Subscribers.CustomerSubscriberTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class HandleAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_And_Skip_When_Upsert_Event_Is_Stale(
        [Frozen]
        IEventMapper eventMapper,
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ICustomerRepository customerRepository,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        ILogger<CustomerSubscriber> logger,
        [Frozen]
        EventContext eventContext,
        CustomerSubscriber sut,
        CancellationToken cancellationToken)
    {
        var model = new Customer
        {
            Id = "customer-1",
            EventRaisedAt = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            Identities = [],
        };
        var existing = new Shared.Database.Entities.Customer
        {
            Id = "customer-1",
            EventRaisedAt = new DateTimeOffset(2026, 4, 2, 0, 0, 0, TimeSpan.Zero),
        };
        var @event = new Event
        {
            Metadata = new ValueMetadata
            {
                Type = ValueType.CustomerUpserted,
            },
        };

        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => eventMapper.MapTo(@event)).Returns(model);
        A.CallTo(() => customerRepository.UpsertNakedAsync("customer-1", cancellationToken)).Returns(existing);

        var result = await sut.HandleAsync(eventContext, new Key(), @event, cancellationToken);

        result.ShouldBe(EventSubscriberResults.Success);
        A.CallTo(() => cachedCustomerService.RemoveAsync(A<IReadOnlyList<Shared.Database.Entities.Customer>>._, cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(logger)
            .Where(call => call.Method.Name == nameof(ILogger.Log) && call.GetArgument<LogLevel>(0) == LogLevel.Information)
            .MustHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_When_Delete_Event_Is_Processed(
        [Frozen]
        IEventMapper eventMapper,
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ICustomerRepository customerRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        ILogger<CustomerSubscriber> logger,
        [Frozen]
        EventContext eventContext,
        CustomerSubscriber sut,
        CancellationToken cancellationToken)
    {
        var model = new Customer
        {
            Id = "customer-1",
            EventRaisedAt = new DateTimeOffset(2026, 4, 2, 0, 0, 0, TimeSpan.Zero),
            Identities = [],
        };
        var existing = new Shared.Database.Entities.Customer
        {
            Id = "customer-1",
            EventRaisedAt = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
        };
        var @event = new Event
        {
            Metadata = new ValueMetadata
            {
                Type = ValueType.CustomerDeleted,
            },
        };

        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => eventMapper.MapTo(@event)).Returns(model);
        A.CallTo(() => customerRepository.GetByIdAsync("customer-1", cancellationToken)).Returns(existing);
        A.CallTo(() => customerRepository.Remove(existing)).Returns(existing);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        var result = await sut.HandleAsync(eventContext, new Key(), @event, cancellationToken);

        result.ShouldBe(EventSubscriberResults.Success);
        A.CallTo(() => cachedCustomerService.RemoveAsync(
                A<IReadOnlyList<Shared.Database.Entities.Customer>>.That.Matches(items => items.Count == 1 && items.First().Id == "customer-1"),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(logger)
            .Where(call => call.Method.Name == nameof(ILogger.Log) && call.GetArgument<LogLevel>(0) == LogLevel.Information)
            .MustHaveHappened();
    }
}
