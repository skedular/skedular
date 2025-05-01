using Api.Shared.Clients.Events.Skedular.Payment.V1.Value;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Payment.Shared.Models;
using Stripe;
using Customer = Payment.Shared.Models.Customer;
using PaymentStatus = Api.Shared.Clients.Events.Skedular.Payment.V1.Value.PaymentStatus;
using Product = Payment.Shared.Models.Product;
using StripeConnectAccount = Api.Shared.Clients.Events.Skedular.Payment.V1.Value.StripeConnectAccount;

namespace Payment.Shared.Mappers;

public interface IMapper
{
    StripeConnectAccount MapTo(Models.StripeConnectAccount src);
    CustomerCreateOptions MapTo(Organization src);
    CustomerUpdateOptions MergeTo(Organization src);
    CustomerCreateOptions MapTo(Customer src);
    CustomerUpdateOptions MergeTo(Customer src);
    BookingPaymentCreatedDetails MapToBookingPaymentCreatedDetails(StripeCheckoutSession src);
    BookingPaymentDetails MapToBookingPaymentDetails(StripeCheckoutSession src);
    ProductCreateOptions MapToProduct(ProductVersion src, Product product, string organizationId);
    PriceCreateOptions MapToPrice(ProductVersion src, Product product, string organizationId, string stripeProductId);
}

public class Mapper : IMapper
{
    public StripeConnectAccount MapTo(Models.StripeConnectAccount src) =>
        new()
        {
            Id = src.Id,
            OrganizationId = src.Organization is null ? string.Empty : src.Organization.Id,
            StripeAccountId = src.StripeAccountId,
            Name = src.Name.ToSafeString(),
            ChargesEnabled = src.ChargesEnabled,
            PayoutsEnabled = src.PayoutsEnabled,
            Type = src.Type.ToSafeString(),
            Country = src.Country.ToSafeString(),
            DefaultCurrency = src.DefaultCurrency.ToSafeString(),
            BusinessType = src.BusinessType.ToSafeString(),
            CompanyName = src.CompanyName.ToSafeString(),
            Url = src.Url.ToSafeString(),
            SupportUrl = src.SupportUrl.ToSafeString(),
            ContactEmail = src.ContactEmail.ToSafeString(),
            ContactPhone = src.ContactPhone.ToSafeString(),
            CapabilitiesCardPayments = src.CapabilitiesCardPayments.ToSafeString(),
            CapabilitiesTransfers = src.CapabilitiesTransfers.ToSafeString(),
            OnboardingUrl = src.OnboardingUrl.ToSafeString(),
            OnboardingCompleted = src.OnboardingCompleted
        };

    public CustomerCreateOptions MapTo(Organization src) =>
        new()
        {
            Name = src.Name,
            Email = string.IsNullOrWhiteSpace(src.ContactEmail) ? null : src.ContactEmail,
            Phone = string.IsNullOrWhiteSpace(src.ContactPhone) ? null : src.ContactPhone,
            Metadata = new Dictionary<string, string> { { "type", "organization" }, { "organizationId", src.Id } }
        };

    public CustomerUpdateOptions MergeTo(Organization src) =>
        new()
        {
            Name = src.Name,
            Email = string.IsNullOrWhiteSpace(src.ContactEmail) ? null : src.ContactEmail,
            Phone = string.IsNullOrWhiteSpace(src.ContactPhone) ? null : src.ContactPhone,
            Metadata = new Dictionary<string, string> { { "type", "organization" }, { "organizationId", src.Id } }
        };

    public CustomerCreateOptions MapTo(Customer src) =>
        new()
        {
            Name = src.ToDisplayableName(),
            Email = src.Identities.ToSingleEmail(),
            Phone = src.PhoneNumber.ToSafeString(),
            PreferredLocales = string.IsNullOrWhiteSpace(src.Locale) ? [] : [src.Locale],
            Metadata = new Dictionary<string, string> { { "type", "customer" }, { "customerId", src.Id } }
        };

    public CustomerUpdateOptions MergeTo(Customer src) =>
        new()
        {
            Name = src.ToDisplayableName(),
            Email = src.Identities.ToSingleEmail(),
            Phone = src.PhoneNumber.ToSafeString(),
            PreferredLocales = string.IsNullOrWhiteSpace(src.Locale) ? [] : [src.Locale],
            Metadata = new Dictionary<string, string> { { "type", "customer" }, { "customerId", src.Id } }
        };

    public BookingPaymentCreatedDetails MapToBookingPaymentCreatedDetails(StripeCheckoutSession src) =>
        new() { Id = src.Booking!.Id, PaymentReferenceId = src.Id, CheckoutUrl = src.Url.ToSafeString() };

    public BookingPaymentDetails MapToBookingPaymentDetails(StripeCheckoutSession src) =>
        new()
        {
            Id = src.Booking!.Id,
            PaymentReferenceId = src.Id,
            PaymentStatus = src.PaymentStatus switch
            {
                "no_payment_required" => PaymentStatus.NoPaymentRequired,
                "paid" => PaymentStatus.Paid,
                "unpaid" => PaymentStatus.Unpaid,
                _ => throw new ArgumentOutOfRangeException()
            }
        };

    public ProductCreateOptions MapToProduct(ProductVersion src, Product product, string organizationId) =>
        new()
        {
            Name = src.Name.ToSafeString(),
            UnitLabel = src.PriceUnit.ToPriceUnitName(),
            Metadata = new Dictionary<string, string>
            {
                { "productId", product.Id }, { "productVersionId", src.Id }, { "organizationId", organizationId }
            }
        };

    public PriceCreateOptions MapToPrice(ProductVersion src, Product product, string organizationId, string stripeProductId) =>
        new()
        {
            Currency = src.Currency.ToCurrency(),
            BillingScheme = "per_unit",
            UnitAmountDecimal = src.Price * 100,
            Product = stripeProductId,
            Metadata = new Dictionary<string, string> { { "productId", product.Id }, { "organizationId", organizationId } }
        };
}
