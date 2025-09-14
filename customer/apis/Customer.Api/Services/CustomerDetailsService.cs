using Api.Shared.Services.Models;

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
        PersonalInformationVisibility personalInformationVisibility,
        CancellationToken cancellationToken);

    Task<Shared.Models.Customer> UpdateCustomerDetailsAsync(
        string id,
        string? timezone,
        string? designation,
        string? title,
        string? name,
        string? givenName,
        string? middleName,
        string? familyName,
        string? phoneNumber,
        PersonalInformationVisibility personalInformationVisibility,
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
        PersonalInformationVisibility personalInformationVisibility,
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
        customer.PersonalInformationVisibility = personalInformationVisibility.ToPersonalInformationVisibility();

        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> UpdateCustomerDetailsAsync(
        string id,
        string? timezone,
        string? designation,
        string? title,
        string? name,
        string? givenName,
        string? middleName,
        string? familyName,
        string? phoneNumber,
        PersonalInformationVisibility personalInformationVisibility,
        CancellationToken cancellationToken)
    {
        var customer = await customerHelperService.GetCustomerAsync(cancellationToken);
        if (customer.Id != id)
        {
            throw new UnauthorizedAccessException();
        }

        customer.Timezone = timezone;
        customer.Designation = designation;
        customer.Title = title;
        customer.Name = name;
        customer.GivenName = givenName;
        customer.MiddleName = middleName;
        customer.FamilyName = familyName;
        customer.PhoneNumber = phoneNumber;
        customer.PersonalInformationVisibility = personalInformationVisibility.ToPersonalInformationVisibility();

        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }
}
