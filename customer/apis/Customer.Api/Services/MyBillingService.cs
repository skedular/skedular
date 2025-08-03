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
        await repositoryFactory.CustomerBillingDetailsRepository.AddAsync(organizationBillingDetailsEntity, cancellationToken);

        customerEntity.BillingDetails = organizationBillingDetailsEntity;
        var mappedCustomer = mapper.MapTo(customerEntity);

        organizationOutboxPublisher.PublishCustomers([mapper.MapTo(customerEntity)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

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

        customer = await UpdateInternalAsync(customerBillingDetails, existingCustomerBillingDetails, customerEntity, cancellationToken);

        return customer;
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
        await repositoryFactory.CustomerBillingDetailsRepository.UpdateAsync(organizationBillingDetailsEntity, cancellationToken);

        existingCustomer.BillingDetails = organizationBillingDetailsEntity;

        var mappedCustomer = mapper.MapTo(existingCustomer);
        organizationOutboxPublisher.PublishCustomers([mappedCustomer], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedCustomer;
    }
}
