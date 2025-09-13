using Api.Shared.Services;
using Customer.Api.Mappers;
using Customer.Shared.Models;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;

namespace Customer.Api.Services;

public interface IBillingService
{
    Task<Shared.Models.Customer> AddAsync(CustomerBillingDetails customerBillingDetails, CancellationToken cancellationToken);
    Task<Shared.Models.Customer> UpdateAsync(CustomerBillingDetails customerBillingDetails, CancellationToken cancellationToken);
    Task<CustomerBillingDetails?> GetBillingAsync(string requestedCustomerId, CancellationToken cancellationToken);
}

public class BillingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    IRandomHelper randomHelper,
    IMapper mapper,
    ICustomerOutboxPublisher organizationOutboxPublisher) : IBillingService
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

    public async Task<CustomerBillingDetails?> GetBillingAsync(string requestedCustomerId, CancellationToken cancellationToken)
    {
        var (_, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        if (customerEntity.Id != requestedCustomerId)
        {
            return null;
        }

        var customerBillingDetails =
            await repositoryFactory.CustomerBillingDetailsRepository.GetByCustomerIdAsync(customerEntity.Id, cancellationToken);

        return mapper.MapTo(customerBillingDetails);
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

        return mappedCustomer;
    }
}
