using Api.Shared.Services;
using Customer.Api.Models;
using Customer.Shared.Mappers;
using Customer.Shared.Models;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Customer.Shared.Services.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;

namespace Customer.Api.Services;

public interface IBillingService
{
    Task<Shared.Models.Customer> AddAsync(CustomerBillingDetails customerBillingDetails, CancellationToken cancellationToken);
    Task<Shared.Models.Customer> UpdateAsync(CustomerBillingDetailsPatchRequest request, CancellationToken cancellationToken);
    Task<CustomerBillingDetails?> GetBillingAsync(string requestedCustomerId, CancellationToken cancellationToken);
}

public class BillingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    IRandomHelper randomHelper,
    IEntityMapper entityMapper,
    ICustomerOutboxPublisher organizationOutboxPublisher,
    ICachedCustomerService cachedCustomerService,
    ILogger<BillingService> logger) : IBillingService
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

        var organizationBillingDetailsEntity = entityMapper.MapTo(customerBillingDetails, customerEntity);
        repositoryFactory.CustomerBillingDetailsRepository.Add(organizationBillingDetailsEntity);

        customerEntity.BillingDetails = organizationBillingDetailsEntity;
        var mappedCustomer = entityMapper.MapTo(customerEntity);

        organizationOutboxPublisher.PublishCustomers([entityMapper.MapTo(customerEntity)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedCustomerService.UpdateAsync([customerEntity], cancellationToken);

        return mappedCustomer;
    }

    public async Task<Shared.Models.Customer> UpdateAsync(CustomerBillingDetailsPatchRequest request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.BillingDetails.Id);

        var editUnits = string.Join(",", request.FieldsToUpdate);
        logger.LogInformation(
            "Customer billing details patch autosave started. BillingDetailsId: {BillingDetailsId}, EditUnits: {EditUnits}",
            request.BillingDetails.Id,
            editUnits);

        try
        {
            var (customer, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
            var existingCustomerBillingDetails = await repositoryFactory.CustomerBillingDetailsRepository.GetByIdAsync(
                request.BillingDetails.Id,
                cancellationToken) ?? throw new CustomerBillingDetailsNotFound();
            if (existingCustomerBillingDetails.Customer.Id != customer.Id)
            {
                logger.LogWarning(
                    "Customer billing details patch autosave rejected by authorization. CustomerId: {CustomerId}, BillingDetailsId: {BillingDetailsId}, EditUnits: {EditUnits}",
                    customer.Id,
                    request.BillingDetails.Id,
                    editUnits);
                throw new UnauthorizedAccessException();
            }

            var billingDetails = entityMapper.MapTo(existingCustomerBillingDetails)!;
            Apply(request, billingDetails);

            var updatedCustomer = await UpdateInternalAsync(billingDetails, existingCustomerBillingDetails, customerEntity, cancellationToken);
            logger.LogInformation(
                "Customer billing details patch autosave completed. CustomerId: {CustomerId}, BillingDetailsId: {BillingDetailsId}, EditUnits: {EditUnits}",
                updatedCustomer.Id,
                request.BillingDetails.Id,
                editUnits);
            return updatedCustomer;
        }
        catch (Exception exception) when (exception is not UnauthorizedAccessException)
        {
            logger.LogError(
                exception,
                "Customer billing details patch autosave failed. BillingDetailsId: {BillingDetailsId}, EditUnits: {EditUnits}",
                request.BillingDetails.Id,
                editUnits);
            throw;
        }
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

        return entityMapper.MapTo(customerBillingDetails);
    }

    private async Task<Shared.Models.Customer> UpdateInternalAsync(
        CustomerBillingDetails organizationBillingDetails,
        Shared.Database.Entities.CustomerBillingDetails existingCustomerBillingDetails,
        Shared.Database.Entities.Customer existingCustomer,
        CancellationToken cancellationToken)
    {
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var organizationBillingDetailsEntity =
            entityMapper.MergeToEntity(organizationBillingDetails, existingCustomerBillingDetails, existingCustomer);
        repositoryFactory.CustomerBillingDetailsRepository.Update(organizationBillingDetailsEntity);

        existingCustomer.BillingDetails = organizationBillingDetailsEntity;

        var mappedCustomer = entityMapper.MapTo(existingCustomer);
        organizationOutboxPublisher.PublishCustomers([mappedCustomer], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedCustomerService.UpdateAsync([existingCustomer], cancellationToken);

        return mappedCustomer;
    }

    private static void Apply(CustomerBillingDetailsPatchRequest request, CustomerBillingDetails billingDetails)
    {
        foreach (var field in request.FieldsToUpdate)
        {
            switch (field)
            {
                case CustomerBillingDetailsPatchField.CompanyName:
                    billingDetails.CompanyName = request.BillingDetails.CompanyName;
                    break;
                case CustomerBillingDetailsPatchField.Email:
                    billingDetails.Email = request.BillingDetails.Email;
                    break;
                case CustomerBillingDetailsPatchField.BillingAddress:
                    billingDetails.OsmType = request.BillingDetails.OsmType;
                    billingDetails.OsmId = request.BillingDetails.OsmId;
                    billingDetails.PlaceId = request.BillingDetails.PlaceId;
                    billingDetails.Coordinates = request.BillingDetails.Coordinates;
                    billingDetails.FormattedAddress = request.BillingDetails.FormattedAddress;
                    billingDetails.AddressLine1 = request.BillingDetails.AddressLine1;
                    billingDetails.AddressLine2 = request.BillingDetails.AddressLine2;
                    billingDetails.Suburb = request.BillingDetails.Suburb;
                    billingDetails.City = request.BillingDetails.City;
                    billingDetails.Province = request.BillingDetails.Province;
                    billingDetails.Zipcode = request.BillingDetails.Zipcode;
                    billingDetails.Country = request.BillingDetails.Country;
                    billingDetails.CountryCode = request.BillingDetails.CountryCode;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(request.FieldsToUpdate), field,
                        $"Unexpected value for {nameof(request.FieldsToUpdate)}: {field}. Update enum mapping or caller input.");
            }
        }
    }
}
