namespace Customer.Api.Services;

public interface ICustomerDetailsService
{
    Task<Shared.Models.Customer> UpdateMyCustomerDetailsAsync(
        string? timezone,
        string? designation,
        string? title,
        string? name,
        string? givenName,
        string? middleName,
        string? familyName,
        string? phoneNumber,
        CancellationToken cancellationToken);
}

public class CustomerDetailsService(ICustomerHelperService customerHelperService) : ICustomerDetailsService
{
    public async Task<Shared.Models.Customer> UpdateMyCustomerDetailsAsync(
        string? timezone,
        string? designation,
        string? title,
        string? name,
        string? givenName,
        string? middleName,
        string? familyName,
        string? phoneNumber,
        CancellationToken cancellationToken)
    {
        var customer = await customerHelperService.GetCustomerAsync(cancellationToken);
        customer.Timezone = timezone;
        customer.Designation = designation;
        customer.Title = title;
        customer.Name = name;
        customer.GivenName = givenName;
        customer.MiddleName = middleName;
        customer.FamilyName = familyName;
        customer.PhoneNumber = phoneNumber;

        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }
}
