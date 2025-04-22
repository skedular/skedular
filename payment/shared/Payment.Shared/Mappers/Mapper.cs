using Api.Shared.Clients.Events.Skedular.Payment.V1.Value;
using Enterprise.Shared;

namespace Payment.Shared.Mappers;

public interface IMapper
{
    OrganizationStripeConnectAccount MapTo(Models.OrganizationStripeConnectAccount src);
}

public class Mapper : IMapper
{
    public OrganizationStripeConnectAccount MapTo(Models.OrganizationStripeConnectAccount src) =>
        new()
        {
            Id = src.Id,
            OrganizationId = src.Organization.Id,
            Name = src.Name.ToSafeString(),
            ChargesEnabled = src.ChargesEnabled,
            PayoutsEnabled = src.PayoutsEnabled,
            Type = src.Type.ToSafeString(),
            Country = src.Country.ToSafeString(),
            DefaultCurrency = src.DefaultCurrency.ToSafeString(),
            BusinessType = src.BusinessType.ToSafeString(),
            CompanyName = src.CompanyName.ToSafeString(),
            Email = src.Email.ToSafeString(),
            Phone = src.Phone.ToSafeString(),
            CapabilitiesCardPayments = src.CapabilitiesCardPayments.ToSafeString(),
            CapabilitiesTransfers = src.CapabilitiesTransfers.ToSafeString(),
            OnboardingUrl = src.OnboardingUrl.ToSafeString()
        };
}
