using Api.Shared.Clients.Events.Skedular.Billing.V1.Value;
using Billing.Shared.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;

namespace Billing.Shared.Mappers;

public interface IMapper
{
    OrganizationOfferingBilling MapTo(OrganizationOffering src);
    OrganizationBillingInfo MapTo(Organization src);
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

    public OrganizationBillingInfo MapTo(Organization src) =>
        new()
        {
            OrganizationId = src.Id,
            BillingContactEmail = src.BillingContactEmail.ToSafeString(),
            BillingContactAddressLine1 = src.BillingContactAddressLine1.ToSafeString(),
            BillingContactAddressLine2 = src.BillingContactAddressLine2.ToSafeString(),
            BillingContactSuburb = src.BillingContactSuburb.ToSafeString(),
            BillingContactCity = src.BillingContactCity.ToSafeString(),
            BillingContactProvince = src.BillingContactProvince.ToSafeString(),
            BillingContactZipcode = src.BillingContactZipcode.ToSafeString(),
            BillingContactCountry = src.BillingContactCountry.ToSafeString()
        };
}
