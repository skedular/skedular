using Api.Shared.Clients.Events.Skedular.Billing.V1.Value;
using Billing.Shared.Models;
using Google.Protobuf.WellKnownTypes;

namespace Billing.Shared.Mappers;

public interface IMapper
{
    OrganizationOfferingBilling MapTo(OrganizationOffering src);
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
}
