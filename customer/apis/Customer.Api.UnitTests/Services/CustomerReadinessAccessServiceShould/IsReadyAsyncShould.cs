using Customer.Api.Services;
using Customer.Shared.Models;
using Customer.Shared.Services.Cache;
using CustomerEntity = Customer.Shared.Database.Entities.Customer;

namespace Customer.Api.UnitTests.Services.CustomerReadinessAccessServiceShould;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class IsReadyAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_False_When_No_Customer_Found(
        [Frozen] ICachedCustomerService cachedCustomerService,
        CustomerReadinessAccessService sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => cachedCustomerService.GetNullableAsync(cancellationToken)).Returns(null);

        var result = await sut.IsReadyAsync(cancellationToken);

        result.ShouldBeFalse();
        A.CallTo(() => cachedCustomerService.RemoveByIdAsync(A<string>._, cancellationToken)).MustNotHaveHappened();
        A.CallTo(() => cachedCustomerService.GetAsync(cancellationToken)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_False_When_Partial_Domains_Remain_Partial_After_Refresh(
        [Frozen] ICachedCustomerService cachedCustomerService,
        CustomerReadinessAccessService sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        var initial = new CustomerEntity
        {
            Id = customerId, ProvisionedDomains = [CustomerReadinessState.Domains.Booking, CustomerReadinessState.Domains.Organization]
        };
        var refreshed = new CustomerEntity
        {
            Id = customerId, ProvisionedDomains = [CustomerReadinessState.Domains.Booking, CustomerReadinessState.Domains.Organization]
        };

        A.CallTo(() => cachedCustomerService.GetNullableAsync(cancellationToken)).Returns(initial);
        A.CallTo(() => cachedCustomerService.GetAsync(cancellationToken)).Returns(refreshed);

        var result = await sut.IsReadyAsync(cancellationToken);

        result.ShouldBeFalse();
        A.CallTo(() => cachedCustomerService.RemoveAsync(
                A<IReadOnlyList<CustomerEntity>>.That.Matches(customers => customers.Count == 1 && customers[0].Id == customerId),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedCustomerService.GetAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_True_When_All_Required_Domains_Provisioned_On_First_Read(
        [Frozen] ICachedCustomerService cachedCustomerService,
        CustomerReadinessAccessService sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        var customer = new CustomerEntity { Id = customerId, ProvisionedDomains = CustomerReadinessState.RequiredDomains.ToList() };

        A.CallTo(() => cachedCustomerService.GetNullableAsync(cancellationToken))
            .Returns(customer);

        var result = await sut.IsReadyAsync(cancellationToken);

        result.ShouldBeTrue();
        A.CallTo(() => cachedCustomerService.RemoveByIdAsync(A<string>._, cancellationToken)).MustNotHaveHappened();
        A.CallTo(() => cachedCustomerService.GetAsync(cancellationToken)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_True_When_Refresh_Makes_Customer_Ready(
        [Frozen] ICachedCustomerService cachedCustomerService,
        CustomerReadinessAccessService sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        var initial = new CustomerEntity { Id = customerId, ProvisionedDomains = [CustomerReadinessState.Domains.Booking] };
        var refreshed = new CustomerEntity { Id = customerId, ProvisionedDomains = CustomerReadinessState.RequiredDomains.ToList() };

        A.CallTo(() => cachedCustomerService.GetNullableAsync(cancellationToken)).Returns(initial);
        A.CallTo(() => cachedCustomerService.GetAsync(cancellationToken)).Returns(refreshed);

        var result = await sut.IsReadyAsync(cancellationToken);

        result.ShouldBeTrue();
        A.CallTo(() => cachedCustomerService.RemoveAsync(
                A<IReadOnlyList<CustomerEntity>>.That.Matches(customers => customers.Count == 1 && customers[0].Id == customerId),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedCustomerService.GetAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
