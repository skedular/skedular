using Api.Shared.Services.Models;
using Enterprise.Shared;
using Payment.Shared.Models;
using Stripe;
using Customer = Payment.Shared.Models.Customer;
using Identity = Payment.Shared.Database.Entities.Identity;
using Organization = Payment.Shared.Database.Entities.Organization;
using StripeConnectAccount = Payment.Shared.Database.Entities.StripeConnectAccount;

namespace Payment.Api.Mappers;

public interface IMapper
{
    Customer MapTo(Shared.Database.Entities.Customer src);
    AccountCreateOptions MapToStripeAccountRequest(Organization src);
    StripeConnectAccount MapTo(Account src, string id, string name, Organization organization);
    Shared.Models.StripeConnectAccount MapTo(StripeConnectAccount src);
}

public class Mapper : IMapper
{
    public Customer MapTo(Shared.Database.Entities.Customer src) =>
        new()
        {
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Id = src.Id,
            Identities = MapTo(src.Identities).ToList()
        };

    public AccountCreateOptions MapToStripeAccountRequest(Organization src) =>
        new()
        {
            BusinessProfile = new AccountBusinessProfileOptions
            {
                Name = src.Name,
                Url = string.IsNullOrWhiteSpace(src.Website) ? null : src.Website,
                SupportUrl = string.IsNullOrWhiteSpace(src.Website) ? null : src.Website,
                SupportEmail = string.IsNullOrWhiteSpace(src.ContactEmail) ? null : src.ContactEmail,
                SupportPhone = string.IsNullOrWhiteSpace(src.ContactPhone) ? null : src.ContactPhone
            },
            Company = new AccountCompanyOptions
            {
                Name = src.Name,
                Address = src.PhysicalAddress is null
                    ? null
                    : new AddressOptions
                    {
                        Line1 = src.PhysicalAddress?.AddressLine1.ToSafeString(),
                        Line2 = src.PhysicalAddress?.AddressLine2.ToSafeString(),
                        City = src.PhysicalAddress?.City.ToSafeString(),
                        State = src.PhysicalAddress?.Province.ToSafeString(),
                        PostalCode = src.PhysicalAddress?.Zipcode.ToSafeString(),
                        Country = src.PhysicalAddress?.Country.ToSafeString()
                    },
                Phone = string.IsNullOrWhiteSpace(src.ContactPhone) ? null : src.ContactPhone
            },
            BusinessType = "company",
            Email = string.IsNullOrWhiteSpace(src.ContactEmail) ? null : src.ContactEmail,
            Capabilities =
                new AccountCapabilitiesOptions
                {
                    CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                    Transfers = new AccountCapabilitiesTransfersOptions { Requested = true }
                },
            Type = "standard",
            Metadata = new Dictionary<string, string> { { "organizationId", src.Id } }
        };

    public StripeConnectAccount MapTo(Account src, string id, string name, Organization organization) =>
        new()
        {
            Id = id,
            StripeAccountId = src.Id,
            Name = name,
            ChargesEnabled = src.ChargesEnabled,
            PayoutsEnabled = src.PayoutsEnabled,
            Type = src.Type.ToSafeString(),
            Country = src.Country,
            DefaultCurrency = src.DefaultCurrency,
            BusinessType = src.BusinessType,
            CompanyName = src.Company?.Name,
            Url = organization.Website,
            SupportUrl = organization.Website,
            ContactEmail = src.Email,
            ContactPhone = src.Company?.Phone,
            DetailsSubmitted = src.DetailsSubmitted,
            CapabilitiesCardPayments = src.Capabilities.CardPayments.ToSafeString(),
            CapabilitiesTransfers = src.Capabilities.Transfers.ToSafeString(),
            Organization = organization
        };

    public Shared.Models.StripeConnectAccount MapTo(StripeConnectAccount src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            StripeAccountId = src.StripeAccountId,
            Name = src.Name,
            ChargesEnabled = src.ChargesEnabled,
            PayoutsEnabled = src.PayoutsEnabled,
            Type = src.Type,
            Country = src.Country,
            DefaultCurrency = src.DefaultCurrency,
            BusinessType = src.BusinessType,
            Url = src.Url,
            SupportUrl = src.SupportUrl,
            CompanyName = src.CompanyName,
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            DetailsSubmitted = src.DetailsSubmitted,
            CapabilitiesCardPayments = src.CapabilitiesCardPayments,
            CapabilitiesTransfers = src.CapabilitiesTransfers,
            OnboardingUrl = src.OnboardingUrl,
            Organization = MapTo(src.Organization!),
            StripeConnectAccountAuthorization = MapTo(src.StripeConnectAccountAuthorization)
        };

    private static IEnumerable<Shared.Models.Identity> MapTo(IEnumerable<Identity?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static Shared.Models.Identity? MapTo(Identity? src) =>
        src is null
            ? null
            : new Shared.Models.Identity { Id = src.Id, CreatedAt = src.CreatedAt, ModifiedAt = src.ModifiedAt };

    private static Shared.Models.Organization MapTo(Organization src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Name = src.Name,
            Website = src.Website,
            Type = src.Type.ToOrganizationType(),
            MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy()
        };

    private static StripeConnectAccountAuthorization? MapTo(Shared.Database.Entities.StripeConnectAccountAuthorization? src) =>
        src is null
            ? null
            : new StripeConnectAccountAuthorization
            {
                Id = src.Id, CreatedAt = src.CreatedAt, ModifiedAt = src.ModifiedAt, IsAuthorized = src.IsAuthorized
            };
}
