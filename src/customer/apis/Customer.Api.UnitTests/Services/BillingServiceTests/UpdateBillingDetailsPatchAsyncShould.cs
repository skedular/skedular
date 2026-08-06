using Customer.Api.Models;
using Customer.Api.Services;
using Customer.Shared.Mappers;
using Customer.Shared.Models;
using Customer.Shared.Repositories;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Testing.Shared.Assertions;
using CustomerEntity = Customer.Shared.Database.Entities.Customer;
using CustomerBillingDetailsEntity = Customer.Shared.Database.Entities.CustomerBillingDetails;
using CustomerModel = Customer.Shared.Models.Customer;

namespace Customer.Api.UnitTests.Services.BillingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdateBillingDetailsPatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Apply_Only_Selected_Company_Name_And_Preserve_Other_Fields(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ICustomerBillingDetailsRepository billingDetailsRepository,
        [Frozen]
        ICustomerService customerService,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IDbContextTransaction transaction,
        [Frozen]
        IUnitOfWork unitOfWork,
        BillingService sut,
        CancellationToken cancellationToken)
    {
        var customerEntity = new CustomerEntity
        {
            Id = "cust-1",
        };
        var customerModel = new CustomerModel
        {
            Id = "cust-1",
        };
        var existingBillingEntity = new CustomerBillingDetailsEntity
        {
            Id = "billing-1",
            Customer = customerEntity,
            CompanyName = "Old Company",
            Email = "old@example.com",
        };
        var existingBillingModel = new CustomerBillingDetails
        {
            Id = "billing-1",
            CompanyName = "Old Company",
            Email = "old@example.com",
        };
        var updatedCustomerModel = new CustomerModel
        {
            Id = "cust-1",
        };
        var request = new CustomerBillingDetailsPatchRequest(
            new CustomerBillingDetails
            {
                Id = "billing-1",
                CompanyName = "New Company",
            },
            new HashSet<CustomerBillingDetailsPatchField>
            {
                CustomerBillingDetailsPatchField.CompanyName,
            });

        A.CallTo(() => repositoryFactory.CustomerBillingDetailsRepository).Returns(billingDetailsRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customerModel, customerEntity));
        A.CallTo(() => billingDetailsRepository.GetByIdAsync("billing-1", cancellationToken)).Returns(existingBillingEntity);
        A.CallTo(() => entityMapper.MapTo(existingBillingEntity)).Returns(existingBillingModel);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => entityMapper.MergeToEntity(A<CustomerBillingDetails>._, existingBillingEntity, customerEntity)).Returns(existingBillingEntity);
        A.CallTo(() => entityMapper.MapTo(customerEntity)).Returns(updatedCustomerModel);

        var result = await sut.UpdateAsync(request, cancellationToken);

        result.ShouldBe(updatedCustomerModel);
        existingBillingModel.CompanyName.ShouldBe("New Company");
        existingBillingModel.Email.ShouldBe("old@example.com");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Autosave_Started_And_Completed(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ICustomerBillingDetailsRepository billingDetailsRepository,
        [Frozen]
        ICustomerService customerService,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IDbContextTransaction transaction,
        [Frozen]
        ILogger<BillingService> logger,
        [Frozen]
        IUnitOfWork unitOfWork,
        BillingService sut,
        CancellationToken cancellationToken)
    {
        var customerEntity = new CustomerEntity
        {
            Id = "cust-1",
        };
        var customerModel = new CustomerModel
        {
            Id = "cust-1",
        };
        var existingBillingEntity = new CustomerBillingDetailsEntity
        {
            Id = "billing-1",
            Customer = customerEntity,
        };
        var updatedCustomerModel = new CustomerModel
        {
            Id = "cust-1",
        };
        var request = new CustomerBillingDetailsPatchRequest(
            new CustomerBillingDetails
            {
                Id = "billing-1",
                CompanyName = "New Company",
            },
            new HashSet<CustomerBillingDetailsPatchField>
            {
                CustomerBillingDetailsPatchField.CompanyName,
            });

        A.CallTo(() => repositoryFactory.CustomerBillingDetailsRepository).Returns(billingDetailsRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customerModel, customerEntity));
        A.CallTo(() => billingDetailsRepository.GetByIdAsync("billing-1", cancellationToken)).Returns(existingBillingEntity);
        A.CallTo(() => entityMapper.MapTo(existingBillingEntity)).Returns(new CustomerBillingDetails
        {
            Id = "billing-1",
        });
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => entityMapper.MergeToEntity(A<CustomerBillingDetails>._, existingBillingEntity, customerEntity)).Returns(existingBillingEntity);
        A.CallTo(() => entityMapper.MapTo(customerEntity)).Returns(updatedCustomerModel);

        await sut.UpdateAsync(request, cancellationToken);

        LogAssertions.ACallToLogInfoContaining(logger, "Customer billing details patch autosave started").MustHaveHappenedOnceExactly();
        LogAssertions.ACallToLogInfoContaining(logger, "Customer billing details patch autosave completed").MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Authorization_Rejection_And_Rethrow(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ICustomerBillingDetailsRepository billingDetailsRepository,
        [Frozen]
        ICustomerService customerService,
        [Frozen]
        ILogger<BillingService> logger,
        BillingService sut,
        CancellationToken cancellationToken)
    {
        var customerEntity = new CustomerEntity
        {
            Id = "cust-1",
        };
        var customerModel = new CustomerModel
        {
            Id = "cust-1",
        };
        var existingBillingEntity = new CustomerBillingDetailsEntity
        {
            Id = "billing-1",
            Customer = new CustomerEntity
            {
                Id = "other-cust",
            },
        };
        var request = new CustomerBillingDetailsPatchRequest(
            new CustomerBillingDetails
            {
                Id = "billing-1",
                CompanyName = "New Company",
            },
            new HashSet<CustomerBillingDetailsPatchField>
            {
                CustomerBillingDetailsPatchField.CompanyName,
            });

        A.CallTo(() => repositoryFactory.CustomerBillingDetailsRepository).Returns(billingDetailsRepository);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customerModel, customerEntity));
        A.CallTo(() => billingDetailsRepository.GetByIdAsync("billing-1", cancellationToken)).Returns(existingBillingEntity);

        await Should.ThrowAsync<UnauthorizedAccessException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Warning)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("rejected by authorization"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Error_And_Rethrow_On_General_Failure(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ICustomerBillingDetailsRepository billingDetailsRepository,
        [Frozen]
        ICustomerService customerService,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        ILogger<BillingService> logger,
        [Frozen]
        IUnitOfWork unitOfWork,
        BillingService sut,
        CancellationToken cancellationToken)
    {
        var customerEntity = new CustomerEntity
        {
            Id = "cust-1",
        };
        var customerModel = new CustomerModel
        {
            Id = "cust-1",
        };
        var existingBillingEntity = new CustomerBillingDetailsEntity
        {
            Id = "billing-1",
            Customer = customerEntity,
        };
        var request = new CustomerBillingDetailsPatchRequest(
            new CustomerBillingDetails
            {
                Id = "billing-1",
                CompanyName = "New Company",
            },
            new HashSet<CustomerBillingDetailsPatchField>
            {
                CustomerBillingDetailsPatchField.CompanyName,
            });

        A.CallTo(() => repositoryFactory.CustomerBillingDetailsRepository).Returns(billingDetailsRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customerModel, customerEntity));
        A.CallTo(() => billingDetailsRepository.GetByIdAsync("billing-1", cancellationToken)).Returns(existingBillingEntity);
        A.CallTo(() => entityMapper.MapTo(existingBillingEntity)).Returns(new CustomerBillingDetails
        {
            Id = "billing-1",
        });
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken))
            .ThrowsAsync(new InvalidOperationException("transaction failed"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Error)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("Customer billing details patch autosave failed"))
            .MustHaveHappenedOnceExactly();
    }
}
