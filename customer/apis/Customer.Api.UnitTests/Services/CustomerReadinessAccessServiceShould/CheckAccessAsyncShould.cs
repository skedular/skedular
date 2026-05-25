using Customer.Api.Services;
using Customer.Shared.Models;
using Customer.Shared.Services.Cache;
using CustomerEntity = Customer.Shared.Database.Entities.Customer;

namespace Customer.Api.UnitTests.Services.CustomerReadinessAccessServiceShould;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CheckAccessAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Blocked_When_No_Customer_Found(
        [Frozen] ICachedCustomerService cachedCustomerService,
        CustomerReadinessAccessService sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => cachedCustomerService.GetByIdAsync(customerId, cancellationToken))
            .Returns(null);

        var result = await sut.CheckAccessAsync(customerId, cancellationToken);

        result.IsAllowed.ShouldBeFalse();
        result.MissingDomains.ShouldNotBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Blocked_When_Partial_Domains(
        [Frozen] ICachedCustomerService cachedCustomerService,
        CustomerReadinessAccessService sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        var customer = new CustomerEntity
        {
            Id = customerId, ProvisionedDomains = [CustomerReadinessState.Domains.Booking, CustomerReadinessState.Domains.Organization]
        };

        A.CallTo(() => cachedCustomerService.GetByIdAsync(customerId, cancellationToken))
            .Returns(customer);

        var result = await sut.CheckAccessAsync(customerId, cancellationToken);

        result.IsAllowed.ShouldBeFalse();
        result.MissingDomains.ShouldNotBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Allowed_When_All_Required_Domains_Provisioned(
        [Frozen] ICachedCustomerService cachedCustomerService,
        CustomerReadinessAccessService sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        var customer = new CustomerEntity { Id = customerId, ProvisionedDomains = CustomerReadinessState.RequiredDomains.ToList() };

        A.CallTo(() => cachedCustomerService.GetByIdAsync(customerId, cancellationToken))
            .Returns(customer);

        var result = await sut.CheckAccessAsync(customerId, cancellationToken);

        result.IsAllowed.ShouldBeTrue();
        result.MissingDomains.ShouldBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Only_Call_CachedCustomerService_No_Other_Domain_Service(
        [Frozen] ICachedCustomerService cachedCustomerService,
        CustomerReadinessAccessService sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => cachedCustomerService.GetByIdAsync(customerId, cancellationToken))
            .Returns(null);

        await sut.CheckAccessAsync(customerId, cancellationToken);

        // Only ICachedCustomerService.GetByIdAsync should be called — no other domain services.
        A.CallTo(() => cachedCustomerService.GetByIdAsync(customerId, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
