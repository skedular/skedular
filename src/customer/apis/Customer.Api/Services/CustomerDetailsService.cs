using Api.Shared.Services.Models;
using Customer.Api.Models;

namespace Customer.Api.Services;

public interface ICustomerDetailsService
{
    Task<Shared.Models.Customer> UpdateMyCustomerDetailsAsync(CustomerDetailsPatchRequest request, CancellationToken cancellationToken);

    Task<Shared.Models.Customer> UpdateCustomerDetailsAsync(string id, CustomerDetailsPatchRequest request, CancellationToken cancellationToken);
}

public class CustomerDetailsService(
    ICustomerHelperService customerHelperService,
    ILogger<CustomerDetailsService> logger) : ICustomerDetailsService
{
    public async Task<Shared.Models.Customer> UpdateMyCustomerDetailsAsync(
        CustomerDetailsPatchRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await customerHelperService.GetCustomerAsync(cancellationToken);
        var editUnits = string.Join(",", request.FieldsToUpdate);
        logger.LogInformation(
            "My customer details patch autosave started. CustomerId: {CustomerId}, EditUnits: {EditUnits}",
            customer.Id,
            editUnits);

        try
        {
            Apply(request, customer);
            var updatedCustomer = await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
            logger.LogInformation(
                "My customer details patch autosave completed. CustomerId: {CustomerId}, EditUnits: {EditUnits}",
                updatedCustomer.Id,
                editUnits);
            return updatedCustomer;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "My customer details patch autosave failed. CustomerId: {CustomerId}, EditUnits: {EditUnits}",
                customer.Id,
                editUnits);
            throw;
        }
    }

    public async Task<Shared.Models.Customer> UpdateCustomerDetailsAsync(
        string id,
        CustomerDetailsPatchRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await customerHelperService.GetCustomerAsync(cancellationToken);
        var editUnits = string.Join(",", request.FieldsToUpdate);
        logger.LogInformation(
            "Customer details patch autosave started. CustomerId: {CustomerId}, RequestedCustomerId: {RequestedCustomerId}, EditUnits: {EditUnits}",
            customer.Id,
            id,
            editUnits);

        if (customer.Id != id)
        {
            logger.LogWarning(
                "Customer details patch autosave rejected by authorization. CustomerId: {CustomerId}, RequestedCustomerId: {RequestedCustomerId}, EditUnits: {EditUnits}",
                customer.Id,
                id,
                editUnits);
            throw new UnauthorizedAccessException();
        }

        try
        {
            Apply(request, customer);
            var updatedCustomer = await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
            logger.LogInformation(
                "Customer details patch autosave completed. CustomerId: {CustomerId}, EditUnits: {EditUnits}",
                updatedCustomer.Id,
                editUnits);
            return updatedCustomer;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Customer details patch autosave failed. CustomerId: {CustomerId}, RequestedCustomerId: {RequestedCustomerId}, EditUnits: {EditUnits}",
                customer.Id,
                id,
                editUnits);
            throw;
        }
    }

    private static void Apply(CustomerDetailsPatchRequest request, Shared.Database.Entities.Customer customer)
    {
        foreach (var field in request.FieldsToUpdate)
        {
            switch (field)
            {
                case CustomerDetailsPatchField.Timezone:
                    customer.Timezone = request.Timezone;
                    break;
                case CustomerDetailsPatchField.Designation:
                    customer.Designation = request.Designation;
                    break;
                case CustomerDetailsPatchField.Title:
                    customer.Title = request.Title;
                    break;
                case CustomerDetailsPatchField.Name:
                    customer.Name = request.Name;
                    break;
                case CustomerDetailsPatchField.GivenName:
                    customer.GivenName = request.GivenName;
                    break;
                case CustomerDetailsPatchField.MiddleName:
                    customer.MiddleName = request.MiddleName;
                    break;
                case CustomerDetailsPatchField.FamilyName:
                    customer.FamilyName = request.FamilyName;
                    break;
                case CustomerDetailsPatchField.PhoneNumber:
                    customer.PhoneNumber = request.PhoneNumber;
                    break;
                case CustomerDetailsPatchField.PersonalInformationVisibility:
                    customer.PersonalInformationVisibility = request.PersonalInformationVisibility.ToPersonalInformationVisibility();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(request.FieldsToUpdate), field, null);
            }
        }
    }
}
