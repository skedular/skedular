using Api.Shared.Clients.Events.Skedular.Billing.V1.Value;
using Billing.Shared.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;

namespace Billing.Shared.Mappers;

public interface IMapper
{
    OrganizationOfferingBilling MapTo(OrganizationOffering src);
    CustomerBillingContact MapTo(Customer src);
}

public class Mapper : IMapper
{
    public OrganizationOfferingBilling MapTo(OrganizationOffering src) =>
        new()
        {
            OfferingId = src.Id,
            OrganizationId = src.Organization.Id,
            TotalCost = src.TotalCost,
            InvoiceDate = Timestamp.FromDateTimeOffset(src.InvoiceDate!.Value)
        };

    public CustomerBillingContact MapTo(Customer src) =>
        new()
        {
            CustomerId = src.Id,
            CompanyName = src.BillingContactCompanyName.ToSafeString(),
            Email = src.BillingContactEmail.ToSafeString(),
            AddressLine1 = src.BillingContactAddressLine1.ToSafeString(),
            AddressLine2 = src.BillingContactAddressLine2.ToSafeString(),
            Suburb = src.BillingContactSuburb.ToSafeString(),
            City = src.BillingContactCity.ToSafeString(),
            Province = src.BillingContactProvince.ToSafeString(),
            Zipcode = src.BillingContactZipcode.ToSafeString(),
            Country = src.BillingContactCountry.ToSafeString()
        };
}
