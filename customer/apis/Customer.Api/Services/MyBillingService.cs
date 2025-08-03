using Api.Shared.Services;
using Customer.Api.Mappers;
using Customer.Shared.Models;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;

namespace Customer.Api.Services;

public interface IMyBillingService
{
    Task<Shared.Models.Customer> AddAsync(CustomerBillingDetails customerBillingDetails, CancellationToken cancellationToken);
    Task<Shared.Models.Customer> UpdateAsync(CustomerBillingDetails customerBillingDetails, CancellationToken cancellationToken);
}

public class MyBillingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    IRandomHelper randomHelper,
    IMapper mapper,
    ICachedCustomerService cachedCustomerService,
    ICustomerOutboxPublisher organizationOutboxPublisher) : IMyBillingService
{
    public async Task<Shared.Models.Customer> AddAsync(CustomerBillingDetails customerBillingDetails, CancellationToken cancellationToken)
    {
        var (customer, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(customerBillingDetails.Id))
        {
            var existingCustomerBillingDetails = await repositoryFactory.CustomerBillingDetailsRepository.GetByIdAsync(
                customerBillingDetails.Id,
                cancellationToken);
            if (existingCustomerBillingDetails is not null)
            {
                if (existingCustomerBillingDetails.Customer.Id != customer.Id)
                {
                    throw new UnauthorizedAccessException();
                }

                return await UpdateInternalAsync(
                    customerBillingDetails,
                    existingCustomerBillingDetails,
                    customerEntity,
                    cancellationToken);
            }
        }
        else
        {
            customerBillingDetails.Id = randomHelper.Generate();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var organizationBillingDetailsEntity = mapper.MapTo(customerBillingDetails, customerEntity);
        repositoryFactory.CustomerBillingDetailsRepository.Add(organizationBillingDetailsEntity);

        customerEntity.BillingDetails = organizationBillingDetailsEntity;
        var mappedCustomer = mapper.MapTo(customerEntity);

        organizationOutboxPublisher.PublishCustomers([mapper.MapTo(customerEntity)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedCustomerService.CleanCacheAsync(customerEntity, cancellationToken);

        return mappedCustomer;
    }

    public async Task<Shared.Models.Customer> UpdateAsync(CustomerBillingDetails customerBillingDetails, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(customerBillingDetails.Id);

        var (customer, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        var existingCustomerBillingDetails = await repositoryFactory.CustomerBillingDetailsRepository.GetByIdAsync(
            customerBillingDetails.Id,
            cancellationToken) ?? throw new CustomerBillingDetailsNotFound();
        if (existingCustomerBillingDetails.Customer.Id != customer.Id)
        {
            throw new UnauthorizedAccessException();
        }

        return await UpdateInternalAsync(customerBillingDetails, existingCustomerBillingDetails, customerEntity, cancellationToken);
    }

    private async Task<Shared.Models.Customer> UpdateInternalAsync(
        CustomerBillingDetails organizationBillingDetails,
        Shared.Database.Entities.CustomerBillingDetails existingCustomerBillingDetails,
        Shared.Database.Entities.Customer existingCustomer,
        CancellationToken cancellationToken)
    {
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var organizationBillingDetailsEntity =
            mapper.MergeToEntity(organizationBillingDetails, existingCustomerBillingDetails, existingCustomer);
        repositoryFactory.CustomerBillingDetailsRepository.Update(organizationBillingDetailsEntity);

        existingCustomer.BillingDetails = organizationBillingDetailsEntity;

        var mappedCustomer = mapper.MapTo(existingCustomer);
        organizationOutboxPublisher.PublishCustomers([mappedCustomer], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedCustomerService.CleanCacheAsync(existingCustomer, cancellationToken);

        return mappedCustomer;
    }
}
