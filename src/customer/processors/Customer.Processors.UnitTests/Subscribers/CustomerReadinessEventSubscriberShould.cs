using Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1;
using Customer.Processors.Subscribers;
using Customer.Shared.Repositories;
using Customer.Shared.Services.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using ValueDomain = Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1.Domain;
using ValueType = Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1.Type;

namespace Customer.Processors.UnitTests.Subscribers;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CustomerReadinessEventSubscriberShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Mark_Domain_Provisioned_And_Clear_Cache_On_CustomerIdentityProvisioned(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ICustomerRepository customerRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        EventContext eventContext,
        CustomerReadinessEventSubscriber sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        var @event = new Event
        {
            Metadata = new Metadata
            {
                Type = ValueType.CustomerIdentityProvisioned,
            },
            Data = new Data
            {
                CustomerIdentityProvisioned = new CustomerIdentityProvisioned
                {
                    CustomerId = customerId,
                    Domain = ValueDomain.Booking,
                },
            },
        };

        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        var result = await sut.HandleAsync(eventContext, new Key(), @event, cancellationToken);

        result.ShouldBe(EventSubscriberResults.Success);
        A.CallTo(() => customerRepository.MarkDomainProvisionedAsync(customerId, A<string>._, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedCustomerService.RemoveByIdAsync(customerId, cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Success_And_Skip_Persist_For_Unmappable_Domain(
        [Frozen]
        ICustomerRepository customerRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        EventContext eventContext,
        CustomerReadinessEventSubscriber sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        var @event = new Event
        {
            Metadata = new Metadata
            {
                Type = ValueType.CustomerIdentityProvisioned,
            },
            Data = new Data
            {
                CustomerIdentityProvisioned = new CustomerIdentityProvisioned
                {
                    CustomerId = customerId,
                    Domain = (ValueDomain)999,
                },
            },
        };

        var result = await sut.HandleAsync(eventContext, new Key(), @event, cancellationToken);

        result.ShouldBe(EventSubscriberResults.Success);
        A.CallTo(() => customerRepository.MarkDomainProvisionedAsync(A<string>._, A<string>._, cancellationToken)).MustNotHaveHappened();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustNotHaveHappened();
        A.CallTo(() => cachedCustomerService.RemoveByIdAsync(A<string>._, cancellationToken)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Success_For_Unknown_Event_Type(
        [Frozen]
        ICustomerRepository customerRepository,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        EventContext eventContext,
        CustomerReadinessEventSubscriber sut,
        CancellationToken cancellationToken)
    {
        var @event = new Event
        {
            Metadata = new Metadata
            {
                Type = (ValueType)999,
            },
            Data = new Data(),
        };

        var result = await sut.HandleAsync(eventContext, new Key(), @event, cancellationToken);

        result.ShouldBe(EventSubscriberResults.Success);
        A.CallTo(() => customerRepository.MarkDomainProvisionedAsync(A<string>._, A<string>._, cancellationToken)).MustNotHaveHappened();
        A.CallTo(() => cachedCustomerService.RemoveByIdAsync(A<string>._, cancellationToken)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Map_All_Known_Domains(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ICustomerRepository customerRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        EventContext eventContext,
        CustomerReadinessEventSubscriber sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        var knownDomains = new[]
        {
            ValueDomain.Booking, ValueDomain.Organization, ValueDomain.Team, ValueDomain.Marketplace, ValueDomain.Location, ValueDomain.Core,
            ValueDomain.Slack, ValueDomain.MsTeams,
        };

        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        foreach (var domain in knownDomains)
        {
            var @event = new Event
            {
                Metadata = new Metadata
                {
                    Type = ValueType.CustomerIdentityProvisioned,
                },
                Data = new Data
                {
                    CustomerIdentityProvisioned = new CustomerIdentityProvisioned
                    {
                        CustomerId = customerId,
                        Domain = domain,
                    },
                },
            };

            var result = await sut.HandleAsync(eventContext, new Key(), @event, cancellationToken);

            result.ShouldBe(EventSubscriberResults.Success);
        }

        A.CallTo(() => customerRepository.MarkDomainProvisionedAsync(customerId, A<string>._, cancellationToken))
            .MustHaveHappened(knownDomains.Length, Times.Exactly);
    }
}
