using Api.Shared.Services.Models;
using Enterprise.Shared;
using HotChocolate.Types.Pagination;
using Payment.Api.GraphQL;
using Payment.Shared.Models;
using Stripe;
using Customer = Payment.Shared.Models.Customer;
using Identity = Payment.Shared.Database.Entities.Identity;
using Organization = Payment.Shared.Database.Entities.Organization;
using PaymentMethod = Payment.Api.GraphQL.PaymentMethod;
using StripeConnectAccount = Payment.Shared.Database.Entities.StripeConnectAccount;
using StripePaymentMethod = Payment.Shared.Models.StripePaymentMethod;

namespace Payment.Api.Mappers;

public interface IMapper
{
    IEnumerable<PaymentMethod> MapTo(IEnumerable<StripePaymentMethod> src);
    Shared.Database.Entities.StripePaymentMethod MergeTo(Stripe.PaymentMethod paymentMethod, Shared.Database.Entities.StripePaymentMethod dest);
    Customer MapTo(Shared.Database.Entities.Customer src);
    IEnumerable<StripePaymentMethod> MapTo(IEnumerable<Shared.Database.Entities.StripePaymentMethod> src);
    AccountCreateOptions MapToStripeAccountRequest(Organization src);
    StripeConnectAccount MapTo(Account src, string id, string name, Organization organization);
    Shared.Models.StripeConnectAccount MapTo(StripeConnectAccount src);
    OrganizationStripeConnectAccountDetails? MapTo(Shared.Models.StripeConnectAccount? src);
    OrganizationStripeConnectAccountEdge MapTo(Edge<Shared.Models.StripeConnectAccount> src);
}

public class Mapper : IMapper
{
    public IEnumerable<PaymentMethod> MapTo(IEnumerable<StripePaymentMethod> src) => src.Select(MapTo);

    public Shared.Database.Entities.StripePaymentMethod MergeTo(Stripe.PaymentMethod paymentMethod, Shared.Database.Entities.StripePaymentMethod dest)
    {
        dest.PaymentMethodId = paymentMethod.Id;

        if (paymentMethod.Card is null)
        {
            return dest;
        }

        dest.CardBrand = paymentMethod.Card.Brand;
        dest.CardCountry = paymentMethod.Card.Country;
        dest.CardDescription = paymentMethod.Card.Description;
        dest.CardExpiryMonth = (byte)paymentMethod.Card.ExpMonth;
        dest.CardExpiryYear = (short)paymentMethod.Card.ExpYear;
        dest.CardFingerprint = paymentMethod.Card.Fingerprint;
        dest.CardFunding = paymentMethod.Card.Funding;
        dest.CardIssuer = paymentMethod.Card.Issuer;
        dest.CardLastFourDigit = paymentMethod.Card.Last4;
        return dest;
    }

    public Customer MapTo(Shared.Database.Entities.Customer src) =>
        new()
        {
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Id = src.Id,
            Identities = MapTo(src.Identities).ToList()
        };

    public IEnumerable<StripePaymentMethod> MapTo(IEnumerable<Shared.Database.Entities.StripePaymentMethod> src) => src.Select(MapTo);

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

    public OrganizationStripeConnectAccountDetails? MapTo(Shared.Models.StripeConnectAccount? src) =>
        src is null
            ? null
            : new OrganizationStripeConnectAccountDetails
            {
                Id = src.Id,
                Name = src.Name,
                ChargesEnabled = src.ChargesEnabled,
                PayoutsEnabled = src.PayoutsEnabled,
                Type = src.Type,
                Country = src.Country,
                DefaultCurrency = src.DefaultCurrency,
                BusinessType = src.BusinessType,
                CompanyName = src.CompanyName,
                Url = src.Url,
                SupportUrl = src.SupportUrl,
                ContactEmail = src.ContactEmail,
                ContactPhone = src.ContactPhone,
                CapabilitiesCardPayments = src.CapabilitiesCardPayments,
                CapabilitiesTransfers = src.CapabilitiesTransfers,
                OnboardingUrl = src.OnboardingUrl,
                OnboardingCompleted = src.OnboardingCompleted,
                Organization = MapTo(src.Organization!)
            };

    public OrganizationStripeConnectAccountEdge MapTo(Edge<Shared.Models.StripeConnectAccount> src) => new(MapTo(src.Node)!, src.Cursor);

    private static StripePaymentMethod MapTo(Shared.Database.Entities.StripePaymentMethod src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            SetupIntentId = src.SetupIntentId,
            ClientSecret = src.ClientSecret,
            Status = src.Status.ToStripePaymentMethodStatus(),
            PaymentMethodId = src.PaymentMethodId,
            CardBrand = src.CardBrand,
            CardCountry = src.CardCountry,
            CardDescription = src.CardDescription,
            CardExpiryMonth = src.CardExpiryMonth,
            CardExpiryYear = src.CardExpiryYear,
            CardFingerprint = src.CardFingerprint,
            CardFunding = src.CardFunding,
            CardIssuer = src.CardIssuer,
            CardLastFourDigit = src.CardLastFourDigit
        };

    private static PaymentMethod MapTo(StripePaymentMethod src) =>
        new()
        {
            Id = src.Id,
            CardBrand = src.CardBrand,
            CardCountry = src.CardCountry,
            CardDescription = src.CardDescription,
            CardExpiryMonth = src.CardExpiryMonth,
            CardExpiryYear = src.CardExpiryYear,
            CardFingerprint = src.CardFingerprint,
            CardFunding = src.CardFunding,
            CardIssuer = src.CardIssuer,
            CardLastFourDigit = src.CardLastFourDigit
        };

    private static IEnumerable<Shared.Models.Identity> MapTo(IEnumerable<Identity?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static Shared.Models.Identity? MapTo(Identity? src) =>
        src is null
            ? null
            : new Shared.Models.Identity { Id = src.Id, CreatedAt = src.CreatedAt, ModifiedAt = src.ModifiedAt };

    private static OrganizationDetails MapTo(Shared.Models.Organization src) =>
        new() { UniqueId = src.Id, Name = src.Name.ToSafeString(), Website = src.Website };

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
